// US-INF-1.1 (#37): Key Vault + secrets. Connection strings are constructed here so the
// raw account key / SQL password never leave Key Vault via deployment outputs.
@description('Azure region for the Key Vault.')
param location string

@description('Globally-unique Key Vault name (3-24 alphanumeric/hyphen).')
param keyVaultName string

@description('Azure AD tenant that owns the vault.')
param tenantId string = subscription().tenantId

@description('Tags applied to every resource.')
param tags object

@description('Storage account name used to derive the blob connection string.')
param storageAccountName string

@description('SQL server FQDN used to derive the database connection string.')
param sqlServerFqdn string

@description('SQL database name.')
param sqlDatabaseName string

@description('SQL administrator login.')
param sqlAdminLogin string

@description('SQL administrator password.')
@secure()
param sqlAdminPassword string

@description('JWT signing secret (32+ chars).')
@secure()
param jwtSecret string

@description('Loki ingestion URL for monitoring.')
param lokiUrl string = ''

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

var blobConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

var dbConnectionString = 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    // RBAC data-plane: App Service identity gets "Key Vault Secrets User".
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enabledForTemplateDeployment: true
  }
}

resource jwtSecretValue 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'JwtSecret'
  properties: {
    value: jwtSecret
  }
}

resource dbSecretValue 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'DbConnectionString'
  properties: {
    value: dbConnectionString
  }
}

resource blobSecretValue 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'BlobStorageConnectionString'
  properties: {
    value: blobConnectionString
  }
}

resource lokiSecretValue 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = if (!empty(lokiUrl)) {
  parent: keyVault
  name: 'LokiUrl'
  properties: {
    value: lokiUrl
  }
}

@description('Name of the created Key Vault.')
output name string = keyVault.name

@description('Vault URI, e.g. https://kv-fosterflow-prod-swe.vault.azure.net/.')
output vaultUri string = keyVault.properties.vaultUri
