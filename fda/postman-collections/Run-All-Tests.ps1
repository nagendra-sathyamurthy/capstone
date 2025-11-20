# Run-All-Tests.ps1
# Comprehensive test script for all Food Delivery Application workflow collections
# This script runs all Postman collections with their respective data files and generates detailed reports

<#
.SYNOPSIS
    Runs all Postman workflow collections for the Food Delivery Application
    
.DESCRIPTION
    This script executes Newman test runs for:
    - User Registration (43 accounts across 9 roles)
    - Operator Service Workflows (order management, inventory, packaging)
    - Restaurant Owner Workflows (restaurant and menu management)
    
    Generates HTML reports for each workflow and a summary report at the end.
    
.PARAMETER Environment
    Environment to test against (Local or Production)
    Default: Local
    
.PARAMETER GenerateReports
    Generate HTML reports using newman-reporter-htmlextra
    Default: $true
    
.PARAMETER Verbose
    Show detailed test output
    Default: $false
    
.EXAMPLE
    .\Run-All-Tests.ps1
    Runs all tests against local environment with HTML reports
    
.EXAMPLE
    .\Run-All-Tests.ps1 -Environment Production -Verbose
    Runs all tests against production with verbose output
    
.EXAMPLE
    .\Run-All-Tests.ps1 -GenerateReports $false
    Runs tests without generating HTML reports (faster execution)
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Local", "Production")]
    [string]$Environment = "Local",
    
    [Parameter(Mandatory=$false)]
    [bool]$GenerateReports = $true,
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

# Script configuration
$ErrorActionPreference = "Continue"
$WarningPreference = "Continue"

# Colors for console output
function Write-Header {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error-Message {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Yellow
}

function Write-Step {
    param([string]$Message)
    Write-Host "> $Message" -ForegroundColor White
}

# Initialize test tracking
$script:TotalTests = 0
$script:PassedTests = 0
$script:FailedTests = 0
$script:TotalDuration = 0
$script:TestResults = @()

# Paths
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$EnvironmentFile = Join-Path $ScriptDir "Capstone-$Environment-Environment.postman_environment.json"
$ReportsDir = Join-Path $ScriptDir "test-results"

# Ensure reports directory exists
if ($GenerateReports) {
    if (-not (Test-Path $ReportsDir)) {
        New-Item -Path $ReportsDir -ItemType Directory -Force | Out-Null
        Write-Info "Created test-results directory: $ReportsDir"
    }
}

# Check prerequisites
Write-Header "Prerequisites Check"

# Check if Newman is installed
Write-Step "Checking Newman installation..."
try {
    $newmanVersion = newman --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Newman is installed (version: $newmanVersion)"
    } else {
        throw "Newman not found"
    }
} catch {
    Write-Error-Message "Newman is not installed. Installing Newman..."
    npm install -g newman
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Message "Failed to install Newman. Please install manually: npm install -g newman"
        exit 1
    }
}

# Check if newman-reporter-htmlextra is installed (if reports requested)
if ($GenerateReports) {
    Write-Step "Checking newman-reporter-htmlextra installation..."
    try {
        npm list -g newman-reporter-htmlextra 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Success "newman-reporter-htmlextra is installed"
        } else {
            throw "newman-reporter-htmlextra not found"
        }
    } catch {
        Write-Info "Installing newman-reporter-htmlextra..."
        npm install -g newman-reporter-htmlextra
        if ($LASTEXITCODE -ne 0) {
            Write-Error-Message "Failed to install newman-reporter-htmlextra"
            $GenerateReports = $false
        }
    }
}

# Check if environment file exists
Write-Step "Checking environment file..."
if (-not (Test-Path $EnvironmentFile)) {
    Write-Error-Message "Environment file not found: $EnvironmentFile"
    Write-Info "Available environments:"
    Get-ChildItem -Path $ScriptDir -Filter "*.postman_environment.json" | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor Gray
    }
    exit 1
}
Write-Success "Environment file found: $Environment"

Write-Host ""

# Function to run a Newman test
function Invoke-NewmanTest {
    param(
        [string]$CollectionName,
        [string]$CollectionPath,
        [string]$DataFile = $null,
        [string]$Folder = $null,
        [string]$ReportName,
        [string]$Description
    )
    
    Write-Header $CollectionName
    Write-Info $Description
    Write-Step "Collection: $CollectionPath"
    if ($DataFile) {
        Write-Step "Data File: $DataFile"
    }
    if ($Folder) {
        Write-Step "Folder: $Folder"
    }
    Write-Host ""
    
    $startTime = Get-Date
    
    # Build Newman command
    $newmanArgs = @(
        "run", $CollectionPath,
        "-e", $EnvironmentFile,
        "--delay-request", "500",
        "--timeout-request", "10000"
    )
    
    if ($DataFile -and (Test-Path $DataFile)) {
        $newmanArgs += "-d"
        $newmanArgs += $DataFile
    }
    
    if ($Folder) {
        $newmanArgs += "--folder"
        $newmanArgs += $Folder
    }
    
    if ($GenerateReports) {
        $reportPath = Join-Path $ReportsDir "$ReportName.html"
        $newmanArgs += "--reporters"
        $newmanArgs += "cli,htmlextra"
        $newmanArgs += "--reporter-htmlextra-export"
        $newmanArgs += $reportPath
    }
    
    if ($Verbose) {
        $newmanArgs += "--verbose"
    }
    
    # Run Newman
    Write-Step "Running tests..."
    & newman @newmanArgs
    
    $exitCode = $LASTEXITCODE
    $endTime = Get-Date
    $duration = ($endTime - $startTime).TotalSeconds
    
    $script:TotalDuration += $duration
    
    # Record result
    $result = @{
        Name = $CollectionName
        Description = $Description
        Duration = $duration
        Success = ($exitCode -eq 0)
        Timestamp = $startTime
    }
    
    $script:TestResults += $result
    
    if ($exitCode -eq 0) {
        $script:PassedTests++
        Write-Success "Tests passed in $([math]::Round($duration, 2)) seconds"
        if ($GenerateReports) {
            Write-Info "Report: $reportPath"
        }
    } else {
        $script:FailedTests++
        Write-Error-Message "Tests failed (Exit code: $exitCode)"
    }
    
    $script:TotalTests++
    Write-Host ""
}

# Start test execution
$overallStartTime = Get-Date

Write-Header "Food Delivery Application - Comprehensive Test Suite"
Write-Info "Environment: $Environment"
Write-Info "Generate Reports: $GenerateReports"
Write-Info "Started: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Write-Host ""

# =============================================================================
# 1. USER REGISTRATION TESTS
# =============================================================================

Write-Header "PHASE 1: User Registration Tests"
Write-Info "Registering and authenticating all user roles (43 accounts)"

# 1.1 Register Customers (5 accounts)
Invoke-NewmanTest `
    -CollectionName "User Registration - Customers" `
    -CollectionPath (Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "user-registration\data\customer-registration.json") `
    -ReportName "01-user-registration-customers" `
    -Description "Register 5 customer accounts (Customer role)"

# 1.2 Register Restaurant Owners (8 accounts)
Invoke-NewmanTest `
    -CollectionName "User Registration - Restaurant Owners" `
    -CollectionPath (Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "user-registration\data\restaurant-owner-registration.json") `
    -ReportName "02-user-registration-owners" `
    -Description "Register 8 restaurant owner accounts (Biller role)"

# 1.3 Register Kitchen Workers (10 accounts)
Invoke-NewmanTest `
    -CollectionName "User Registration - Kitchen Workers" `
    -CollectionPath (Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "user-registration\data\kitchen-worker-registration.json") `
    -ReportName "03-user-registration-workers" `
    -Description "Register 10 kitchen worker accounts (Worker role)"

# 1.4 Register Delivery Agents (10 accounts)
Invoke-NewmanTest `
    -CollectionName "User Registration - Delivery Agents" `
    -CollectionPath (Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "user-registration\data\delivery-agent-registration.json") `
    -ReportName "04-user-registration-agents" `
    -Description "Register 10 delivery agent accounts (DeliveryAgent role)"

# 1.5 Register IT Admins (10 accounts)
Invoke-NewmanTest `
    -CollectionName "User Registration - IT Admins" `
    -CollectionPath (Join-Path $ScriptDir "user-registration\User-Registration-Flow.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "user-registration\data\it-admin-registration.json") `
    -ReportName "05-user-registration-admins" `
    -Description "Register 10 IT admin accounts (Developer, Tester, NetworkAdmin, DatabaseAdmin roles)"

# =============================================================================
# 2. RESTAURANT OWNER WORKFLOWS TESTS
# =============================================================================

Write-Header "PHASE 2: Restaurant Owner Workflows"
Write-Info "Testing restaurant registration, menu management, and business operations"

# 2.1 Restaurant Registration
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Restaurant Registration" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Restaurant Registration" `
    -ReportName "06-restaurant-registration" `
    -Description "Register restaurants with complete business information"

# 2.2 Menu Management - Add Items
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Add Menu Items" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Menu Management - Add Items" `
    -ReportName "07-menu-add-items" `
    -Description "Add menu items with pricing, categories, and dietary information"

# 2.3 Menu Management - Update Items
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Update Menu Items" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Menu Management - Update Items" `
    -ReportName "08-menu-update-items" `
    -Description "Update menu item availability, prices, and descriptions"

# 2.4 Restaurant Management - Status Updates
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Restaurant Status" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Restaurant Management - Status Updates" `
    -ReportName "09-restaurant-status" `
    -Description "Manage restaurant active/inactive status"

# 2.5 Business Hours Management
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Business Hours" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Restaurant Management - Hours Updates" `
    -ReportName "10-restaurant-hours" `
    -Description "Update restaurant operating hours and closed days"

# 2.6 Contact Information Updates
Invoke-NewmanTest `
    -CollectionName "Restaurant Owner - Contact Info" `
    -CollectionPath (Join-Path $ScriptDir "restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json") `
    -Folder "Restaurant Management - Contact Updates" `
    -ReportName "11-restaurant-contact" `
    -Description "Update restaurant contact information (phone, email, website)"

# =============================================================================
# 3. OPERATOR SERVICE WORKFLOWS TESTS
# =============================================================================

Write-Header "PHASE 3: Operator Service Workflows"
Write-Info "Testing order management, inventory control, packaging, and delivery handover"

# 3.1 Order Management - View Orders
Invoke-NewmanTest `
    -CollectionName "Operator - View Orders" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Order Management - View Orders" `
    -ReportName "12-operator-view-orders" `
    -Description "View pending, accepted, and completed orders"

# 3.2 Order Management - Accept Orders
Invoke-NewmanTest `
    -CollectionName "Operator - Accept Orders" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Order Management - Accept Orders" `
    -ReportName "13-operator-accept-orders" `
    -Description "Accept customer orders and start preparation"

# 3.3 Menu Management - Availability
Invoke-NewmanTest `
    -CollectionName "Operator - Menu Availability" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Menu Management - Update Availability" `
    -ReportName "14-operator-menu-availability" `
    -Description "Update menu item availability based on inventory"

# 3.4 Inventory Management
Invoke-NewmanTest `
    -CollectionName "Operator - Inventory Management" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Inventory Management" `
    -ReportName "15-operator-inventory" `
    -Description "Check inventory levels and low-stock alerts"

# 3.5 Packaging Management
Invoke-NewmanTest `
    -CollectionName "Operator - Packaging Management" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Packaging Management" `
    -ReportName "16-operator-packaging" `
    -Description "Update packaging details for orders (fragile, eco-friendly, special instructions)"

# 3.6 Order Handover - Generate OTP
Invoke-NewmanTest `
    -CollectionName "Operator - Generate Handover OTP" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Order Handover - Generate OTP" `
    -ReportName "17-operator-generate-otp" `
    -Description "Generate OTP for secure order handover to delivery agent"

# 3.7 Order Handover - Verify OTP
Invoke-NewmanTest `
    -CollectionName "Operator - Verify Handover OTP" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -Folder "Order Handover - Verify OTP" `
    -ReportName "18-operator-verify-otp" `
    -Description "Verify OTP and complete handover to delivery agent"

# 3.8 Complete Operator Workflow (End-to-End)
Invoke-NewmanTest `
    -CollectionName "Operator - Complete Workflow" `
    -CollectionPath (Join-Path $ScriptDir "operator-service-workflows\Operator-Service-Workflows.postman_collection.json") `
    -ReportName "19-operator-complete-workflow" `
    -Description "End-to-end operator workflow from order acceptance to handover"

# =============================================================================
# TEST SUMMARY
# =============================================================================

$overallEndTime = Get-Date
$overallDuration = ($overallEndTime - $overallStartTime).TotalSeconds

Write-Header "Test Execution Summary"

Write-Host "Execution Details:" -ForegroundColor Cyan
Write-Host "  Environment:      $Environment" -ForegroundColor White
Write-Host "  Started:          $(Get-Date -Date $overallStartTime -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "  Completed:        $(Get-Date -Date $overallEndTime -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "  Total Duration:   $([math]::Round($overallDuration, 2)) seconds ($([math]::Round($overallDuration/60, 2)) minutes)" -ForegroundColor White
Write-Host ""

Write-Host "Test Results:" -ForegroundColor Cyan
Write-Host "  Total Suites:     $script:TotalTests" -ForegroundColor White
Write-Host "  Passed:           $script:PassedTests" -ForegroundColor Green
Write-Host "  Failed:           $script:FailedTests" -ForegroundColor $(if ($script:FailedTests -gt 0) { "Red" } else { "Green" })
Write-Host "  Success Rate:     $([math]::Round(($script:PassedTests / $script:TotalTests) * 100, 2))%" -ForegroundColor $(if ($script:FailedTests -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

if ($GenerateReports) {
    Write-Host "Reports Generated:" -ForegroundColor Cyan
    Write-Host "  Location:         $ReportsDir" -ForegroundColor White
    Write-Host "  Total Reports:    $script:TotalTests HTML files" -ForegroundColor White
    Write-Host ""
}

# Detailed results table
Write-Host "Detailed Results:" -ForegroundColor Cyan
Write-Host ("-" * 100) -ForegroundColor Gray
Write-Host ("{0,-5} {1,-50} {2,-12} {3,-10}" -f "No.", "Test Suite", "Duration", "Status") -ForegroundColor White
Write-Host ("-" * 100) -ForegroundColor Gray

$index = 1
foreach ($result in $script:TestResults) {
    $status = if ($result.Success) { "✓ PASSED" } else { "✗ FAILED" }
    $statusColor = if ($result.Success) { "Green" } else { "Red" }
    $durationStr = "$([math]::Round($result.Duration, 2))s"
    
    Write-Host ("{0,-5} {1,-50} {2,-12} " -f "$index.", $result.Name, $durationStr) -NoNewline -ForegroundColor White
    Write-Host $status -ForegroundColor $statusColor
    $index++
}
Write-Host ("-" * 100) -ForegroundColor Gray
Write-Host ""

# Test phases summary
Write-Host "Test Phases Summary:" -ForegroundColor Cyan
Write-Host "  Phase 1: User Registration       - 5 test suites (43 user accounts)" -ForegroundColor White
Write-Host "  Phase 2: Restaurant Owner Flows  - 6 test suites (restaurant & menu management)" -ForegroundColor White
Write-Host "  Phase 3: Operator Service Flows  - 8 test suites (order & inventory management)" -ForegroundColor White
Write-Host ""

# Generate summary report file
if ($GenerateReports) {
    $summaryFile = Join-Path $ReportsDir "00-test-summary.txt"
    $summaryContent = @"
========================================
Food Delivery Application - Test Summary
========================================

Environment:      $Environment
Started:          $(Get-Date -Date $overallStartTime -Format 'yyyy-MM-dd HH:mm:ss')
Completed:        $(Get-Date -Date $overallEndTime -Format 'yyyy-MM-dd HH:mm:ss')
Total Duration:   $([math]::Round($overallDuration, 2)) seconds ($([math]::Round($overallDuration/60, 2)) minutes)

Test Results:
  Total Suites:   $script:TotalTests
  Passed:         $script:PassedTests
  Failed:         $script:FailedTests
  Success Rate:   $([math]::Round(($script:PassedTests / $script:TotalTests) * 100, 2))%

Test Phases:
  Phase 1: User Registration       - 5 suites (43 accounts)
  Phase 2: Restaurant Owner Flows  - 6 suites
  Phase 3: Operator Service Flows  - 8 suites

Detailed Results:
----------------------------------------
"@

    $index = 1
    foreach ($result in $script:TestResults) {
        $status = if ($result.Success) { "PASSED" } else { "FAILED" }
        $summaryContent += "`n$index. $($result.Name) - $([math]::Round($result.Duration, 2))s - $status"
        $index++
    }

    $summaryContent | Out-File -FilePath $summaryFile -Encoding UTF8
    Write-Info "Summary report saved: $summaryFile"
    Write-Host ""
}

# Exit with appropriate code
if ($script:FailedTests -eq 0) {
    Write-Success "All tests passed! ✓"
    exit 0
} else {
    Write-Error-Message "Some tests failed. Please review the reports for details."
    exit 1
}
