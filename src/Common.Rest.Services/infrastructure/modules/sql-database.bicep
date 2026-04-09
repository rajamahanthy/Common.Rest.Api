@description('Location for the SQL resources')
param location string

@description('SQL Server name')
param sqlServerName string

@description('SQL Server admin username')
param sqlAdminUsername string

@description('Name of the Survey database')
param surveyDbName string

@description('Name of the Address database')
param addressDbName string

@description('SQL Database edition (Standard, Premium)')
param databaseEdition string = 'Standard'

@description('SQL Database service tier (S0, S1, S2, P1, P2, P4, P6)')
param databaseTier string = 'S1'

@description('Key Vault name for storing connection strings')
param keyVaultName string

@description('Principal ID of the Managed Identity for SQL access')
param managedIdentityPrincipalId string

@description('Client ID of the Managed Identity')
param managedIdentityClientId string

@description('Log Analytics Workspace ID for diagnostics')
param logAnalyticsWorkspaceId string

@description('Tags for the resources')
param tags object = {}

// Generate secure password for SQL Admin
@secure()
param sqlAdminPassword string

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  kind: 'v12.0'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {}
  }
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'Application'
      login: 'AppServiceManagedIdentity'
      sid: managedIdentityPrincipalId
      tenantId: subscription().tenantId
    }
  }
  tags: tags
}

// Firewall rule: Allow Azure Services (App Service)
resource sqlFirewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Survey Database
resource surveyDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: surveyDbName
  location: location
  sku: {
    name: databaseTier
    tier: databaseEdition
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824 // 1 GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
  }
  tags: tags
}

// Address Database
resource addressDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: addressDbName
  location: location
  sku: {
    name: databaseTier
    tier: databaseEdition
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824 // 1 GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
  }
  tags: tags
}

// Diagnostic Settings for Survey Database
resource surveyDbDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: surveyDatabase
  name: '${surveyDbName}-diagnostics'
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'SQLInsights'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'AutomaticTuning'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'QueryStoreRuntimeStatistics'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'QueryStoreWaitStatistics'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'Errors'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
    metrics: [
      {
        category: 'Basic'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
  }
}

// Diagnostic Settings for Address Database
resource addressDbDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: addressDatabase
  name: '${addressDbName}-diagnostics'
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'SQLInsights'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'AutomaticTuning'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
      {
        category: 'Errors'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
    metrics: [
      {
        category: 'Basic'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
  }
}

output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output surveyDatabaseName string = surveyDatabase.name
output addressDatabaseName string = addressDatabase.name
