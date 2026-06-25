// Resource-group-scoped orchestrator: wires storage, SQL, Key Vault and App Service together.
@description('Azure region for all resources.')
param location string

@description('Tags applied to every resource.')
param tags object

@description('Storage account name (globally unique).')
param storageAccountName string

@description('Key Vault name (globally unique).')
param keyVaultName string

@description('SQL logical server name (globally unique).')
param sqlServerName string

@description('SQL database name.')
param sqlDatabaseName string

@description('SQL administrator login.')
param sqlAdminLogin string

@description('SQL administrator password.')
@secure()
param sqlAdminPassword string

@description('Optional local client IP to whitelist on the SQL firewall.')
param allowedClientIp string = ''

@description('JWT signing secret (32+ chars).')
@secure()
param jwtSecret string

@description('Loki ingestion URL for monitoring (optional until ready).')
param lokiUrl string = ''

@description('Deploy the Grafana/Prometheus/Loki monitoring stack on ACI (US-INF-4.3, #49).')
param deployMonitoring bool = false

@description('Grafana admin password for the monitoring stack.')
@secure()
param grafanaAdminPassword string = ''

@description('App Service Plan name.')
param appServicePlanName string

@description('App Service / web app name.')
param appServiceName string

@description('Linux runtime stack for the App Service.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

@description('Container group name for the monitoring stack.')
param monitoringContainerGroupName string = 'ci-fosterflow-monitoring'

@description('DNS name label for the monitoring public IP (Grafana).')
param monitoringDnsNameLabel string = 'fosterflow-grafana'

@description('Storage account name backing the monitoring config share (globally unique).')
param monitoringStorageAccountName string = 'stfosterflowmon'

module storage 'modules/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    storageAccountName: storageAccountName
    tags: tags
  }
}

module sql 'modules/sql.bicep' = {
  name: 'sql'
  params: {
    location: location
    sqlServerName: sqlServerName
    databaseName: sqlDatabaseName
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    allowedClientIp: allowedClientIp
    tags: tags
  }
}

module keyVault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    keyVaultName: keyVaultName
    tags: tags
    storageAccountName: storage.outputs.name
    sqlServerFqdn: sql.outputs.fullyQualifiedDomainName
    sqlDatabaseName: sql.outputs.databaseName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    jwtSecret: jwtSecret
    lokiUrl: effectiveLokiUrl
  }
}

var publicUrl = 'https://${appServiceName}.azurewebsites.net'

// When the monitoring stack is deployed but no explicit Loki URL was supplied, point the
// App Service at the ACI Loki endpoint. The container group's public FQDN is deterministic
// (<dnsNameLabel>.<region>.azurecontainer.io), so this needs no cross-module dependency.
var effectiveLokiUrl = !empty(lokiUrl)
  ? lokiUrl
  : (deployMonitoring ? 'http://${monitoringDnsNameLabel}.${location}.azurecontainer.io:3100' : '')

module monitoring 'modules/monitoring.bicep' = if (deployMonitoring) {
  name: 'monitoring'
  params: {
    location: location
    tags: tags
    containerGroupName: monitoringContainerGroupName
    dnsNameLabel: monitoringDnsNameLabel
    storageAccountName: monitoringStorageAccountName
    grafanaAdminPassword: grafanaAdminPassword
  }
}

module appService 'modules/appservice.bicep' = {
  name: 'appservice'
  params: {
    location: location
    appServicePlanName: appServicePlanName
    appServiceName: appServiceName
    linuxFxVersion: linuxFxVersion
    keyVaultName: keyVault.outputs.name
    jwtIssuer: publicUrl
    jwtAudience: publicUrl
    lokiConfigured: !empty(effectiveLokiUrl)
    tags: tags
  }
}

@description('Public application URL.')
output appUrl string = publicUrl

@description('Storage blob endpoint.')
output blobEndpoint string = storage.outputs.blobEndpoint

@description('SQL server FQDN.')
output sqlServerFqdn string = sql.outputs.fullyQualifiedDomainName

@description('Key Vault URI.')
output keyVaultUri string = keyVault.outputs.vaultUri

@description('Effective Loki ingestion URL wired into the App Service (empty if logging to Loki is disabled).')
output lokiUrl string = effectiveLokiUrl

@description('Public Grafana URL (empty unless deployMonitoring is true).')
output grafanaUrl string = deployMonitoring ? monitoring!.outputs.grafanaUrl : ''
