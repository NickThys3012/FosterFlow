// Monitoring stack on a Linux VM (Ubuntu 22.04 LTS, Standard_B2s).
// ~70% cheaper than the previous Azure Container Instances setup:
//   ACI  (3 × 1 vCPU / 1 GB):  ~$118/month
//   Standard_B2s VM (2 vCPU / 4 GB):  ~$35/month  (incl. OS disk + public IP)
//
// Grafana, Prometheus and Loki run via Docker Compose. Grafana reaches the
// other services by Docker Compose service name (no localhost workaround needed).
// Configuration is mounted from an Azure Files share populated by
// infra/upload-monitoring-config.sh after the deployment completes.

@description('Azure region for monitoring resources.')
param location string

@description('Tags applied to every resource.')
param tags object

@description('VM resource name.')
param vmName string = 'vm-fosterflow-monitoring'

@description('DNS name label for the public IP (<label>.<region>.cloudapp.azure.com).')
param dnsNameLabel string

@description('Storage account name backing the Azure Files config share (3-24 lowercase alphanumeric).')
param storageAccountName string

@description('Azure Files share name holding Prometheus/Loki/Grafana config + dashboards.')
param configShareName string = 'monitoring-config'

@description('Grafana admin password.')
@secure()
param grafanaAdminPassword string

@description('Grafana admin user name.')
param grafanaAdminUser string = 'admin'

@description('Container image tags.')
param grafanaImage string = 'grafana/grafana:latest'
param prometheusImage string = 'prom/prometheus:latest'
param lokiImage string = 'grafana/loki:3.0.0'

@description('VM administrator username.')
param adminUsername string = 'monitoringadmin'

@description('VM administrator password. Use a strong password (12+ chars, mixed case, digits, symbols).')
@secure()
param adminPassword string

@description('VM SKU. Standard_B2s (2 vCPU / 4 GB RAM) comfortably runs all three services at ~$30/month.')
param vmSize string = 'Standard_B2s'

// ── Storage (Azure Files share for monitoring config) ──────────────────────

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
  }
}

resource fileService 'Microsoft.Storage/storageAccounts/fileServices@2023-05-01' = {
  parent: storage
  name: 'default'
}

resource configShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-05-01' = {
  parent: fileService
  name: configShareName
  properties: { shareQuota: 5 }
}

var storageKey = storage.listKeys().keys[0].value

// ── Networking ─────────────────────────────────────────────────────────────

resource nsg 'Microsoft.Network/networkSecurityGroups@2023-09-01' = {
  name: 'nsg-${vmName}'
  location: location
  tags: tags
  properties: {
    securityRules: [
      {
        name: 'allow-grafana'
        properties: {
          priority: 100
          protocol: 'Tcp'
          access: 'Allow'
          direction: 'Inbound'
          sourceAddressPrefix: 'Internet'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '3000'
        }
      }
      {
        name: 'allow-loki'
        properties: {
          // Loki ingestion endpoint must be reachable by the App Service so
          // Serilog can push logs over the internet.
          priority: 110
          protocol: 'Tcp'
          access: 'Allow'
          direction: 'Inbound'
          sourceAddressPrefix: 'Internet'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
          destinationPortRange: '3100'
        }
      }
    ]
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2023-09-01' = {
  name: 'vnet-${vmName}'
  location: location
  tags: tags
  properties: {
    addressSpace: { addressPrefixes: ['10.1.0.0/16'] }
    subnets: [
      {
        name: 'snet-monitoring'
        properties: {
          addressPrefix: '10.1.0.0/24'
          networkSecurityGroup: { id: nsg.id }
        }
      }
    ]
  }
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2023-09-01' = {
  name: 'pip-${vmName}'
  location: location
  tags: tags
  sku: { name: 'Standard' }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: { domainNameLabel: dnsNameLabel }
  }
}

resource nic 'Microsoft.Network/networkInterfaces@2023-09-01' = {
  name: 'nic-${vmName}'
  location: location
  tags: tags
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          subnet: { id: '${vnet.id}/subnets/snet-monitoring' }
          publicIPAddress: { id: publicIp.id }
          privateIPAllocationMethod: 'Dynamic'
        }
      }
    ]
  }
}

// ── cloud-init ─────────────────────────────────────────────────────────────
// File contents are base64-encoded inside the cloud-init YAML to sidestep
// indentation and special-character escaping issues. Docker Compose service
// names work for inter-container communication (unlike ACI's shared namespace
// which required localhost).

var cifsCredsContent = 'username=${storageAccountName}\npassword=${storageKey}\n'

var monitoringEnvContent = 'GF_SECURITY_ADMIN_USER=${grafanaAdminUser}\nGF_SECURITY_ADMIN_PASSWORD=${grafanaAdminPassword}\nGF_USERS_ALLOW_SIGN_UP=false\nGF_PATHS_PROVISIONING=/mnt/config/grafana/provisioning\n'

var dockerComposeContent = 'version: "3.8"\nservices:\n  grafana:\n    image: ${grafanaImage}\n    restart: always\n    ports:\n      - "3000:3000"\n    env_file: /etc/monitoring.env\n    volumes:\n      - /mnt/monitoring-config:/mnt/config:ro\n      - grafana_data:/var/lib/grafana\n  prometheus:\n    image: ${prometheusImage}\n    restart: always\n    command:\n      - "--config.file=/mnt/config/prometheus/prometheus.prod.yml"\n      - "--storage.tsdb.path=/prometheus"\n      - "--storage.tsdb.retention.time=30d"\n    volumes:\n      - /mnt/monitoring-config:/mnt/config:ro\n      - prometheus_data:/prometheus\n  loki:\n    image: ${lokiImage}\n    restart: always\n    command:\n      - "-config.file=/mnt/config/loki/loki-config.yml"\n    ports:\n      - "3100:3100"\n    volumes:\n      - /mnt/monitoring-config:/mnt/config:ro\n      - loki_data:/loki\nvolumes:\n  grafana_data:\n  prometheus_data:\n  loki_data:\n'

var cloudInitScript = '#cloud-config\npackage_update: true\npackages:\n  - docker.io\n  - docker-compose\n  - cifs-utils\n\nwrite_files:\n  - path: /etc/cifs-monitoring-creds\n    encoding: b64\n    permissions: "0600"\n    content: ${base64(cifsCredsContent)}\n  - path: /etc/monitoring.env\n    encoding: b64\n    permissions: "0600"\n    content: ${base64(monitoringEnvContent)}\n  - path: /opt/monitoring/docker-compose.yml\n    encoding: b64\n    permissions: "0644"\n    content: ${base64(dockerComposeContent)}\n\nruncmd:\n  - mkdir -p /mnt/monitoring-config\n  - chmod 600 /etc/cifs-monitoring-creds /etc/monitoring.env\n  - mount -t cifs //${storageAccountName}.file.core.windows.net/${configShareName} /mnt/monitoring-config -o vers=3.0,credentials=/etc/cifs-monitoring-creds,dir_mode=0755,file_mode=0644,serverino,nofail || true\n  - echo "//${storageAccountName}.file.core.windows.net/${configShareName} /mnt/monitoring-config cifs vers=3.0,credentials=/etc/cifs-monitoring-creds,dir_mode=0755,file_mode=0644,serverino,nofail 0 0" >> /etc/fstab\n  - systemctl enable docker\n  - systemctl start docker\n  - sleep 15\n  - cd /opt/monitoring && docker-compose up -d\n'

// ── Virtual Machine ────────────────────────────────────────────────────────

resource vm 'Microsoft.Compute/virtualMachines@2024-03-01' = {
  name: vmName
  location: location
  tags: tags
  dependsOn: [configShare]
  properties: {
    hardwareProfile: { vmSize: vmSize }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: '0001-com-ubuntu-server-jammy'
        sku: '22_04-lts-gen2'
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
        managedDisk: { storageAccountType: 'Standard_LRS' }
        diskSizeGB: 30
      }
    }
    osProfile: {
      computerName: vmName
      adminUsername: adminUsername
      adminPassword: adminPassword
      customData: base64(cloudInitScript)
      linuxConfiguration: {
        disablePasswordAuthentication: false
      }
    }
    networkProfile: {
      networkInterfaces: [{ id: nic.id }]
    }
  }
}

@description('Public Grafana URL.')
output grafanaUrl string = 'http://${publicIp.properties.dnsSettings.fqdn}:3000'

@description('Public Loki ingestion URL the App Service pushes logs to.')
output lokiPublicUrl string = 'http://${publicIp.properties.dnsSettings.fqdn}:3100'

@description('Storage account name backing the config share.')
output storageAccountName string = storage.name

@description('Config file share name to upload Prometheus/Loki/Grafana files into.')
output configShareName string = configShareName

@description('VM name — pass to "az vm run-command invoke" or "az vm restart" to manage the stack.')
output vmName string = vmName
