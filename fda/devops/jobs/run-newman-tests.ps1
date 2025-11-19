# Run all Newman tests for deployed services
# This script runs API tests against locally deployed services

Write-Host "=== Capstone Services - Newman Test Suite ===" -ForegroundColor Green
Write-Host ""

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Cyan
$dockerRunning = $false
$maxAttempts = 10
$attempt = 0

while (-not $dockerRunning -and $attempt -lt $maxAttempts) {
    try {
        docker ps > $null 2>&1
        if ($?) {
            $dockerRunning = $true
            Write-Host "✓ Docker is running" -ForegroundColor Green
        }
    } catch {
        $attempt++
        if ($attempt -lt $maxAttempts) {
            Write-Host "Waiting for Docker... (attempt $attempt/$maxAttempts)" -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
    }
}

if (-not $dockerRunning) {
    Write-Host "✗ Docker is not running. Please start Docker Desktop or Rancher Desktop first." -ForegroundColor Red
    Write-Host "  Run: rdctl start" -ForegroundColor Yellow
    exit 1
}

# Check if services are running
Write-Host ""
Write-Host "Checking services..." -ForegroundColor Cyan
$services = docker ps --filter "name=capstone-" --format "{{.Names}}" 2>$null

if (-not $services) {
    Write-Host "✗ No services are running. Please deploy services first." -ForegroundColor Red
    Write-Host "  Run: docker-compose -f docker-compose-local.yml up -d" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Services are running:" -ForegroundColor Green
$services | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }

# Create test results directory
$testResultsDir = "..\..\..\..\test-results"
New-Item -ItemType Directory -Force -Path $testResultsDir | Out-Null
Write-Host ""
Write-Host "Test results will be saved to: $testResultsDir" -ForegroundColor Cyan
Write-Host ""

# Define test collections for deployed services
$testCollections = @(
    @{
        Name = "Authentication"
        Collection = "Authentication-Service-Fixed.postman_collection.json"
        Report = "authentication-test-report"
    },
    @{
        Name = "Catalog"
        Collection = "Catalog-Service.postman_collection.json"
        Report = "catalog-test-report"
    },
    @{
        Name = "CRM"
        Collection = "CRM-Service.postman_collection.json"
        Report = "crm-test-report"
    },
    @{
        Name = "Cart"
        Collection = "Cart-Service.postman_collection.json"
        Report = "cart-test-report"
    }
)

$environment = "Capstone-Local-Environment.postman_environment.json"
$totalTests = $testCollections.Count
$currentTest = 0
$passedTests = 0
$failedTests = 0

# Run tests for each service
foreach ($test in $testCollections) {
    $currentTest++
    Write-Host "[$currentTest/$totalTests] Testing $($test.Name) Service..." -ForegroundColor Yellow
    Write-Host "─────────────────────────────────────────────────────────────" -ForegroundColor Gray
    
    $collectionPath = $test.Collection
    $reportPath = "$testResultsDir\$($test.Report).html"
    $jsonReportPath = "$testResultsDir\$($test.Report).json"
    
    try {
        # Run newman with both HTML and JSON reporters
        $result = npx newman run $collectionPath `
            -e $environment `
            --reporters cli,htmlextra,json `
            --reporter-htmlextra-export $reportPath `
            --reporter-json-export $jsonReportPath `
            --timeout-request 10000 `
            --bail 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ $($test.Name) tests completed successfully" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "✗ $($test.Name) tests failed" -ForegroundColor Red
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
Write-Host "Total Services Tested: $totalTests" -ForegroundColor White
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
