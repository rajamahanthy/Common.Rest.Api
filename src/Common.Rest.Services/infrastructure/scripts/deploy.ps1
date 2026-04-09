#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy Azure infrastructure using Bicep for Common.Rest.Api

.DESCRIPTION
    This script deploys the production-ready Azure infrastructure for the Common.Rest.Api solution
    including App Service, SQL Database, Key Vault, and monitoring resources.

.PARAMETER Environment
    Target environment: dev, test, or prod
    Default: dev

.PARAMETER ResourceGroupName
    Name of the Azure resource group (optional, will be generated if not provided)

.PARAMETER Location
    Azure region for deployment
    Default: uksouth

.PARAMETER ParameterFilePath
    Path to the parameter file (JSON)
    Default: ./parameters.dev.json

.PARAMETER TemplateFilePath
    Path to the Bicep template file
    Default: ./main.bicep

.PARAMETER SubscriptionId
    Azure subscription ID (optional, uses current context if not provided)

.EXAMPLE
    .\deploy.ps1 -Environment dev -Location uksouth

.EXAMPLE
    .\deploy.ps1 -Environment dev -Location eastus -ParameterFilePath ./parameters.dev.json

#>

param(
    [string]$Environment = 'dev',
    [string]$ResourceGroupName,
    [string]$Location = 'uksouth',
    [string]$ParameterFilePath = './parameters.dev.json',
    [string]$TemplateFilePath = './main.bicep',
    [string]$SubscriptionId
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Script configuration
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
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

# Validate Bicep CLI is installed
function Test-BicepCliInstalled {
    try {
        $result = & bicep version 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Bicep CLI found: $result"
            return $true
        }
    } catch {
        Write-Warning "Bicep CLI not found. Please install it with: winget install microsoft.bicep"
        return $false
    }
}

# Validate parameter file exists
function Test-ParameterFile {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        Write-Error "Parameter file not found: $FilePath"
        return $false
    }
    
    Write-Success "Parameter file found: $FilePath"
    return $true
}

# Validate template file exists
function Test-TemplateFile {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        Write-Error "Template file not found: $FilePath"
        return $false
    }
    
    Write-Success "Template file found: $FilePath"
    return $true
}

# Build Bicep to ARM template
function Invoke-BicepBuild {
    param([string]$TemplatePath)
    
    Write-Info "Building Bicep template: $TemplatePath"
    
    try {
        $outputPath = $TemplatePath -replace '\.bicep$', '.json'
        & bicep build $TemplatePath --outfile $outputPath
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Bicep template built successfully: $outputPath"
            return $outputPath
        } else {
            Write-Error "Failed to build Bicep template"
            return $null
        }
    } catch {
        Write-Error "Error building Bicep: $_"
        return $null
    }
}

# Authenticate to Azure
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
        Write-Success "Authenticated as: $($currentContext.Account.Id) (Subscription: $($currentContext.Subscription.Id))"
        
        return $currentContext
    } catch {
        Write-Error "Failed to authenticate: $_"
        return $null
    }
}

# Create resource group if it doesn't exist
function New-ResourceGroupIfNotExists {
    param([string]$ResourceGroupName, [string]$Location)
    
    Write-Info "Checking if resource group exists: $ResourceGroupName"
    
    try {
        $rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
        
        if ($rg) {
            Write-Success "Resource group already exists: $ResourceGroupName"
            return $rg
        } else {
            Write-Info "Creating resource group: $ResourceGroupName in $Location"
            $rg = New-AzResourceGroup -Name $ResourceGroupName -Location $Location -Force -ErrorAction Stop
            Write-Success "Resource group created: $ResourceGroupName"
            return $rg
        }
    } catch {
        Write-Error "Failed to create resource group: $_"
        return $null
    }
}

# Deploy ARM template
function Invoke-ArmDeployment {
    param(
        [string]$ResourceGroupName,
        [string]$TemplatePath,
        [string]$ParameterPath
    )
    
    Write-Info "Starting Bicep deployment to resource group: $ResourceGroupName"
    
    try {
        $deployment = New-AzResourceGroupDeployment `
            -ResourceGroupName $ResourceGroupName `
            -TemplateFile $TemplatePath `
            -TemplateParameterFile $ParameterPath `
            -Mode Incremental `
            -ErrorAction Stop
        
        if ($deployment.ProvisioningState -eq 'Succeeded') {
            Write-Success "Deployment succeeded!"
            return $deployment
        } else {
            Write-Error "Deployment failed with state: $($deployment.ProvisioningState)"
            return $null
        }
    } catch {
        Write-Error "Deployment failed: $_"
        return $null
    }
}

# Display deployment outputs
function Show-DeploymentOutputs {
    param($Deployment)
    
    Write-Info "`n=== Deployment Outputs ==="
    
    if ($Deployment.Outputs) {
        foreach ($output in $Deployment.Outputs.GetEnumerator()) {
            Write-Host "  $($output.Key): $($output.Value.Value)" -ForegroundColor Cyan
        }
    }
}

# Main execution
function Main {
    Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║        Common.Rest.Api - Azure Infrastructure Deploy        ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
    
    # Validate prerequisites
    Write-Info "Validating prerequisites..."
    
    if (-not (Test-BicepCliInstalled)) {
        Write-Error "Bicep CLI is required. Please install it and try again."
        exit 1
    }
    
    if (-not (Test-ParameterFile $ParameterFilePath)) {
        exit 1
    }
    
    if (-not (Test-TemplateFile $TemplateFilePath)) {
        exit 1
    }
    
    # Connect to Azure
    if (-not (Connect-AzureContext -SubscriptionId $SubscriptionId)) {
        exit 1
    }
    
    # Set resource group name if not provided
    if (-not $ResourceGroupName) {
        $ResourceGroupName = "$resourcePrefix-$Environment-rg"
    }
    
    Write-Info "Environment: $Environment"
    Write-Info "Resource Group: $ResourceGroupName"
    Write-Info "Location: $Location"
    
    # Create resource group
    $rg = New-ResourceGroupIfNotExists -ResourceGroupName $ResourceGroupName -Location $Location
    if (-not $rg) {
        exit 1
    }
    
    # Build Bicep template
    $armTemplatePath = Invoke-BicepBuild -TemplatePath $TemplateFilePath
    if (-not $armTemplatePath) {
        exit 1
    }
    
    # Deploy
    $deployment = Invoke-ArmDeployment `
        -ResourceGroupName $ResourceGroupName `
        -TemplatePath $armTemplatePath `
        -ParameterPath $ParameterFilePath
    
    if (-not $deployment) {
        exit 1
    }
    
    # Show outputs
    Show-DeploymentOutputs -Deployment $deployment
    
    Write-Success "`n✓ Deployment completed successfully!"
    Write-Info "Resource Group: $ResourceGroupName"
    Write-Info "Location: $Location"
    Write-Info "`nNext steps:"
    Write-Info "1. Deploy your .NET application to the App Service"
    Write-Info "2. Run database migrations: .\configure-migrations.ps1 -Environment $Environment"
    Write-Info "3. Configure custom domain and SSL certificate"
    Write-Info "4. Set up CI/CD pipeline"
}

# Run main
try {
    Main
} catch {
    Write-Error "Deployment failed: $_"
    exit 1
}
