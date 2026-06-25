// US-INF-1.2 (#38): Azure SQL Server + database (Basic), firewall, 7-day backups.
@description('Azure region for the SQL server and database.')
param location string

@description('Globally-unique logical SQL server name (without the .database.windows.net suffix).')
param sqlServerName string

@description('Database name.')
param databaseName string = 'sqldb-fosterflow-prod-weu'

@description('SQL administrator login.')
param administratorLogin string

@description('SQL administrator password.')
@secure()
param administratorLoginPassword string

@description('Optional client IP to whitelist for local connections. Leave empty to skip.')
param allowedClientIp string = ''

@description('Tags applied to every resource.')
param tags object

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    // Geo-redundancy disabled for the hackathon.
    requestedBackupStorageRedundancy: 'Local'
  }
}

// 7-day point-in-time restore retention.
resource shortTermBackup 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2023-08-01-preview' = {
  parent: database
  name: 'default'
  properties: {
    retentionDays: 7
  }
}

// Firewall rule: allow other Azure services (App Service) to reach the server.
resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Optional firewall rule for a developer's local IP (for migrations / SSMS).
resource allowClientIp 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = if (!empty(allowedClientIp)) {
  parent: sqlServer
  name: 'AllowLocalClientIp'
  properties: {
    startIpAddress: allowedClientIp
    endIpAddress: allowedClientIp
  }
}

@description('Fully-qualified domain name, e.g. sql-fosterflow-prod-weu.database.windows.net.')
output fullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName

@description('Database name.')
output databaseName string = database.name
