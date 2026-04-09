@description('Location for the Key Vault')
param location string

@description('Name of the Key Vault')
param keyVaultName string

@description('Principal ID of the Managed Identity that will access this Key Vault')
param managedIdentityObjectId string

@description('Tags for the resource')
param tags object = {}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: managedIdentityObjectId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
          certificates: [
            'get'
            'list'
          ]
          keys: [
            'get'
            'list'
          ]
        }
      }
    ]
    enabledForDeployment: true
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    softDeleteRetentionInDays: 7
    enablePurgeProtection: false
  }
  tags: tags
}

output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
