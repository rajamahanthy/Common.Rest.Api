@description('Location for the App Service')
param location string

@description('App Service Plan name')
param appServicePlanName string

@description('App Service name')
param appServiceName string

@description('App Service Plan SKU (S1, S2, P1, P2, etc.)')
param appServicePlanSku string = 'S1'

@description('App Service Plan tier (Standard, Premium)')
param appServicePlanTier string = 'Standard'

@description('Managed Identity resource ID')
param managedIdentityId string

@description('Managed Identity client ID')
param managedIdentityClientId string

@description('Key Vault name')
param keyVaultName string

@description('Key Vault resource ID')
param keyVaultId string

@description('Application Insights instrumentation key')
param appInsightsInstrumentationKey string

@description('Application Insights connection string')
param appInsightsConnectionString string

@description('SQL Server name')
param sqlServerName string

@description('SQL Server FQDN')
param sqlServerFqdn string

@description('Survey database name')
param surveyDbName string

@description('Address database name')
param addressDbName string

@description('Log Analytics Workspace ID')
param logAnalyticsWorkspaceId string

@description('Environment (dev, test, prod)')
param environment string = 'dev'

@description('Tags for the resources')
param tags object = {}

// App Service Plan (Linux)
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: appServicePlanSku
    tier: appServicePlanTier
    capacity: 1
  }
  properties: {
    reserved: true
  }
  tags: tags
}

// App Service
resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      remoteDebuggingEnabled: false
      http20Enabled: true
      healthCheckPath: '/health'
      numberOfWorkers: 1
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'ASPNETCORE_URLS'
          value: 'http://+:80'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'recommended'
        }
        {
          name: 'KeyVault__VaultUri'
          value: 'https://${keyVaultName}.vault.azure.net/'
        }
        {
          name: 'KeyVault__ClientId'
          value: managedIdentityClientId
        }
        {
          name: 'ManagedIdentity__ClientId'
          value: managedIdentityClientId
        }
        {
          name: 'ConnectionStrings__SurveyDatabase'
          value: 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${surveyDbName};Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;Authentication=Active Directory Default;'
        }
        {
          name: 'ConnectionStrings__AddressDatabase'
          value: 'Server=tcp:${sqlServerFqdn},1433;Initial Catalog=${addressDbName};Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;Authentication=Active Directory Default;'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
  }
  tags: tags
}

// App Service Configuration
resource appServiceConfig 'Microsoft.Web/sites/config@2023-12-01' = {
  parent: appService
  name: 'web'
  properties: {
    numberOfWorkers: 1
    defaultDocuments: []
    netFrameworkVersion: 'v8.0'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: true
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: true
    publishingUsername: 'disabled'
    scmType: 'None'
    use32BitWorkerProcess: false
    webSocketsEnabled: true
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: true
      }
    ]
  }
}

// Diagnostic Settings for App Service
resource appServiceDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: appService
  name: '${appServiceName}-diagnostics'
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'AppServiceFileAuditLogs'
        enabled: false
        retentionPolicy: {
          days: 0
          enabled: false
        }
      }
      {
        category: 'AppServicePlatformLogs'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
  }
}

// Key Vault Access Policy for App Service
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: reference(managedIdentityId, '2023-01-31').principalId
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
  }
}

// Reference to existing Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

output appServiceId string = appService.id
output appServiceName string = appService.name
output appServiceHostname string = appService.properties.defaultHostName
output appServicePlanId string = appServicePlan.id
