// US-INF-1.3 (#39) + US-INF-1.1 (#37): Storage account + private cat-photos container.
@description('Azure region for the storage account.')
param location string

@description('Globally-unique storage account name (3-24 lowercase alphanumeric).')
param storageAccountName string

@description('Blob container that holds cat photos.')
param containerName string = 'cat-photos'

@description('Tags applied to every resource.')
param tags object

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    // Private container access only (no anonymous read).
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

resource catPhotos 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobService
  name: containerName
  properties: {
    // Private: SAS tokens are required for client-side upload/read.
    publicAccess: 'None'
  }
}

@description('Name of the created storage account.')
output name string = storageAccount.name

@description('Primary blob endpoint, e.g. https://stfosterflowprodweu.blob.core.windows.net/.')
output blobEndpoint string = storageAccount.properties.primaryEndpoints.blob

@description('Resource ID of the storage account.')
output id string = storageAccount.id
