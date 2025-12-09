# Run all Newman tests for deployed services
# This script runs API tests against locally deployed Kubernetes services

Write-Host "=== Capstone Services - Newman Test Suite ===" -ForegroundColor Green
Write-Host ""

# Check if kubectl is available
Write-Host "Checking Kubernetes status..." -ForegroundColor Cyan
try {
    $context = kubectl config current-context 2>$null
    if ($context) {
        Write-Host "✓ Kubernetes context: $context" -ForegroundColor Green
    } else {
        Write-Host "✗ Kubernetes cluster not accessible" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ kubectl not found. Please install kubectl first." -ForegroundColor Red
    exit 1
}

# Check if services are running
Write-Host ""
Write-Host "Checking services in capstone-services namespace..." -ForegroundColor Cyan
$pods = kubectl get pods -n capstone-services --no-headers 2>$null

if (-not $pods) {
    Write-Host "✗ No services are running. Please deploy services first." -ForegroundColor Red
    Write-Host "  Run: .\deploy.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Services are running:" -ForegroundColor Green
kubectl get pods -n capstone-services --no-headers | ForEach-Object { 
    $podInfo = $_ -split '\s+'
    Write-Host "  - $($podInfo[0]): $($podInfo[2])" -ForegroundColor Gray 
}

# Create test results directory
$testResultsDir = "..\..\..\test-results"
New-Item -ItemType Directory -Force -Path $testResultsDir | Out-Null
Write-Host ""
Write-Host "Test results will be saved to: $testResultsDir" -ForegroundColor Cyan
Write-Host ""

# Define test collections
$collectionsPath = "..\..\..\postman-collections"
$testCollections = @(
    @{
        Name = "User Registration"
        Collection = "$collectionsPath\user-registration\User-Registration-Flow.postman_collection.json"
        Report = "user-registration-report"
        DataFile = "$collectionsPath\user-registration\data\customer-registration.json"
    },
    @{
        Name = "Restaurant Owner Workflows"
        Collection = "$collectionsPath\restaurant-owner-workflows\Restaurant-Owner-Workflows.postman_collection.json"
        Report = "restaurant-owner-report"
        DataFile = "$collectionsPath\restaurant-owner-workflows\data\restaurants-data.json"
    },
    @{
        Name = "Operator Service Workflows"
        Collection = "$collectionsPath\operator-service-workflows\Operator-Service-Workflows.postman_collection.json"
        Report = "operator-service-report"
        DataFile = "$collectionsPath\operator-service-workflows\data\operators-data.json"
    },
    @{
        Name = "Customer Workflows"
        Collection = "$collectionsPath\customer-workflows\Customer-Workflows.postman_collection.json"
        Report = "customer-workflows-report"
        DataFile = "$collectionsPath\customer-workflows\data\customer-profiles-data.json"
    }
)

$environment = "$collectionsPath\Capstone-Local-Environment.postman_environment.json"
$totalTests = $testCollections.Count
$currentTest = 0
$passedTests = 0
$failedTests = 0

# Run tests for each collection
foreach ($test in $testCollections) {
    $currentTest++
    Write-Host "[$currentTest/$totalTests] Testing $($test.Name)..." -ForegroundColor Yellow
    Write-Host "─────────────────────────────────────────────────────────────" -ForegroundColor Gray
    
    $collectionPath = $test.Collection
    $reportPath = "$testResultsDir\$($test.Report).html"
    $jsonReportPath = "$testResultsDir\$($test.Report).json"
    
    # Check if collection file exists
    if (-not (Test-Path $collectionPath)) {
        Write-Host "✗ Collection file not found: $collectionPath" -ForegroundColor Red
        $failedTests++
        Write-Host ""
        continue
    }
    
    try {
        # Run newman with both HTML and JSON reporters
        # Check if data file exists and use it for data-driven testing
        $newmanArgs = @(
            "run", $collectionPath,
            "-e", $environment,
            "--reporters", "cli,htmlextra,json",
            "--reporter-htmlextra-export", $reportPath,
            "--reporter-json-export", $jsonReportPath,
            "--timeout-request", "10000",
            "--color", "on"
        )
        
        # Add iteration data if data file exists
        if ($test.DataFile -and (Test-Path $test.DataFile)) {
            Write-Host "  Using data file: $($test.DataFile)" -ForegroundColor Cyan
            $newmanArgs += "--iteration-data"
            $newmanArgs += $test.DataFile
        }
        
        npx newman @newmanArgs 2>&1 | Tee-Object -Variable output
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ $($test.Name) tests completed successfully" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "✗ $($test.Name) tests failed (exit code: $LASTEXITCODE)" -ForegroundColor Red
            $failedTests++
        }
    } catch {
        Write-Host "✗ $($test.Name) tests encountered an error: $_" -ForegroundColor Red
        $failedTests++
    }
    
    Write-Host ""
}

# Summary
Write-Host "=== Test Summary ===" -ForegroundColor Green
Write-Host "Total Test Collections: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -gt 0) { "Red" } else { "White" })
Write-Host ""
Write-Host "Test reports available in: $testResultsDir" -ForegroundColor Cyan
Write-Host ""

if ($failedTests -gt 0) {
    Write-Host "⚠ Some tests failed. Check the reports for details." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "✓ All tests passed!" -ForegroundColor Green
    exit 0
}
