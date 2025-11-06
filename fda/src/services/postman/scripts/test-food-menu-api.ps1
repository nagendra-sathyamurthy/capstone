# Simple Food Menu Catalog Testing Script
# Focus on testing the menu system we just created

param(
    [switch]$KeepServiceRunning
)

$ErrorActionPreference = "Stop"

# Colors for output
$Green = "Green"
$Red = "Red"
$Yellow = "Yellow"
$Blue = "Blue"
$Cyan = "Cyan"

Write-Host "🍕 Food Menu Catalog API Testing" -ForegroundColor $Cyan
Write-Host "=================================" -ForegroundColor $Cyan

# Check Newman
try {
    $newmanVersion = newman --version 2>$null
    Write-Host "✅ Newman v$newmanVersion ready" -ForegroundColor $Green
} catch {
    Write-Host "❌ Newman not found. Please install: npm install -g newman" -ForegroundColor $Red
    exit 1
}

# Set up environment
$env:MONGO_CONNECTION_STRING = "mongodb://localhost:27017"
$catalogPort = 5002
$resultsDir = "test-results"

if (!(Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

# Function to check if port is available
function Test-Port {
    param([int]$Port)
    try {
        $connection = Test-NetConnection -ComputerName "localhost" -Port $Port -InformationLevel Quiet -WarningAction SilentlyContinue 2>$null
        return -not $connection
    } catch {
        return $true
    }
}

# Function to wait for service
function Wait-ForService {
    param([int]$Port, [int]$TimeoutSeconds = 60)
    
    $url = "http://localhost:$Port/api/health"
    $startTime = Get-Date
    
    Write-Host "🔍 Waiting for catalog service..." -ForegroundColor $Yellow
    
    do {
        try {
            $response = Invoke-WebRequest -Uri $url -TimeoutSec 5 -UseBasicParsing 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Catalog service is healthy" -ForegroundColor $Green
                return $true
            }
        } catch {
            # Service not ready yet
        }
        
        Start-Sleep -Seconds 2
        $elapsed = (Get-Date) - $startTime
    } while ($elapsed.TotalSeconds -lt $TimeoutSeconds)
    
    Write-Host "❌ Service health check timeout" -ForegroundColor $Red
    return $false
}

# Cleanup function
function Stop-CatalogService {
    if (Test-Path "catalog.pid") {
        try {
            $pid = Get-Content "catalog.pid"
            if ($pid -and (Get-Process -Id $pid -ErrorAction SilentlyContinue)) {
                Stop-Process -Id $pid -Force
                Write-Host "✅ Stopped catalog service (PID: $pid)" -ForegroundColor $Green
            }
            Remove-Item "catalog.pid" -Force
        } catch {
            Write-Host "⚠️  Could not stop catalog service" -ForegroundColor $Yellow
        }
    }
}

try {
    # Check if we need to start the service
    $needToStart = $true
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$catalogPort/api/health" -TimeoutSec 3 -UseBasicParsing 2>$null
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ Catalog service already running" -ForegroundColor $Green
            $needToStart = $false
        }
    } catch {
        # Service not running
    }

    if ($needToStart) {
        # Stop any existing service
        Stop-CatalogService
        Start-Sleep -Seconds 2

        # Check port availability
        if (-not (Test-Port $catalogPort)) {
            Write-Host "❌ Port $catalogPort is already in use" -ForegroundColor $Red
            exit 1
        }

        # Start catalog service
        Write-Host "🚀 Starting catalog service..." -ForegroundColor $Blue
        
        $catalogPath = "..\catalog\Services\catalog.Services.csproj"
        if (!(Test-Path $catalogPath)) {
            Write-Host "❌ Catalog project not found at $catalogPath" -ForegroundColor $Red
            exit 1
        }

        $process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", $catalogPath, "--urls", "http://localhost:$catalogPort" -PassThru -WindowStyle Minimized
        $process.Id | Out-File -FilePath "catalog.pid" -Encoding ASCII
        Write-Host "✅ Catalog service started (PID: $($process.Id))" -ForegroundColor $Green

        # Wait for service to be healthy
        if (-not (Wait-ForService $catalogPort)) {
            Write-Host "❌ Service failed to start properly" -ForegroundColor $Red
            exit 1
        }
    }

    # Seed sample data first
    Write-Host ""
    Write-Host "🌱 Seeding sample menu data..." -ForegroundColor $Blue
    try {
        $seedResponse = Invoke-WebRequest -Uri "http://localhost:$catalogPort/api/seed/sample-menu" -Method POST -UseBasicParsing 2>$null
        if ($seedResponse.StatusCode -eq 200 -or $seedResponse.StatusCode -eq 401) {
            Write-Host "✅ Sample data endpoint accessible" -ForegroundColor $Green
        }
    } catch {
        Write-Host "⚠️  Seeding may require authentication, proceeding with tests..." -ForegroundColor $Yellow
    }

    # Run Newman tests
    Write-Host ""
    Write-Host "🧪 Running Newman tests..." -ForegroundColor $Blue
    
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $jsonResults = Join-Path $resultsDir "food-menu-test-$timestamp.json"
    $htmlResults = Join-Path $resultsDir "food-menu-test-$timestamp.html"

    # Newman command
    $newmanArgs = @(
        "run", "Food-Delivery-API-Collection.postman_collection.json",
        "--environment", "food-delivery-environment.json",
        "--folder", "🍕 Food Menu Catalog Service",
        "--reporters", "cli,json,htmlextra",
        "--reporter-json-export", $jsonResults,
        "--reporter-htmlextra-export", $htmlResults,
        "--timeout-request", "10000",
        "--delay-request", "500",
        "--color", "on",
        "--reporter-htmlextra-title", "Food Delivery Menu API Test Report",
        "--reporter-htmlextra-logs"
    )
    
    Write-Host "Command: newman $($newmanArgs -join ' ')" -ForegroundColor $Cyan
    Write-Host ""
    
    & newman @newmanArgs
    $newmanExitCode = $LASTEXITCODE

    # Process results
    Write-Host ""
    Write-Host "📋 Test Results Summary:" -ForegroundColor $Blue
    Write-Host "========================" -ForegroundColor $Blue

    if (Test-Path $jsonResults) {
        try {
            $results = Get-Content $jsonResults | ConvertFrom-Json
            $run = $results.run
            
            Write-Host ""
            Write-Host "📊 Food Menu API Test Results:" -ForegroundColor $Cyan
            Write-Host "   Total Requests: $($run.stats.requests.total)" -ForegroundColor $Yellow
            Write-Host "   Successful: $($run.stats.requests.total - $run.stats.requests.failed)" -ForegroundColor $Green
            Write-Host "   Failed Requests: $($run.stats.requests.failed)" -ForegroundColor $(if ($run.stats.requests.failed -gt 0) { $Red } else { $Green })
            Write-Host "   Total Tests: $($run.stats.assertions.total)" -ForegroundColor $Yellow
            Write-Host "   Passed Tests: $($run.stats.assertions.total - $run.stats.assertions.failed)" -ForegroundColor $Green
            Write-Host "   Failed Tests: $($run.stats.assertions.failed)" -ForegroundColor $(if ($run.stats.assertions.failed -gt 0) { $Red } else { $Green })
            Write-Host "   Average Response Time: $([math]::Round($run.timings.responseAverage, 2))ms" -ForegroundColor $Yellow
            
            # Success rate
            $successRate = [math]::Round((($run.stats.requests.total - $run.stats.requests.failed) / $run.stats.requests.total) * 100, 1)
            Write-Host "   Success Rate: $successRate%" -ForegroundColor $(if ($successRate -eq 100) { $Green } else { $Yellow })
            
            if ($run.failures -and $run.failures.Count -gt 0) {
                Write-Host ""
                Write-Host "❌ Test Failures:" -ForegroundColor $Red
                $run.failures | Select-Object -First 5 | ForEach-Object {
                    Write-Host "   • $($_.source.name): $($_.error.test)" -ForegroundColor $Red
                    if ($_.error.message) {
                        Write-Host "     $($_.error.message)" -ForegroundColor $Yellow
                    }
                }
                if ($run.failures.Count -gt 5) {
                    Write-Host "   ... and $($run.failures.Count - 5) more failures" -ForegroundColor $Yellow
                }
            }
            
        } catch {
            Write-Host "   ⚠️  Could not parse detailed results from JSON" -ForegroundColor $Yellow
        }
    }

    Write-Host ""
    Write-Host "📁 Generated Reports:" -ForegroundColor $Cyan
    if (Test-Path $jsonResults) {
        Write-Host "   📄 JSON: $jsonResults" -ForegroundColor $Green
    }
    if (Test-Path $htmlResults) {
        Write-Host "   🌐 HTML: $htmlResults" -ForegroundColor $Green
        Write-Host "      Open in browser for detailed visual report" -ForegroundColor $Yellow
    }

    # Final result
    Write-Host ""
    if ($newmanExitCode -eq 0) {
        Write-Host "🎉 Food Menu API tests completed successfully!" -ForegroundColor $Green
        Write-Host "   Your food delivery menu system is working perfectly!" -ForegroundColor $Green
    } else {
        Write-Host "❌ Some tests failed (Exit code: $newmanExitCode)" -ForegroundColor $Red
        Write-Host "   Check the reports above for detailed information" -ForegroundColor $Yellow
    }

    # Service info
    Write-Host ""
    Write-Host "🔗 Service Endpoints:" -ForegroundColor $Blue
    Write-Host "   • Catalog API: http://localhost:$catalogPort/api/menu" -ForegroundColor $Cyan
    Write-Host "   • Swagger UI: http://localhost:$catalogPort/swagger" -ForegroundColor $Cyan
    Write-Host "   • Health Check: http://localhost:$catalogPort/api/health" -ForegroundColor $Cyan

    if (-not $KeepServiceRunning) {
        Stop-CatalogService
    } else {
        Write-Host ""
        Write-Host "🔄 Service is still running for manual testing" -ForegroundColor $Green
        Write-Host "   To stop: Stop-Process -Id $(Get-Content 'catalog.pid')" -ForegroundColor $Yellow
    }

    exit $newmanExitCode

} catch {
    Write-Host ""
    Write-Host "❌ Error during execution:" -ForegroundColor $Red
    Write-Host $_.Exception.Message -FreegroundColor $Red
    exit 1

} finally {
    if (-not $KeepServiceRunning) {
        Stop-CatalogService
    }
}