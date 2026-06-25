// Example parameter file. Copy to main.bicepparam (gitignored) and fill in secrets,
// or pass secure values on the CLI / via environment variables instead of committing them.
//
// Resource names follow the Microsoft Cloud Adoption Framework convention and are derived
// from the tokens below, e.g. rg-fosterflow-prod-swe, stfosterflowprodswe, kv-fosterflow-prod-swe.
using 'main.bicep'

param location = 'swedencentral'
param workload = 'fosterflow'
param environmentAbbreviation = 'prod'
param regionAbbreviation = 'swe'
param environmentName = 'production'
param linuxFxVersion = 'DOTNETCORE|10.0'

param sqlAdminLogin = 'fosterflowadmin'

// Never commit real secrets. Supply these at deploy time, e.g.:
//   az deployment sub create ... --parameters sqlAdminPassword=$SQL_PASSWORD jwtSecret=$JWT_SECRET
param sqlAdminPassword = ''
param jwtSecret = ''
param lokiUrl = ''
param allowedClientIp = ''
