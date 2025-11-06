# PowerShell Script to Start All Capstone Services for Newman Testing
# This script starts all microservices for API testing

Write-Host "🚀 Starting Capstone Microservices for Newman Testing..." -ForegroundColor Green

# Set the base directory
$BaseDir = "c:\dotnet\capstone\fda\src\services"

# Function to start a service
function Start-Service {
    param(
        [string]$ServiceName,
        [int]$Port
    )
    
    $ServiceDir = "$BaseDir\$ServiceName\Services"
    $LogDir = "$BaseDir\$ServiceName\logs"
    
    # Create logs directory if it doesn't exist
    if (!(Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    
    Write-Host "Starting $ServiceName service on port $Port..." -ForegroundColor Yellow
    
    # Start the service
    $Process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls=http://localhost:$Port" -WorkingDirectory $ServiceDir -PassThru -WindowStyle Hidden
    
    # Store the PID for later cleanup
    $Process.Id | Out-File "$LogDir\$ServiceName.pid"
    
    Write-Host "✅ $ServiceName service started (PID: $($Process.Id))" -ForegroundColor Green
    
    return $Process
}

# Start all services
$AuthProcess = Start-Service -ServiceName "authentication" -Port 5001
$CatalogProcess = Start-Service -ServiceName "catalog" -Port 5002  
$CrmProcess = Start-Service -ServiceName "crm" -Port 5003
$CartProcess = Start-Service -ServiceName "cart" -Port 5004

Write-Host ""
Write-Host "⏳ Waiting 30 seconds for services to fully start..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

Write-Host ""
Write-Host "🔍 Checking service health..." -ForegroundColor Cyan

# Function to check service health
function Test-ServiceHealth {
    param(
        [string]$ServiceName,
        [int]$Port,
        [string]$HealthEndpoint
    )
    
    try {
        $Response = Invoke-WebRequest -Uri "http://localhost:$Port$HealthEndpoint" -Method GET -TimeoutSec 5 -UseBasicParsing
        if ($Response.StatusCode -eq 200) {
            Write-Host "✅ $ServiceName service is healthy" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "❌ $ServiceName service is not responding" -ForegroundColor Red
        return $false
    }
}

$AuthHealthy = Test-ServiceHealth -ServiceName "Authentication" -Port 5001 -HealthEndpoint "/api/health"
$CatalogHealthy = Test-ServiceHealth -ServiceName "Catalog" -Port 5002 -HealthEndpoint "/health"
$CrmHealthy = Test-ServiceHealth -ServiceName "CRM" -Port 5003 -HealthEndpoint "/api/health"
$CartHealthy = Test-ServiceHealth -ServiceName "Cart" -Port 5004 -HealthEndpoint "/api/health"

Write-Host ""
if ($AuthHealthy -and $CatalogHealthy -and $CrmHealthy -and $CartHealthy) {
    Write-Host "🧪 All services healthy! Running Newman tests..." -ForegroundColor Green
    
    # Change to postman directory
    Set-Location "$BaseDir\postman"
    
    # Run Newman with comprehensive reporting
    newman run "Capstone-Services.postman_collection.json" -e "local-environment.json" --reporter-json-export "test-results/newman-report.json" --timeout-request 10000
    
    Write-Host ""
    Write-Host "📊 Test Results:" -ForegroundColor Cyan
    Write-Host "- JSON Report: $BaseDir\postman\test-results\newman-report.json"
    Write-Host "- Summary Report: $BaseDir\postman\test-results\newman-test-summary.md"
} else {
    Write-Host "⚠️  Some services are not healthy. Newman tests may fail." -ForegroundColor Yellow
    Write-Host "You can still run tests manually with:" -ForegroundColor Yellow
    Write-Host "cd '$BaseDir\postman'" -ForegroundColor Gray
    Write-Host "newman run 'Capstone-Services.postman_collection.json' -e 'local-environment.json'" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🛑 To stop all services, run:" -ForegroundColor Red
Write-Host "Stop-Process -Id $($AuthProcess.Id), $($CatalogProcess.Id), $($CrmProcess.Id), $($CartProcess.Id) -Force" -ForegroundColor Gray

Write-Host ""
Write-Host "📝 Service Process IDs:" -ForegroundColor Cyan
Write-Host "- Authentication: $($AuthProcess.Id)"
Write-Host "- Catalog: $($CatalogProcess.Id)"
Write-Host "- CRM: $($CrmProcess.Id)"
Write-Host "- Cart: $($CartProcess.Id)"

# Store all PIDs in a file for easy cleanup
$AllPids = @($AuthProcess.Id, $CatalogProcess.Id, $CrmProcess.Id, $CartProcess.Id)
$AllPids | Out-File "$BaseDir\postman\test-results\service-pids.txt"

Write-Host ""
Write-Host "✨ Services are running! You can now run Newman tests or use the APIs." -ForegroundColor Green