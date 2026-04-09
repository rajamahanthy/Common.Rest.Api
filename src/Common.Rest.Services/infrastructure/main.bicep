@description('Environment name (dev, test, prod)')
param environment string = 'dev'

@description('Azure region for resource deployment')
param location string

@description('Resource naming prefix (e.g., VO)')
param resourcePrefix string = 'VO'

@description('SQL Server admin username')
param sqlAdminUsername string

@description('App Service Plan SKU')
param appServicePlanSku string = 'S1'

@description('App Service Plan tier')
param appServicePlanTier string = 'Standard'

@description('SQL Database edition')
param sqlDatabaseEdition string = 'Standard'

@description('SQL Database service tier (S0, S1, S2)')
param sqlDatabaseTier string = 'S1'

@description('Application Insights retention in days')
param appInsightsRetentionDays int = 30

@description('Tags for all resources')
param tags object = {
  environment: environment
  project: 'Common.Rest.Api'
  managedBy: 'Bicep'
  createdDate: utcNow('u')
}

// Variables
var resourceGroupName = '${resourcePrefix}-${environment}-rg'
var keyVaultName = '${resourcePrefix}${environment}kv${uniqueString(resourceGroup().id)}'
var appServicePlanName = '${resourcePrefix}-${environment}-asp'
var appServiceName = '${resourcePrefix}-${environment}-app'
var sqlServerName = '${resourcePrefix}${environment}sql${uniqueString(resourceGroup().id)}'
var surveyDbName = '${resourcePrefix}${environment}-survey-db'
var addressDbName = '${resourcePrefix}${environment}-address-db'
var appInsightsName = '${resourcePrefix}-${environment}-ai'
var logAnalyticsName = '${resourcePrefix}-${environment}-log'
var managedIdentityName = '${resourcePrefix}-${environment}-mi'
var storageAccountName = '${resourcePrefix}${environment}sa${uniqueString(resourceGroup().id)}'

// Deploy Managed Identity
module managedIdentity './modules/managed-identity.bicep' = {
  name: 'managedIdentity-${uniqueString(deployment().name)}'
  params: {
    location: location
    managedIdentityName: managedIdentityName
    tags: tags
  }
}

// Deploy Log Analytics Workspace
module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring-${uniqueString(deployment().name)}'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName
    appInsightsName: appInsightsName
    retentionDays: appInsightsRetentionDays
    tags: tags
  }
}

// Deploy Key Vault
module keyVault './modules/key-vault.bicep' = {
  name: 'keyVault-${uniqueString(deployment().name)}'
  params: {
    location: location
    keyVaultName: keyVaultName
    managedIdentityObjectId: managedIdentity.outputs.managedIdentityPrincipalId
    tags: tags
  }
}

// Deploy SQL Database
module sqlDatabase './modules/sql-database.bicep' = {
  name: 'sqlDatabase-${uniqueString(deployment().name)}'
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlAdminUsername: sqlAdminUsername
    surveyDbName: surveyDbName
    addressDbName: addressDbName
    databaseEdition: sqlDatabaseEdition
    databaseTier: sqlDatabaseTier
    keyVaultName: keyVault.outputs.keyVaultName
    managedIdentityPrincipalId: managedIdentity.outputs.managedIdentityPrincipalId
    managedIdentityClientId: managedIdentity.outputs.managedIdentityClientId
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
  dependsOn: [
    keyVault
    managedIdentity
  ]
}

// Deploy App Service
module appService './modules/app-service.bicep' = {
  name: 'appService-${uniqueString(deployment().name)}'
  params: {
    location: location
    appServicePlanName: appServicePlanName
    appServiceName: appServiceName
    appServicePlanSku: appServicePlanSku
    appServicePlanTier: appServicePlanTier
    managedIdentityId: managedIdentity.outputs.managedIdentityId
    managedIdentityClientId: managedIdentity.outputs.managedIdentityClientId
    keyVaultName: keyVault.outputs.keyVaultName
    keyVaultId: keyVault.outputs.keyVaultId
    appInsightsInstrumentationKey: monitoring.outputs.appInsightsInstrumentationKey
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    sqlServerName: sqlDatabase.outputs.sqlServerName
    sqlServerFqdn: sqlDatabase.outputs.sqlServerFqdn
    surveyDbName: surveyDbName
    addressDbName: addressDbName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    environment: environment
    tags: tags
  }
  dependsOn: [
    keyVault
    sqlDatabase
    managedIdentity
    monitoring
  ]
}

// Outputs
output resourceGroupName string = resourceGroupName
output appServiceName string = appServiceName
output appServiceUrl string = 'https://${appService.outputs.appServiceHostname}'
output keyVaultName string = keyVault.outputs.keyVaultName
output sqlServerName string = sqlDatabase.outputs.sqlServerName
output sqlServerFqdn string = sqlDatabase.outputs.sqlServerFqdn
output appInsightsName string = appInsightsName
output managedIdentityClientId string = managedIdentity.outputs.managedIdentityClientId
output managedIdentityId string = managedIdentity.outputs.managedIdentityId
