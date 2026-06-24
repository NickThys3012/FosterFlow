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

@description('App Service Plan name.')
param appServicePlanName string

@description('App Service / web app name.')
param appServiceName string

@description('Linux runtime stack for the App Service.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

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
    lokiUrl: lokiUrl
  }
}

var publicUrl = 'https://${appServiceName}.azurewebsites.net'

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
    lokiConfigured: !empty(lokiUrl)
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
