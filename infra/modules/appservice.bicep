// US-INF-1.4 (#40): App Service Plan (B1 Linux) + App Service hosting the API + Blazor WASM.
@description('Azure region for the plan and app.')
param location string

@description('App Service Plan name.')
param appServicePlanName string

@description('App Service (web app) name. Also the public host: <name>.azurewebsites.net.')
param appServiceName string

@description('Linux runtime stack. Repo targets .NET 10; override if deploying a different runtime.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

@description('Key Vault name that holds the referenced secrets.')
param keyVaultName string

@description('JWT issuer (public app URL).')
param jwtIssuer string

@description('JWT audience (public app URL).')
param jwtAudience string

@description('Tags applied to every resource.')
param tags object

@description('Whether a LokiUrl secret exists in Key Vault and should be referenced.')
param lokiConfigured bool = false

var baseAppSettings = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Production'
  }
  {
    name: 'Jwt__Issuer'
    value: jwtIssuer
  }
  {
    name: 'Jwt__Audience'
    value: jwtAudience
  }
  {
    name: 'Jwt__Secret'
    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=JwtSecret)'
  }
  {
    name: 'ConnectionStrings__Database'
    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=DbConnectionString)'
  }
  {
    name: 'ConnectionStrings__BlobStorage'
    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=BlobStorageConnectionString)'
  }
]

// Only reference the LokiUrl secret once it has been provided.
var lokiAppSettings = lokiConfigured ? [
  {
    name: 'Loki__Url'
    value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=LokiUrl)'
  }
] : []

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      healthCheckPath: '/health'
      http20Enabled: true
      appSettings: concat(baseAppSettings, lokiAppSettings)
    }
  }
}

// Existing reference to scope the role assignment to the vault.
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// Built-in role: Key Vault Secrets User.
var keyVaultSecretsUserRoleId = '4633458b-17de-408a-b874-0445c86b69e6'

resource secretsUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, appService.id, keyVaultSecretsUserRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

@description('Default host name, e.g. app-fosterflow-prod-swe.azurewebsites.net.')
output defaultHostName string = appService.properties.defaultHostName

@description('App Service principal ID (system-assigned identity).')
output principalId string = appService.identity.principalId
