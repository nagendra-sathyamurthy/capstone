# Test Authentication Service Only
# Quick test script for Authentication Service development

param(
    [switch]$SkipServiceStart,
    [switch]$KeepServiceRunning
)

$ErrorActionPreference = "Stop"

# Colors for output
$Green = "Green"
$Red = "Red"
$Yellow = "Yellow"
$Blue = "Blue"
$Cyan = "Cyan"

Write-Host "🔐 Authentication Service Test Runner" -ForegroundColor $Cyan
Write-Host "====================================" -ForegroundColor $Cyan
Write-Host ""

# Directories
$rootDir = Split-Path -Parent $PSScriptRoot
$servicesDir = $rootDir
$postmanDir = Join-Path $rootDir "postman"
$resultsDir = Join-Path $postmanDir "test-results"

# Service configuration
$service = @{
    Name = "Authentication"
    Path = "authentication\Services"
    Project = "authentication.csproj"
    Port = 5001
    HealthEndpoint = "/api/health"
}

# Create results directory
if (!(Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

function Wait-ForServiceHealth {
    param([int]$Port, [string]$HealthEndpoint, [int]$TimeoutSeconds = 30)
    
    $url = "http://localhost:$Port$HealthEndpoint"
    $startTime = Get-Date
    
    Write-Host "   🔍 Waiting for service health check..." -ForegroundColor $Yellow
    
    do {
        try {
            $response = Invoke-WebRequest -Uri $url -TimeoutSec 5 -UseBasicParsing 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Host "   ✅ Authentication service is healthy" -ForegroundColor $Green
                return $true
            }
        } catch {
            # Service not ready yet
        }
        
        Start-Sleep -Seconds 2
        $elapsed = (Get-Date) - $startTime
    } while ($elapsed.TotalSeconds -lt $TimeoutSeconds)
    
    Write-Host "   ❌ Authentication service health check timeout" -ForegroundColor $Red
    return $false
}

try {
    if (-not $SkipServiceStart) {
        # Check if service is already running
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$($service.Port)$($service.HealthEndpoint)" -TimeoutSec 3 -UseBasicParsing 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Authentication service is already running on port $($service.Port)" -ForegroundColor $Green
            }
        } catch {
            # Start the service
            Write-Host "🚀 Starting Authentication service..." -ForegroundColor $Blue
            Push-Location $servicesDir
            
            $servicePath = Join-Path $service.Path $service.Project
            $process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", $servicePath, "--urls", "http://localhost:$($service.Port)" -PassThru -WindowStyle Hidden
            
            Write-Host "   ✅ Authentication service started (PID: $($process.Id))" -ForegroundColor $Green
            Pop-Location
            
            # Wait for health check
            if (-not (Wait-ForServiceHealth $service.Port $service.HealthEndpoint)) {
                Write-Host "❌ Authentication service failed to start properly" -ForegroundColor $Red
                exit 1
            }
        }
    }

    # Run Newman test
    Write-Host ""
    Write-Host "🧪 Running Authentication Service tests..." -ForegroundColor $Blue
    
    $collectionFile = Join-Path $postmanDir "collections\Authentication-Service.postman_collection.json"
    $environmentFile = Join-Path $postmanDir "environments\Food-Delivery-Local.postman_environment.json"
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    
    # Test results files
    $jsonResults = Join-Path $resultsDir "newman-auth-$timestamp.json"
    $htmlResults = Join-Path $resultsDir "newman-auth-$timestamp.html"

    Push-Location $postmanDir

    $newmanArgs = @(
        "run", $collectionFile,
        "--environment", $environmentFile,
        "--reporters", "cli,json,htmlextra",
        "--reporter-json-export", $jsonResults,
        "--reporter-htmlextra-export", $htmlResults,
        "--timeout-request", "10000",
        "--timeout-script", "5000",
        "--delay-request", "500",
        "--color", "on"
    )
    
    Write-Host "   📊 Executing Authentication Service tests..." -ForegroundColor $Yellow
    Write-Host ""
    
    & newman @newmanArgs
    $exitCode = $LASTEXITCODE
    
    Pop-Location

    # Show results
    Write-Host ""
    Write-Host "📋 Authentication Service Test Results:" -ForegroundColor $Blue
    Write-Host "======================================" -ForegroundColor $Blue

    if (Test-Path $jsonResults) {
        try {
            $results = Get-Content $jsonResults | ConvertFrom-Json
            $run = $results.run
            
            Write-Host ""
            Write-Host "📊 Test Statistics:" -ForegroundColor $Cyan
            Write-Host "   Requests: $($run.stats.requests.total) total, $($run.stats.requests.failed) failed" -ForegroundColor $Yellow
            Write-Host "   Assertions: $($run.stats.assertions.total) total, $($run.stats.assertions.failed) failed" -ForegroundColor $Yellow
            Write-Host "   Average Response Time: $([math]::Round($run.timings.responseAverage, 2))ms" -ForegroundColor $Yellow
            
            if ($run.failures -and $run.failures.Count -gt 0) {
                Write-Host ""
                Write-Host "❌ Test Failures ($($run.failures.Count)):" -ForegroundColor $Red
                $run.failures | ForEach-Object {
                    Write-Host "   • $($_.source.name): $($_.error.test)" -ForegroundColor $Red
                    Write-Host "     $($_.error.message)" -ForegroundColor $Yellow
                }
            }
            
        } catch {
            Write-Host "   ⚠️  Could not parse detailed results" -ForegroundColor $Yellow
        }
    }

    Write-Host ""
    Write-Host "📁 Generated Reports:" -ForegroundColor $Cyan
    if (Test-Path $jsonResults) {
        Write-Host "   📄 JSON Report: $jsonResults" -ForegroundColor $Green
    }
    if (Test-Path $htmlResults) {
        Write-Host "   🌐 HTML Report: $htmlResults" -ForegroundColor $Green
    }

    Write-Host ""
    if ($exitCode -eq 0) {
        Write-Host "🎉 Authentication Service tests passed!" -ForegroundColor $Green
    } else {
        Write-Host "❌ Authentication Service tests failed!" -ForegroundColor $Red
    }

    if ($KeepServiceRunning) {
        Write-Host ""
        Write-Host "🔄 Service is still running for manual testing:" -ForegroundColor $Blue
        Write-Host "   🔐 Authentication API: http://localhost:$($service.Port)" -ForegroundColor $Cyan
        Write-Host "   📚 Swagger UI: http://localhost:$($service.Port)/swagger" -ForegroundColor $Cyan
        Write-Host ""
        Write-Host "   To stop the service, press Ctrl+C or close this window" -ForegroundColor $Yellow
    }

    exit $exitCode

} catch {
    Write-Host ""
    Write-Host "❌ Error during execution:" -ForegroundColor $Red
    Write-Host $_.Exception.Message -ForegroundColor $Red
    exit 1
}