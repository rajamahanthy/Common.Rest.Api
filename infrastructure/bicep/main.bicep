// ──────────────────────────────────────────────────────────────────────────
// Main Bicep deployment for the microservices platform
// Deploys: App Service Plan, App Services, SQL Servers, Key Vault, App Insights
// ──────────────────────────────────────────────────────────────────────────

targetScope = 'resourceGroup'

@description('Environment name (dev, staging, prod)')
@allowed(['dev', 'staging', 'prod'])
param environment string = 'dev'

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Base name for all resources')
param baseName string = 'restapi'

@description('SQL administrator login')
@secure()
param sqlAdminLogin string

@description('SQL administrator password')
@secure()
param sqlAdminPassword string

@description('Azure AD tenant ID for authentication')
param tenantId string = subscription().tenantId

// ── Variables ──────────────────────────────────────────────────────────
var suffix = '${baseName}-${environment}'
var appServicePlanName = 'asp-${suffix}'
var keyVaultName = 'kv-${suffix}'
var appInsightsName = 'ai-${suffix}'
var logAnalyticsName = 'la-${suffix}'
var sqlServerName = 'sql-${suffix}'

var services = [
  'surveydata'
  'address'
  'hereditament'
  'marketdata'
]

// ── Log Analytics Workspace ───────────────────────────────────────────
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// ── Application Insights ──────────────────────────────────────────────
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

// ── App Service Plan ──────────────────────────────────────────────────
resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: environment == 'prod' ? 'P1v3' : 'B1'
    tier: environment == 'prod' ? 'PremiumV3' : 'Basic'
  }
  properties: {
    reserved: false // Windows
  }
}

// ── Key Vault ─────────────────────────────────────────────────────────
resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
  }
}

// ── SQL Server ────────────────────────────────────────────────────────
resource sqlServer 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }

  // Allow Azure services
  resource firewallRule 'firewallRules' = {
    name: 'AllowAllAzureIps'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

// ── SQL Databases (one per service) ────────────────────────────────────
resource sqlDatabases 'Microsoft.Sql/servers/databases@2024-05-01-preview' = [
  for service in services: {
    parent: sqlServer
    name: '${service}-db'
    location: location
    sku: {
      name: environment == 'prod' ? 'S1' : 'Basic'
      tier: environment == 'prod' ? 'Standard' : 'Basic'
    }
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
      maxSizeBytes: environment == 'prod' ? 268435456000 : 2147483648
    }
  }
]

// ── App Services (one per microservice) ────────────────────────────────
resource appServices 'Microsoft.Web/sites@2024-04-01' = [
  for service in services: {
    name: 'app-${service}-${suffix}'
    location: location
    identity: {
      type: 'SystemAssigned'
    }
    properties: {
      serverFarmId: appServicePlan.id
      httpsOnly: true
      siteConfig: {
        netFrameworkVersion: 'v10.0'
        alwaysOn: environment == 'prod'
        minTlsVersion: '1.2'
        appSettings: [
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: appInsights.properties.ConnectionString
          }
          {
            name: 'KeyVault__VaultUri'
            value: keyVault.properties.vaultUri
          }
        ]
        connectionStrings: [
          {
            name: 'ServiceDb'
            connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${service}-db;Authentication=Active Directory Managed Identity;Encrypt=True;'
            type: 'SQLAzure'
          }
        ]
      }
    }
  }
]

// ── Key Vault Access for App Services (RBAC) ──────────────────────────
resource kvRoleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for (service, index) in services: {
    name: guid(keyVault.id, appServices[index].id, '4633458b-17de-408a-b874-0445c86b69e6')
    scope: keyVault
    properties: {
      roleDefinitionId: subscriptionResourceId(
        'Microsoft.Authorization/roleDefinitions',
        '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User
      )
      principalId: appServices[index].identity.principalId
      principalType: 'ServicePrincipal'
    }
  }
]

// ── Outputs ───────────────────────────────────────────────────────────
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output keyVaultUri string = keyVault.properties.vaultUri
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output appServiceNames array = [for service in services: 'app-${service}-${suffix}']
