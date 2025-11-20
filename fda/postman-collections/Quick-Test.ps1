# Quick-Test.ps1
# Quick test runner for individual workflow collections
# Use this for rapid testing of specific workflows during development

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("users", "restaurants", "operators", "all")]
    [string]$Workflow = "all",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("Local", "Production")]
    [string]$Environment = "Local",
    
    [Parameter(Mandatory=$false)]
    [switch]$NoReports
)

$ErrorActionPreference = "Continue"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$EnvironmentFile = Join-Path $ScriptDir "Capstone-$Environment-Environment.postman_environment.json"

# Colors
function Write-Title { param([string]$Msg) Write-Host "`n=== $Msg ===" -ForegroundColor Cyan }
function Write-OK { param([string]$Msg) Write-Host "✓ $Msg" -ForegroundColor Green }
function Write-Fail { param([string]$Msg) Write-Host "✗ $Msg" -ForegroundColor Red }

# Check Newman
if (-not (Get-Command newman -ErrorAction SilentlyContinue)) {
    Write-Fail "Newman not installed. Run: npm install -g newman"
    exit 1
}

# Check environment file
if (-not (Test-Path $EnvironmentFile)) {
    Write-Fail "Environment file not found: $EnvironmentFile"
    exit 1
}

$startTime = Get-Date
$reportArgs = @()

if (-not $NoReports) {
    $reportsDir = Join-Path $ScriptDir "test-results"
    if (-not (Test-Path $reportsDir)) { New-Item -Path $reportsDir -ItemType Directory -Force | Out-Null }
    $reportArgs = @("--reporters", "cli,htmlextra", "--reporter-htmlextra-export")
}

# Test functions
function Test-Users {
    Write-Title "User Registration Tests"
    $collection = Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json"
    
    Write-Host "Testing customers..." -ForegroundColor Yellow
    $args = @("run", $collection, "-e", $EnvironmentFile, "-d", (Join-Path $ScriptDir "user-registration\data\customer-registration.json"))
    if (-not $NoReports) { $args += $reportArgs + (Join-Path $reportsDir "quick-users.html") }
    & newman @args
    
    if ($LASTEXITCODE -eq 0) { Write-OK "User registration tests passed" } else { Write-Fail "Tests failed" }
}

function Test-Restaurants {
    Write-Title "Restaurant Owner Workflows"
    $collection = Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json"
    
    $args = @("run", $collection, "-e", $EnvironmentFile, "--folder", "Restaurant Registration")
    if (-not $NoReports) { $args += $reportArgs + (Join-Path $reportsDir "quick-restaurants.html") }
    & newman @args
    
    if ($LASTEXITCODE -eq 0) { Write-OK "Restaurant tests passed" } else { Write-Fail "Tests failed" }
}

function Test-Operators {
    Write-Title "Operator Service Workflows"
    $collection = Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json"
    
    $args = @("run", $collection, "-e", $EnvironmentFile, "--folder", "Order Management - View Orders")
    if (-not $NoReports) { $args += $reportArgs + (Join-Path $reportsDir "quick-operators.html") }
    & newman @args
    
    if ($LASTEXITCODE -eq 0) { Write-OK "Operator tests passed" } else { Write-Fail "Tests failed" }
}

# Run selected workflow
switch ($Workflow) {
    "users" { Test-Users }
    "restaurants" { Test-Restaurants }
    "operators" { Test-Operators }
    "all" {
        Test-Users
        Test-Restaurants
        Test-Operators
    }
}

$duration = ((Get-Date) - $startTime).TotalSeconds
Write-Host "`nCompleted in $([math]::Round($duration, 2)) seconds" -ForegroundColor Cyan
