// FosterFlow production infrastructure (Epic 0 — issues #37, #38, #39, #40).
// Subscription-scope entry point: creates the resource group (#37) then deploys everything into it.
targetScope = 'subscription'

@description('Azure region for the resource group and all resources.')
param location string = 'swedencentral'

// ── Cloud Adoption Framework naming tokens ────────────────────────────
// Pattern: <resource-type-abbreviation>-<workload>-<env>-<region>
// (storage account omits hyphens per its naming rules).
@description('Workload / application name used in resource names.')
param workload string = 'fosterflow'

@description('Environment token used in resource names (e.g. prod, dev, test).')
param environmentAbbreviation string = 'prod'

@description('Region abbreviation used in resource names (e.g. swe for Sweden Central).')
param regionAbbreviation string = 'swe'

@description('Resource group name.')
param resourceGroupName string = 'rg-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('Deployment environment tag value.')
param environmentName string = 'production'

@description('Storage account name (globally unique, 3-24 lowercase alphanumeric, no hyphens).')
param storageAccountName string = 'st${workload}${environmentAbbreviation}${regionAbbreviation}'

@description('Key Vault name (globally unique, 3-24 chars).')
param keyVaultName string = 'kv-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('SQL logical server name (globally unique, lowercase).')
param sqlServerName string = 'sql-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('SQL database name.')
param sqlDatabaseName string = 'sqldb-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('SQL administrator login.')
param sqlAdminLogin string = 'fosterflowadmin'

@description('SQL administrator password. Provide securely at deploy time.')
@secure()
param sqlAdminPassword string

@description('Optional local client IP to whitelist on the SQL firewall (for migrations/SSMS).')
param allowedClientIp string = ''

@description('JWT signing secret (32+ chars). Provide securely at deploy time.')
@secure()
param jwtSecret string

@description('Loki ingestion URL for monitoring (optional until ready).')
param lokiUrl string = ''

@description('App Service Plan name.')
param appServicePlanName string = 'asp-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('App Service / web app name (becomes <name>.azurewebsites.net).')
param appServiceName string = 'app-${workload}-${environmentAbbreviation}-${regionAbbreviation}'

@description('Linux runtime stack for the App Service. Repo targets .NET 10.')
param linuxFxVersion string = 'DOTNETCORE|10.0'

var tags = {
  project: 'fosterflow'
  environment: environmentName
  hackathon: '2026'
}

resource resourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'fosterflow-resources'
  params: {
    location: location
    tags: tags
    storageAccountName: storageAccountName
    keyVaultName: keyVaultName
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    allowedClientIp: allowedClientIp
    jwtSecret: jwtSecret
    lokiUrl: lokiUrl
    appServicePlanName: appServicePlanName
    appServiceName: appServiceName
    linuxFxVersion: linuxFxVersion
  }
}

@description('Public application URL.')
output appUrl string = resources.outputs.appUrl

@description('Storage blob endpoint.')
output blobEndpoint string = resources.outputs.blobEndpoint

@description('SQL server FQDN.')
output sqlServerFqdn string = resources.outputs.sqlServerFqdn

@description('Key Vault URI.')
output keyVaultUri string = resources.outputs.keyVaultUri
