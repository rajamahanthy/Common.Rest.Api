<#
.SYNOPSIS
Creates module folder structure (no .sln) using controllers-based WebAPI,
then overwrites csproj files to match your reference templates (net10.0 + deps + refs).
Also creates unit/integration test projects with useful packages (no FluentAssertions).
#>

[CmdletBinding()]
param(
  [Parameter(Mandatory)]
  [string] $Name,

  [string] $Prefix = "Common.Rest",

  [string] $OutputPath = (Get-Location).Path,

  # Keep as per your repo layout; you can override on command line
  [string] $SharedProjectRef = "..\..\..\Common.Rest.Services\Common.Rest.Shared\Common.Rest.Shared.csproj",

  [bool] $IncludeTests = $true
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Assert-DotNet {
  try {
    $v = & dotnet --version
    Write-Host "dotnet SDK detected: $v" -ForegroundColor Green
  } catch {
    throw "dotnet SDK not found. Install .NET SDK first: https://dotnet.microsoft.com/download"
  }
}

function New-Dir([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path)) {
    New-Item -ItemType Directory -Path $Path | Out-Null
  }
}

function Exec([string]$Command) {
  Write-Host ">> $Command" -ForegroundColor DarkCyan
  & powershell -NoProfile -Command $Command
  if ($LASTEXITCODE -ne 0) {
    throw "Command failed with exit code $LASTEXITCODE : $Command"
  }
}

function Write-TextFile([string]$Path, [string]$Content) {
  $dir = Split-Path -Parent $Path
  if (-not (Test-Path -LiteralPath $dir)) { New-Dir $dir }
  Set-Content -Path $Path -Value $Content -Encoding UTF8
}

function Apply-Template([string]$Template, [hashtable]$Vars) {
  $out = $Template
  foreach ($k in $Vars.Keys) {
    $out = $out -replace [regex]::Escape("{{${k}}}"), [string]$Vars[$k]
  }
  return $out
}

Assert-DotNet

$ModuleName     = $Name.Trim()
$RootNamespace  = "$Prefix.$ModuleName"

# repo root folder = <OutputPath>\<Name>
$RepoRoot = Join-Path $OutputPath $ModuleName
New-Dir $RepoRoot

Push-Location $RepoRoot
try {
  # --- Paths matching your example ---
  $ApiDir    = Join-Path $RepoRoot "$ModuleName\$RootNamespace.Api"
  $AppDir    = Join-Path $RepoRoot "$ModuleName\$RootNamespace.Application"
  $DomainDir = Join-Path $RepoRoot "$ModuleName\$RootNamespace.Domain"
  $InfraDir  = Join-Path $RepoRoot "$ModuleName\$RootNamespace.Infrastructure"

  $TestsRoot          = Join-Path $RepoRoot "$ModuleName\Tests"
  $IntegrationTestDir = Join-Path $RepoRoot "$ModuleName\Tests\IntegrationTests\$RootNamespace.Integration.Tests"
  $ApiUnitTestDir     = Join-Path $RepoRoot "$ModuleName\Tests\UnitTests\$RootNamespace.Api.Tests"
  $AppUnitTestDir     = Join-Path $RepoRoot "$ModuleName\Tests\UnitTests\$RootNamespace.Application.Tests"
  $DomainUnitTestDir  = Join-Path $RepoRoot "$ModuleName\Tests\UnitTests\$RootNamespace.Domain.Tests"
  $InfraUnitTestDir   = Join-Path $RepoRoot "$ModuleName\Tests\UnitTests\$RootNamespace.Infrastructure.Tests"

  @($ApiDir,$AppDir,$DomainDir,$InfraDir) | ForEach-Object { New-Dir $_ }
  if ($IncludeTests) {
    @($TestsRoot,$IntegrationTestDir,$ApiUnitTestDir,$AppUnitTestDir,$DomainUnitTestDir,$InfraUnitTestDir) | ForEach-Object { New-Dir $_ }
  }

  # --- Scaffold projects (templates may not support -f net10.0 yet) ---
  Exec "dotnet new webapi -n `"$RootNamespace.Api`" -o `"$ApiDir`" --no-https"
  Exec "dotnet new classlib -n `"$RootNamespace.Application`" -o `"$AppDir`""
  Exec "dotnet new classlib -n `"$RootNamespace.Domain`" -o `"$DomainDir`""
  Exec "dotnet new classlib -n `"$RootNamespace.Infrastructure`" -o `"$InfraDir`""

  if ($IncludeTests) {
    Exec "dotnet new xunit -n `"$RootNamespace.Integration.Tests`" -o `"$IntegrationTestDir`""
    Exec "dotnet new xunit -n `"$RootNamespace.Api.Tests`" -o `"$ApiUnitTestDir`""
    Exec "dotnet new xunit -n `"$RootNamespace.Application.Tests`" -o `"$AppUnitTestDir`""
    Exec "dotnet new xunit -n `"$RootNamespace.Domain.Tests`" -o `"$DomainUnitTestDir`""
    Exec "dotnet new xunit -n `"$RootNamespace.Infrastructure.Tests`" -o `"$InfraUnitTestDir`""
  }

  # --- csproj paths ---
  $ApiProj    = Join-Path $ApiDir    "$RootNamespace.Api.csproj"
  $AppProj    = Join-Path $AppDir    "$RootNamespace.Application.csproj"
  $DomainProj = Join-Path $DomainDir "$RootNamespace.Domain.csproj"
  $InfraProj  = Join-Path $InfraDir  "$RootNamespace.Infrastructure.csproj"

  # --- Templates: single-quoted here-strings to avoid PowerShell parsing issues ---
  $apiTemplate = @'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Asp.Versioning.Mvc" />
    <PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" />
    <PackageReference Include="Microsoft.Identity.Web" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="{{SharedProjectRef}}" />
    <ProjectReference Include="..\{{RootNamespace}}.Application\{{RootNamespace}}.Application.csproj" />
    <ProjectReference Include="..\{{RootNamespace}}.Infrastructure\{{RootNamespace}}.Infrastructure.csproj" />
  </ItemGroup>
</Project>
'@

  $appTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="{{SharedProjectRef}}" />
    <ProjectReference Include="..\{{RootNamespace}}.Domain\{{RootNamespace}}.Domain.csproj" />
  </ItemGroup>
</Project>
'@

  $domainTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="{{SharedProjectRef}}" />
  </ItemGroup>
</Project>
'@

  $infraTemplate = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="{{SharedProjectRef}}" />
    <ProjectReference Include="..\{{RootNamespace}}.Domain\{{RootNamespace}}.Domain.csproj" />
  </ItemGroup>
</Project>
'@

  $vars = @{
    RootNamespace    = $RootNamespace
    SharedProjectRef = $SharedProjectRef
  }

  Write-TextFile -Path $ApiProj    -Content (Apply-Template $apiTemplate $vars)
  Write-TextFile -Path $AppProj    -Content (Apply-Template $appTemplate $vars)
  Write-TextFile -Path $DomainProj -Content (Apply-Template $domainTemplate $vars)
  Write-TextFile -Path $InfraProj  -Content (Apply-Template $infraTemplate $vars)

  # --- Tests: overwrite csproj to net10.0 + packages (no FluentAssertions) ---
  if ($IncludeTests) {
    $IntTestProj = Join-Path $IntegrationTestDir "$RootNamespace.Integration.Tests.csproj"
    $ApiTestProj = Join-Path $ApiUnitTestDir     "$RootNamespace.Api.Tests.csproj"
    $AppTestProj = Join-Path $AppUnitTestDir     "$RootNamespace.Application.Tests.csproj"
    $DomTestProj = Join-Path $DomainUnitTestDir  "$RootNamespace.Domain.Tests.csproj"
    $InfTestProj = Join-Path $InfraUnitTestDir   "$RootNamespace.Infrastructure.Tests.csproj"

    function New-TestCsproj([string[]]$ProjectRefs, [string[]]$Packages) {
      $refsXml = ($ProjectRefs | ForEach-Object { "    <ProjectReference Include=""$_"" />" }) -join "`n"
      $pkgsXml = ($Packages | ForEach-Object { "    <PackageReference Include=""$_"" />" }) -join "`n"
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
$pkgsXml
  </ItemGroup>

  <ItemGroup>
$refsXml
  </ItemGroup>
</Project>
"@
    }

    $unitPkgs = @(
      "Microsoft.NET.Test.Sdk",
      "xunit",
      "xunit.runner.visualstudio",
      "coverlet.collector",
      "NSubstitute",
      "Shouldly",
      "AutoFixture",
      "AutoFixture.Xunit2"
    )

    $integrationPkgs = @(
      "Microsoft.NET.Test.Sdk",
      "xunit",
      "xunit.runner.visualstudio",
      "coverlet.collector",
      "Shouldly",
      "NSubstitute",
      "Microsoft.AspNetCore.Mvc.Testing"
    )

    Write-TextFile -Path $ApiTestProj -Content (New-TestCsproj `
      -ProjectRefs @("..\..\..\$RootNamespace.Api\$RootNamespace.Api.csproj") `
      -Packages $unitPkgs)

    Write-TextFile -Path $AppTestProj -Content (New-TestCsproj `
      -ProjectRefs @("..\..\..\$RootNamespace.Application\$RootNamespace.Application.csproj") `
      -Packages $unitPkgs)

    Write-TextFile -Path $DomTestProj -Content (New-TestCsproj `
      -ProjectRefs @("..\..\..\$RootNamespace.Domain\$RootNamespace.Domain.csproj") `
      -Packages $unitPkgs)

    Write-TextFile -Path $InfTestProj -Content (New-TestCsproj `
      -ProjectRefs @("..\..\..\$RootNamespace.Infrastructure\$RootNamespace.Infrastructure.csproj") `
      -Packages $unitPkgs)

    Write-TextFile -Path $IntTestProj -Content (New-TestCsproj `
      -ProjectRefs @(
        "..\..\..\$RootNamespace.Api\$RootNamespace.Api.csproj",
        "..\..\..\$RootNamespace.Infrastructure\$RootNamespace.Infrastructure.csproj"
      ) `
      -Packages $integrationPkgs)
  }

  # Restore (quick validation)
  Exec "dotnet restore `"$ApiProj`""
  Exec "dotnet restore `"$AppProj`""
  Exec "dotnet restore `"$DomainProj`""
  Exec "dotnet restore `"$InfraProj`""

  if ($IncludeTests) {
    Exec "dotnet restore `"$TestsRoot`""
  }

  Write-Host ""
  Write-Host "✅ Done!" -ForegroundColor Green
  Write-Host "Module created at: $RepoRoot\$ModuleName" -ForegroundColor Green
  Write-Host "TargetFramework set to net10.0 in all generated projects." -ForegroundColor Green
  Write-Host "No solution (.sln) created." -ForegroundColor Green

} finally {
  Pop-Location
}