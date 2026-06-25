// US-INF-4.3 (#49): Monitoring stack on Azure Container Instances.
// Runs Grafana, Prometheus and Loki as a single container group. The three
// containers share the group's network namespace, so Grafana reaches Prometheus
// and Loki over localhost (see grafana datasource provisioning). Configuration
// and dashboards are mounted from an Azure Files share that is populated by
// infra/upload-monitoring-config.sh after the deployment completes.
@description('Azure region for the monitoring resources.')
param location string

@description('Tags applied to every resource.')
param tags object

@description('Name of the container group (e.g. ci-fosterflow-prod-swe).')
param containerGroupName string

@description('DNS name label for the public IP. Grafana is served at https://<label>.<region>.azurecontainer.io.')
param dnsNameLabel string

@description('Storage account name that backs the Azure Files config share (globally unique, 3-24 lowercase alphanumeric).')
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

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
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
  properties: {
    shareQuota: 5
  }
}

var storageKey = storage.listKeys().keys[0].value

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: containerGroupName
  location: location
  tags: tags
  dependsOn: [
    configShare
  ]
  properties: {
    osType: 'Linux'
    restartPolicy: 'Always'
    ipAddress: {
      type: 'Public'
      dnsNameLabel: dnsNameLabel
      ports: [
        {
          protocol: 'TCP'
          port: 3000
        }
      ]
    }
    volumes: [
      {
        name: 'config'
        azureFile: {
          shareName: configShareName
          storageAccountName: storage.name
          storageAccountKey: storageKey
        }
      }
    ]
    containers: [
      {
        name: 'grafana'
        properties: {
          image: grafanaImage
          ports: [
            {
              protocol: 'TCP'
              port: 3000
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: json('1.0')
            }
          }
          environmentVariables: [
            {
              name: 'GF_SECURITY_ADMIN_USER'
              value: grafanaAdminUser
            }
            {
              name: 'GF_SECURITY_ADMIN_PASSWORD'
              secureValue: grafanaAdminPassword
            }
            {
              name: 'GF_USERS_ALLOW_SIGN_UP'
              value: 'false'
            }
            {
              name: 'GF_PATHS_PROVISIONING'
              value: '/mnt/config/grafana/provisioning'
            }
          ]
          volumeMounts: [
            {
              name: 'config'
              mountPath: '/mnt/config'
              readOnly: true
            }
          ]
        }
      }
      {
        name: 'prometheus'
        properties: {
          image: prometheusImage
          command: [
            '/bin/prometheus'
            '--config.file=/mnt/config/prometheus/prometheus.prod.yml'
            '--storage.tsdb.path=/prometheus'
            '--storage.tsdb.retention.time=30d'
          ]
          ports: [
            {
              protocol: 'TCP'
              port: 9090
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: json('1.0')
            }
          }
          volumeMounts: [
            {
              name: 'config'
              mountPath: '/mnt/config'
              readOnly: true
            }
          ]
        }
      }
      {
        name: 'loki'
        properties: {
          image: lokiImage
          command: [
            '/usr/bin/loki'
            '-config.file=/mnt/config/loki/loki-config.yml'
          ]
          ports: [
            {
              protocol: 'TCP'
              port: 3100
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: json('1.0')
            }
          }
          volumeMounts: [
            {
              name: 'config'
              mountPath: '/mnt/config'
              readOnly: true
            }
          ]
        }
      }
    ]
  }
}

@description('Public Grafana URL.')
output grafanaUrl string = 'http://${containerGroup.properties.ipAddress.fqdn}:3000'

@description('Loki ingestion URL reachable from inside the container group.')
output lokiInternalUrl string = 'http://localhost:3100'

@description('Storage account name backing the config share.')
output storageAccountName string = storage.name

@description('Config file share name to upload Prometheus/Loki/Grafana files into.')
output configShareName string = configShareName
