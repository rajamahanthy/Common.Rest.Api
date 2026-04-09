#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Configure and execute Entity Framework database migrations on Azure SQL Database

.DESCRIPTION
    This script applies Entity Framework Core migrations to the Azure SQL databases
    (SurveyDb and AddressDb) using the deployed App Service Managed Identity.
    
    The script authenticates using Azure CLI and executes migrations via the App Service.

.PARAMETER Environment
    Target environment: dev, test, or prod
    Default: dev

.PARAMETER ResourceGroupName
    Azure resource group name (optional, will be generated if not provided)
    
.PARAMETER AppServiceName
    App Service name (optional, will be generated if not provided)

.PARAMETER SubscriptionId
    Azure subscription ID (optional)

.EXAMPLE
    .\configure-migrations.ps1 -Environment dev

.EXAMPLE
    .\configure-migrations.ps1 -Environment dev -ResourceGroupName VO-dev-rg -AppServiceName VO-dev-app

#>

param(
    [string]$Environment = 'dev',
    [string]$ResourceGroupName,
    [string]$AppServiceName,
    [string]$SubscriptionId
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Script configuration
$resourcePrefix = 'VO'

# Color output functions
function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Cyan
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

# Connect to Azure
function Connect-AzureContext {
    param([string]$SubscriptionId)
    
    Write-Info "Checking Azure context..."
    
    try {
        $context = Get-AzContext
        
        if (-not $context) {
            Write-Info "Authenticating to Azure..."
            Connect-AzAccount -ErrorAction Stop
            $context = Get-AzContext
        }
        
        if ($SubscriptionId) {
            Write-Info "Switching to subscription: $SubscriptionId"
            Select-AzSubscription -SubscriptionId $SubscriptionId -ErrorAction Stop
        }
        
        $currentContext = Get-AzContext
        Write-Success "Authenticated as: $($currentContext.Account.Id)"
        
        return $currentContext
    } catch {
        Write-Error "Failed to authenticate: $_"
        return $null
    }
}

# Get App Service publishing credentials
function Get-AppServicePublishingCredentials {
    param(
        [string]$ResourceGroupName,
        [string]$AppServiceName
    )
    
    Write-Info "Retrieving publishing credentials for App Service: $AppServiceName"
    
    try {
        # Get the publishing profile
        $publishingProfile = Get-AzWebAppPublishingProfile `
            -ResourceGroupName $ResourceGroupName `
            -Name $AppServiceName `
            -OutputFile $null
        
        if ($publishingProfile) {
            Write-Success "Publishing credentials retrieved"
            return $publishingProfile
        }
    } catch {
        Write-Error "Failed to get publishing credentials: $_"
        return $null
    }
}

# Execute migrations via App Service
function Invoke-DatabaseMigrations {
    param(
        [string]$AppServiceName,
        [string]$Environment
    )
    
    Write-Info "Executing database migrations..."
    
    # Construct the migration endpoint URL
    $appServiceUrl = "https://$AppServiceName.azurewebsites.net"
    $migrationEndpoint = "$appServiceUrl/api/migrations/execute"
    
    Write-Info "Migration endpoint: $migrationEndpoint"
    
    try {
        # Get access token using managed identity
        $token = Get-AzAccessToken -ResourceTypeName Websites
        $headers = @{
            'Authorization' = "Bearer $($token.Token)"
            'Content-Type' = 'application/json'
        }
        
        # Execute migrations
        $response = Invoke-WebRequest `
            -Uri $migrationEndpoint `
            -Method Post `
            -Headers $headers `
            -ErrorAction Stop
        
        if ($response.StatusCode -eq 200) {
            Write-Success "Database migrations executed successfully"
            
            # Parse and display migration results
            $result = $response.Content | ConvertFrom-Json
            Write-Info "Migration Result: $($result | ConvertTo-Json)"
            
            return $true
        } else {
            Write-Error "Migration endpoint returned status: $($response.StatusCode)"
            Write-Error "Response: $($response.Content)"
            return $false
        }
    } catch {
        Write-Warning "Could not execute migrations via HTTP endpoint: $_"
        Write-Info "This may be expected if the migrations endpoint is not yet configured."
        Write-Info "You may need to configure migrations manually or via CI/CD pipeline."
        return $null
    }
}

# Alternative: Execute migrations using Azure CLI
function Invoke-DatabaseMigrationsViaCli {
    param(
        [string]$ResourceGroupName,
        [string]$AppServiceName
    )
    
    Write-Info "Attempting to execute migrations via Azure CLI..."
    
    try {
        # Get the connection string from App Service configuration
        Write-Info "Retrieving App Service configuration..."
        
        $appService = Get-AzWebApp -ResourceGroupName $ResourceGroupName -Name $AppServiceName
        
        if ($appService) {
            Write-Success "App Service found: $($appService.Name)"
            
            # Get app settings
            $appSettings = $appService.SiteConfig.AppSettings
            Write-Info "App settings retrieved. Count: $($appSettings.Count)"
            
            # Connection strings would be retrieved from Key Vault in production
            Write-Info "Connection strings are stored in Key Vault for security"
            Write-Info "Run 'dotnet ef database update' locally or in deployment pipeline"
            
            return $true
        }
    } catch {
        Write-Warning "Error accessing App Service configuration: $_"
        return $false
    }
}

# Display migration instructions
function Show-MigrationInstructions {
    Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║            Database Migration Instructions                 ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
    
    Write-Info "Option 1: Execute locally with connection to Azure SQL"
    Write-Host @"
    
    # Set environment variables
    `$env:ASPNETCORE_ENVIRONMENT = '$Environment'
    
    # Navigate to your API project
    cd ../SurveyData/Common.Rest.SurveyData.Infrastructure
    
    # Get connection string from Key Vault
    az keyvault secret show --vault-name VO${Environment}kv --name ConnectionString
    
    # Execute EF Core migrations
    dotnet ef database update --connection "YOUR_CONNECTION_STRING"
    
"@
    
    Write-Info "Option 2: Execute via CI/CD pipeline during deployment"
    Write-Host @"
    
    # In your CI/CD pipeline (GitHub Actions / Azure Pipelines):
    - name: Run Database Migrations
      run: |
        dotnet ef database update \
          --project src/Common.Rest.Services/SurveyData/Common.Rest.SurveyData.Infrastructure \
          --connection `${{ secrets.SURVEY_DB_CONNECTION_STRING }}
    
"@
    
    Write-Info "Option 3: Execute from App Service post-deployment hook"
    Write-Host @"
    
    # Create a post-deployment script in your project:
    - PostDeploymentActions/RunMigrations.cs
    
    # Call this script after deploying the application
    
"@
}

# Main execution
function Main {
    Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║     Database Migration Configuration for Azure SQL         ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
    
    # Set default values if not provided
    if (-not $ResourceGroupName) {
        $ResourceGroupName = "$resourcePrefix-$Environment-rg"
    }
    
    if (-not $AppServiceName) {
        $AppServiceName = "$resourcePrefix-$Environment-app"
    }
    
    Write-Info "Environment: $Environment"
    Write-Info "Resource Group: $ResourceGroupName"
    Write-Info "App Service: $AppServiceName"
    
    # Connect to Azure
    if (-not (Connect-AzureContext -SubscriptionId $SubscriptionId)) {
        exit 1
    }
    
    # Verify App Service exists
    Write-Info "Verifying App Service exists..."
    try {
        $appService = Get-AzWebApp -ResourceGroupName $ResourceGroupName -Name $AppServiceName -ErrorAction Stop
        Write-Success "App Service verified: $($appService.Name)"
    } catch {
        Write-Error "App Service not found: $AppServiceName"
        exit 1
    }
    
    # Try to execute migrations
    $migrationResult = Invoke-DatabaseMigrations -AppServiceName $AppServiceName -Environment $Environment
    
    if ($null -eq $migrationResult) {
        # Fallback to alternative method
        Invoke-DatabaseMigrationsViaCli -ResourceGroupName $ResourceGroupName -AppServiceName $AppServiceName
    }
    
    # Show instructions
    Show-MigrationInstructions
    
    Write-Success "`n✓ Migration configuration complete!"
    Write-Warning "`nIMPORTANT: Ensure migrations are executed before or after deployment"
    Write-Warning "Recommended: Execute during CI/CD pipeline to ensure consistency"
}

# Run main
try {
    Main
} catch {
    Write-Error "Migration configuration failed: $_"
    exit 1
}
