# Food Delivery API Testing with Newman - Service-Specific Collections
# This script starts all services and runs comprehensive Postman tests using separate collections

param(
    [switch]$SkipBuild,
    [switch]$KeepServicesRunning,
    [int]$TestTimeout = 300,
    [string[]]$Collections = @(),  # Specific collections to test (empty = all)
    [switch]$Sequential = $false   # Run collections sequentially vs parallel
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

# Colors for output
$Green = "Green"
$Red = "Red"
$Yellow = "Yellow"
$Blue = "Blue"
$Cyan = "Cyan"
$Magenta = "Magenta"

Write-Host "🍕 Food Delivery API Testing Suite - Service Collections" -ForegroundColor $Cyan
Write-Host "=======================================================" -ForegroundColor $Cyan
Write-Host ""

# Check prerequisites
Write-Host "🔍 Checking prerequisites..." -ForegroundColor $Blue

# Check Newman
try {
    $newmanVersion = newman --version 2>$null
    Write-Host "✅ Newman installed: v$newmanVersion" -ForegroundColor $Green
} catch {
    Write-Host "❌ Newman not found. Installing..." -ForegroundColor $Red
    npm install -g newman
    $newmanVersion = newman --version
    Write-Host "✅ Newman installed: v$newmanVersion" -ForegroundColor $Green
}

# Check .NET
try {
    $dotnetVersion = dotnet --version 2>$null
    Write-Host "✅ .NET installed: v$dotnetVersion" -ForegroundColor $Green
} catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 9.0 SDK" -ForegroundColor $Red
    exit 1
}

# Service configuration
$services = @(
    @{
        Name = "Authentication"
        Path = "authentication\Services"
        Project = "authentication.csproj"
        Port = 5001
        HealthEndpoint = "/api/health"
        PidFile = "auth.pid"
    },
    @{    
        Name = "Catalog (Menu)"
        Path = "catalog\Services"
        Project = "catalog.Services.csproj"
        Port = 5002
        HealthEndpoint = "/api/health"
        PidFile = "catalog.pid"
    },
    @{
        Name = "CRM"
        Path = "crm\Services"
        Project = "crm.csproj"
        Port = 5003
        HealthEndpoint = "/api/health"
        PidFile = "crm.pid"
    }
)

# Directories
$rootDir = Split-Path -Parent $PSScriptRoot
$servicesDir = $rootDir
$postmanDir = Join-Path $rootDir "postman"
$resultsDir = Join-Path $postmanDir "test-results"

# Create results directory
if (!(Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

# Set environment variable for MongoDB
$env:MONGO_CONNECTION_STRING = "mongodb://localhost:27017"

Write-Host ""
Write-Host "📁 Working Directories:" -ForegroundColor $Blue
Write-Host "   Services: $servicesDir" -ForegroundColor $Yellow
Write-Host "   Postman: $postmanDir" -ForegroundColor $Yellow
Write-Host "   Results: $resultsDir" -ForegroundColor $Yellow
Write-Host ""

# Function to check if port is available
function Test-Port {
    param([int]$Port)
    try {
        $connection = Test-NetConnection -ComputerName "localhost" -Port $Port -InformationLevel Quiet -WarningAction SilentlyContinue
        return -not $connection
    } catch {
        return $true
    }
}

# Function to wait for service health
function Wait-ForServiceHealth {
    param(
        [string]$ServiceName,
        [int]$Port,
        [string]$HealthEndpoint,
        [int]$TimeoutSeconds = 60
    )
    
    $url = "http://localhost:$Port$HealthEndpoint"
    $startTime = Get-Date
    
    Write-Host "   🔍 Waiting for $ServiceName health check..." -ForegroundColor $Yellow
    
    do {
        try {
            $response = Invoke-WebRequest -Uri $url -TimeoutSec 5 -UseBasicParsing 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Host "   ✅ $ServiceName is healthy" -ForegroundColor $Green
                return $true
            }
        } catch {
            # Service not ready yet
        }
        
        Start-Sleep -Seconds 2
        $elapsed = (Get-Date) - $startTime
    } while ($elapsed.TotalSeconds -lt $TimeoutSeconds)
    
    Write-Host "   ❌ $ServiceName health check timeout" -ForegroundColor $Red
    return $false
}

# Function to stop all services
function Stop-AllServices {
    Write-Host ""
    Write-Host "🛑 Stopping all services..." -ForegroundColor $Blue
    
    foreach ($service in $services) {
        $pidFile = Join-Path $postmanDir $service.PidFile
        if (Test-Path $pidFile) {
            try {
                $pid = Get-Content $pidFile
                if ($pid -and (Get-Process -Id $pid -ErrorAction SilentlyContinue)) {
                    Stop-Process -Id $pid -Force
                    Write-Host "   ✅ Stopped $($service.Name) (PID: $pid)" -ForegroundColor $Green
                }
                Remove-Item $pidFile -Force
            } catch {
                Write-Host "   ⚠️  Could not stop $($service.Name)" -ForegroundColor $Yellow
            }
        }
    }
    
    # Kill any remaining dotnet processes for our services
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | 
        Where-Object { $_.ProcessName -eq "dotnet" } |
        ForEach-Object {
            try {
                $_.Kill()
            } catch {}
        }
}

# Cleanup on exit
$cleanup = {
    if (-not $KeepServicesRunning) {
        Stop-AllServices
    }
}
Register-EngineEvent PowerShell.Exiting -Action $cleanup

try {
    # Build services if not skipped
    if (-not $SkipBuild) {
        Write-Host "🔨 Building services..." -ForegroundColor $Blue
        Push-Location $servicesDir
        
        foreach ($service in $services) {
            Write-Host "   Building $($service.Name)..." -ForegroundColor $Yellow
            $buildPath = Join-Path $service.Path $service.Project
            $buildResult = dotnet build $buildPath --verbosity quiet 2>&1
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   ✅ $($service.Name) built successfully" -ForegroundColor $Green
            } else {
                Write-Host "   ❌ $($service.Name) build failed:" -ForegroundColor $Red
                Write-Host $buildResult -ForegroundColor $Red
                exit 1
            }
        }
        Pop-Location
    }

    # Stop any existing services
    Stop-AllServices
    Start-Sleep -Seconds 2

    # Start services
    Write-Host ""
    Write-Host "🚀 Starting services..." -ForegroundColor $Blue
    Push-Location $servicesDir

    foreach ($service in $services) {
        if (-not (Test-Port $service.Port)) {
            Write-Host "   ❌ Port $($service.Port) is already in use" -ForegroundColor $Red
            continue
        }

        Write-Host "   Starting $($service.Name) on port $($service.Port)..." -ForegroundColor $Yellow
        
        $servicePath = Join-Path $service.Path $service.Project
        $process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", $servicePath, "--urls", "http://localhost:$($service.Port)" -PassThru -WindowStyle Hidden
        
        # Save PID for cleanup
        $pidFile = Join-Path $postmanDir $service.PidFile
        $process.Id | Out-File -FilePath $pidFile -Encoding ASCII
        
        Write-Host "   ✅ $($service.Name) started (PID: $($process.Id))" -ForegroundColor $Green
    }
    
    Pop-Location

    # Wait for all services to be healthy
    Write-Host ""
    Write-Host "🏥 Checking service health..." -ForegroundColor $Blue
    
    $allHealthy = $true
    foreach ($service in $services) {
        if (-not (Wait-ForServiceHealth $service.Name $service.Port $service.HealthEndpoint)) {
            $allHealthy = $false
        }
    }

    if (-not $allHealthy) {
        Write-Host ""
        Write-Host "❌ Not all services are healthy. Cannot proceed with testing." -ForegroundColor $Red
        exit 1
    }

    # Define available collections
    $availableCollections = @(
        @{
            Name = "Authentication Service"
            File = "collections\Authentication-Service.postman_collection.json"
            Service = "Authentication"
            Icon = "🔐"
            Priority = 1
        },
        @{
            Name = "Food Menu Catalog"
            File = "collections\Food-Menu-Catalog.postman_collection.json"
            Service = "Catalog (Menu)"
            Icon = "🍽️"
            Priority = 2
        },
        @{
            Name = "CRM Service"
            File = "collections\CRM-Service.postman_collection.json"
            Service = "CRM"
            Icon = "👥"
            Priority = 3
        },
        @{
            Name = "Cart Service"
            File = "collections\Cart-Service.postman_collection.json"
            Service = "Cart"
            Icon = "🛒"
            Priority = 4
        },
        @{
            Name = "Legacy Catalog"
            File = "collections\Legacy-Catalog.postman_collection.json"
            Service = "Legacy"
            Icon = "📚"
            Priority = 5
        }
    )

    # Filter collections if specific ones requested
    if ($Collections.Count -gt 0) {
        $collectionsToTest = $availableCollections | Where-Object { $Collections -contains $_.Name }
        if ($collectionsToTest.Count -eq 0) {
            Write-Host "❌ No matching collections found. Available collections:" -ForegroundColor $Red
            $availableCollections | ForEach-Object { Write-Host "   • $($_.Name)" -ForegroundColor $Yellow }
            exit 1
        }
    } else {
        $collectionsToTest = $availableCollections
    }

    # Sort by priority
    $collectionsToTest = $collectionsToTest | Sort-Object Priority

    # Run Newman tests
    Write-Host ""
    Write-Host "🧪 Running Newman tests for $($collectionsToTest.Count) collection(s)..." -ForegroundColor $Blue
    
    $environmentFile = Join-Path $postmanDir "environments\Food-Delivery-Local.postman_environment.json"
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    
    # Test results tracking
    $allResults = @()
    $totalFailures = 0
    
    Write-Host "   Environment: Food-Delivery-Local.postman_environment.json" -ForegroundColor $Cyan
    Write-Host "   Mode: $(if ($Sequential) { 'Sequential' } else { 'Parallel' })" -ForegroundColor $Cyan
    Write-Host ""

    Push-Location $postmanDir

    if ($Sequential) {
        # Run collections sequentially
        foreach ($collection in $collectionsToTest) {
            Write-Host "🔄 Testing $($collection.Icon) $($collection.Name)..." -ForegroundColor $Blue
            
            $collectionFile = $collection.File
            $safeName = $collection.Name -replace '[^a-zA-Z0-9]', '-'
            
            # Test results files for this collection
            $jsonResults = Join-Path $resultsDir "newman-$safeName-$timestamp.json"
            $htmlResults = Join-Path $resultsDir "newman-$safeName-$timestamp.html"
            $junitResults = Join-Path $resultsDir "newman-$safeName-$timestamp.xml"

            $newmanArgs = @(
                "run", $collectionFile,
                "--environment", $environmentFile,
                "--reporters", "cli,json,htmlextra,junit",
                "--reporter-json-export", $jsonResults,
                "--reporter-htmlextra-export", $htmlResults,
                "--reporter-junit-export", $junitResults,
                "--timeout-request", "10000",
                "--timeout-script", "5000",
                "--delay-request", "500",
                "--color", "on"
            )
            
            Write-Host "   📊 Running $($collection.Name) tests..." -ForegroundColor $Yellow
            
            & newman @newmanArgs
            $exitCode = $LASTEXITCODE
            
            # Parse results
            $result = @{
                Collection = $collection.Name
                Icon = $collection.Icon
                ExitCode = $exitCode
                JsonResults = $jsonResults
                HtmlResults = $htmlResults
                JunitResults = $junitResults
            }
            
            if (Test-Path $jsonResults) {
                try {
                    $jsonContent = Get-Content $jsonResults | ConvertFrom-Json
                    $result.Stats = $jsonContent.run.stats
                    $result.Failures = $jsonContent.run.failures
                    $result.Timings = $jsonContent.run.timings
                } catch {
                    Write-Host "   ⚠️  Could not parse results for $($collection.Name)" -ForegroundColor $Yellow
                }
            }
            
            $allResults += $result
            
            if ($exitCode -eq 0) {
                Write-Host "   ✅ $($collection.Icon) $($collection.Name) - All tests passed!" -ForegroundColor $Green
            } else {
                Write-Host "   ❌ $($collection.Icon) $($collection.Name) - Some tests failed!" -ForegroundColor $Red
                $totalFailures++
            }
            
            Write-Host ""
        }
    } else {
        # Parallel execution (future enhancement)
        Write-Host "ℹ️  Parallel execution not yet implemented. Running sequentially..." -ForegroundColor $Yellow
        Write-Host ""
        
        # For now, run sequentially even when parallel is requested
        foreach ($collection in $collectionsToTest) {
            Write-Host "🔄 Testing $($collection.Icon) $($collection.Name)..." -ForegroundColor $Blue
            
            $collectionFile = $collection.File
            $safeName = $collection.Name -replace '[^a-zA-Z0-9]', '-'
            
            # Test results files for this collection
            $jsonResults = Join-Path $resultsDir "newman-$safeName-$timestamp.json"
            $htmlResults = Join-Path $resultsDir "newman-$safeName-$timestamp.html"
            $junitResults = Join-Path $resultsDir "newman-$safeName-$timestamp.xml"

            $newmanArgs = @(
                "run", $collectionFile,
                "--environment", $environmentFile,
                "--reporters", "cli,json,htmlextra,junit",
                "--reporter-json-export", $jsonResults,
                "--reporter-htmlextra-export", $htmlResults,
                "--reporter-junit-export", $junitResults,
                "--timeout-request", "10000",
                "--timeout-script", "5000",
                "--delay-request", "500",
                "--color", "on"
            )
            
            Write-Host "   � Running $($collection.Name) tests..." -ForegroundColor $Yellow
            
            & newman @newmanArgs
            $exitCode = $LASTEXITCODE
            
            # Parse results
            $result = @{
                Collection = $collection.Name
                Icon = $collection.Icon
                ExitCode = $exitCode
                JsonResults = $jsonResults
                HtmlResults = $htmlResults
                JunitResults = $junitResults
            }
            
            if (Test-Path $jsonResults) {
                try {
                    $jsonContent = Get-Content $jsonResults | ConvertFrom-Json
                    $result.Stats = $jsonContent.run.stats
                    $result.Failures = $jsonContent.run.failures
                    $result.Timings = $jsonContent.run.timings
                } catch {
                    Write-Host "   ⚠️  Could not parse results for $($collection.Name)" -ForegroundColor $Yellow
                }
            }
            
            $allResults += $result
            
            if ($exitCode -eq 0) {
                Write-Host "   ✅ $($collection.Icon) $($collection.Name) - All tests passed!" -ForegroundColor $Green
            } else {
                Write-Host "   ❌ $($collection.Icon) $($collection.Name) - Some tests failed!" -ForegroundColor $Red
                $totalFailures++
            }
            
            Write-Host ""
        }
    }
    
    Pop-Location

    # Process results
    Write-Host ""
    Write-Host "📋 Comprehensive Test Results Summary:" -ForegroundColor $Blue
    Write-Host "=====================================" -ForegroundColor $Blue

    # Overall statistics
    $totalRequests = ($allResults | Where-Object { $_.Stats } | ForEach-Object { $_.Stats.requests.total } | Measure-Object -Sum).Sum
    $totalFailedRequests = ($allResults | Where-Object { $_.Stats } | ForEach-Object { $_.Stats.requests.failed } | Measure-Object -Sum).Sum
    $totalAssertions = ($allResults | Where-Object { $_.Stats } | ForEach-Object { $_.Stats.assertions.total } | Measure-Object -Sum).Sum
    $totalFailedAssertions = ($allResults | Where-Object { $_.Stats } | ForEach-Object { $_.Stats.assertions.failed } | Measure-Object -Sum).Sum
    $avgResponseTime = if ($allResults | Where-Object { $_.Timings }) { 
        ($allResults | Where-Object { $_.Timings } | ForEach-Object { $_.Timings.responseAverage } | Measure-Object -Average).Average 
    } else { 0 }

    Write-Host ""
    Write-Host "📊 Overall Execution Statistics:" -ForegroundColor $Cyan
    Write-Host "   Collections Tested: $($allResults.Count)" -ForegroundColor $Yellow
    Write-Host "   Total Requests: $totalRequests" -ForegroundColor $Yellow
    Write-Host "   Failed Requests: $totalFailedRequests" -ForegroundColor $(if ($totalFailedRequests -gt 0) { $Red } else { $Green })
    Write-Host "   Total Assertions: $totalAssertions" -ForegroundColor $Yellow
    Write-Host "   Failed Assertions: $totalFailedAssertions" -ForegroundColor $(if ($totalFailedAssertions -gt 0) { $Red } else { $Green })
    Write-Host "   Average Response Time: $([math]::Round($avgResponseTime, 2))ms" -ForegroundColor $Yellow

    # Per-collection results
    Write-Host ""
    Write-Host "📝 Results by Collection:" -ForegroundColor $Cyan
    foreach ($result in $allResults) {
        $status = if ($result.ExitCode -eq 0) { "✅ PASS" } else { "❌ FAIL" }
        $statusColor = if ($result.ExitCode -eq 0) { $Green } else { $Red }
        
        Write-Host "   $($result.Icon) $($result.Collection): $status" -ForegroundColor $statusColor
        
        if ($result.Stats) {
            Write-Host "      Requests: $($result.Stats.requests.total) total, $($result.Stats.requests.failed) failed" -ForegroundColor $Yellow
            Write-Host "      Assertions: $($result.Stats.assertions.total) total, $($result.Stats.assertions.failed) failed" -ForegroundColor $Yellow
            if ($result.Timings) {
                Write-Host "      Avg Response: $([math]::Round($result.Timings.responseAverage, 2))ms" -ForegroundColor $Yellow
            }
        }
    }

    # Show failures if any
    $allFailures = $allResults | Where-Object { $_.Failures -and $_.Failures.Count -gt 0 }
    if ($allFailures.Count -gt 0) {
        Write-Host ""
        Write-Host "❌ Test Failures by Collection:" -ForegroundColor $Red
        foreach ($result in $allFailures) {
            Write-Host "   $($result.Icon) $($result.Collection) ($($result.Failures.Count) failures):" -ForegroundColor $Red
            $result.Failures | ForEach-Object {
                Write-Host "      • $($_.source.name): $($_.error.test)" -ForegroundColor $Red
                Write-Host "        $($_.error.message)" -ForegroundColor $Yellow
            }
        }
    }

    Write-Host ""
    Write-Host "📁 Generated Reports:" -ForegroundColor $Cyan
    foreach ($result in $allResults) {
        Write-Host "   $($result.Icon) $($result.Collection):" -ForegroundColor $Magenta
        if (Test-Path $result.JsonResults) {
            Write-Host "      📄 JSON: $($result.JsonResults)" -ForegroundColor $Green
        }
        if (Test-Path $result.HtmlResults) {
            Write-Host "      🌐 HTML: $($result.HtmlResults)" -ForegroundColor $Green
        }
        if (Test-Path $result.JunitResults) {
            Write-Host "      📋 JUnit: $($result.JunitResults)" -ForegroundColor $Green
        }
    }

    # Overall result
    Write-Host ""
    if ($totalFailures -eq 0) {
        Write-Host "🎉 All collections passed successfully!" -ForegroundColor $Green
        Write-Host "   The Food Delivery API ecosystem is working perfectly!" -ForegroundColor $Green
        Write-Host "   Tested $($allResults.Count) service collection(s) with $totalRequests requests and $totalAssertions assertions." -ForegroundColor $Green
    } else {
        Write-Host "❌ $totalFailures out of $($allResults.Count) collection(s) failed" -ForegroundColor $Red
        Write-Host "   Check the detailed reports above for more information." -ForegroundColor $Yellow
    }

    # Service management info
    Write-Host ""
    if ($KeepServicesRunning) {
        Write-Host "🔄 Services are still running for manual testing:" -ForegroundColor $Blue
        foreach ($service in $services) {
            Write-Host "   • $($service.Name): http://localhost:$($service.Port)" -ForegroundColor $Cyan
        }
        Write-Host ""
        Write-Host "   🍕 Catalog (Menu) Swagger: http://localhost:5002/swagger" -ForegroundColor $Green
        Write-Host "   🔐 Authentication Swagger: http://localhost:5001/swagger" -FreegroundColor $Green
        Write-Host ""
        Write-Host "   To stop services later, run: ./stop-services.ps1" -ForegroundColor $Yellow
    }

    exit $(if ($totalFailures -gt 0) { 1 } else { 0 })

} catch {
    Write-Host ""
    Write-Host "❌ Error during execution:" -ForegroundColor $Red
    Write-Host $_.Exception.Message -ForegroundColor $Red
    Write-Host $_.ScriptStackTrace -ForegroundColor $Yellow
    exit 1
}