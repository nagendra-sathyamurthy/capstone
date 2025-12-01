#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Port-forward Kubernetes services to match Docker Compose ports
    
.DESCRIPTION
    This script sets up port forwarding for all Kubernetes services to expose them
    on the same ports as Docker Compose, providing a consistent developer experience.
    
.EXAMPLE
    .\port-forward.ps1
    Start port forwarding for all services
#>

param(
    [Parameter(Mandatory=$false)]
    [switch]$Stop = $false
)

$ErrorActionPreference = "Stop"

# Service port mappings (Kubernetes internal port -> Host port)
$services = @(
    @{Name="mongodb-service"; Namespace="capstone-services"; Port=27017; HostPort=27017},
    @{Name="authentication-service"; Namespace="capstone-services"; Port=8080; HostPort=8081},
    @{Name="catalog-service"; Namespace="capstone-services"; Port=8080; HostPort=8082},
    @{Name="crm-service"; Namespace="capstone-services"; Port=8080; HostPort=8083},
    @{Name="cart-service"; Namespace="capstone-services"; Port=8080; HostPort=8084},
    @{Name="order-service"; Namespace="capstone-services"; Port=8080; HostPort=8085},
    @{Name="payment-service"; Namespace="capstone-services"; Port=8080; HostPort=8086},
    @{Name="gateway-service"; Namespace="capstone-gateway"; Port=5000; HostPort=5000},
    @{Name="customer-app-service"; Namespace="capstone-frontend"; Port=3000; HostPort=3000}
)

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Blue
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

if ($Stop) {
    Write-Host "`n🛑 Stopping all port-forward processes...`n" -ForegroundColor Yellow
    
    # Kill all kubectl port-forward processes
    $processes = Get-Process -Name kubectl -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like "*port-forward*" }
    
    if ($processes) {
        foreach ($proc in $processes) {
            Stop-Process -Id $proc.Id -Force
            Write-Success "Stopped port-forward process (PID: $($proc.Id))"
        }
    } else {
        Write-Info "No port-forward processes found"
    }
    
    Write-Host "`n✅ All port-forwarding stopped`n" -ForegroundColor Green
    exit 0
}

Write-Host @"

╔════════════════════════════════════════════════════════════╗
║                                                            ║
║   🔌 Kubernetes Port Forwarding Setup 🔌                  ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝

"@ -ForegroundColor Cyan

Write-Info "Setting up port forwarding to match Docker Compose ports..."
Write-Host ""

# Check if kubectl is available
try {
    kubectl version --client --short 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "kubectl not found"
    }
} catch {
    Write-Error "kubectl is not installed or not in PATH"
    Write-Info "Please install kubectl and try again"
    exit 1
}

# Start port forwarding for each service
$jobs = @()

foreach ($svc in $services) {
    $serviceName = $svc.Name
    $namespace = $svc.Namespace
    $port = $svc.Port
    $hostPort = $svc.HostPort
    
    Write-Info "Forwarding $serviceName `:$port → localhost:$hostPort"
    
    # Start port-forward in background
    $job = Start-Job -ScriptBlock {
        param($service, $ns, $p, $hp)
        kubectl port-forward -n $ns "service/$service" "${hp}:${p}"
    } -ArgumentList $serviceName, $namespace, $port, $hostPort
    
    $jobs += @{Job=$job; Service=$serviceName; Port=$hostPort}
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Success "Port forwarding started for all services!"

Write-Host @"

📋 Service Access URLs (same as Docker Compose):
   • Customer App:     http://localhost:3000
   • API Gateway:      http://localhost:5000
   • Authentication:   http://localhost:8081
   • Catalog:          http://localhost:8082
   • CRM:              http://localhost:8083
   • Cart:             http://localhost:8084
   • Order:            http://localhost:8085
   • Payment:          http://localhost:8086
   • MongoDB:          mongodb://localhost:27017

ℹ  Port forwarding is running in background jobs
   Press Ctrl+C to stop or run: .\port-forward.ps1 -Stop

"@ -ForegroundColor White

Write-Host "Monitoring port-forward jobs (press Ctrl+C to stop)..." -ForegroundColor Gray
Write-Host ""

try {
    while ($true) {
        Start-Sleep -Seconds 5
        
        # Check job status
        foreach ($jobInfo in $jobs) {
            $job = $jobInfo.Job
            $service = $jobInfo.Service
            $port = $jobInfo.Port
            
            if ($job.State -ne "Running") {
                Write-Warning "$service (port $port) - Job state: $($job.State)"
                
                # Restart failed jobs
                if ($job.State -eq "Failed" -or $job.State -eq "Stopped") {
                    Write-Info "Restarting $service..."
                    $svcConfig = $services | Where-Object { $_.Name -eq $service }
                    $newJob = Start-Job -ScriptBlock {
                        param($s, $ns, $p, $hp)
                        kubectl port-forward -n $ns "service/$s" "${hp}:${p}"
                    } -ArgumentList $svcConfig.Name, $svcConfig.Namespace, $svcConfig.Port, $svcConfig.HostPort
                    
                    $jobInfo.Job = $newJob
                }
            }
        }
    }
} finally {
    Write-Host "`n`nCleaning up..." -ForegroundColor Yellow
    foreach ($jobInfo in $jobs) {
        Stop-Job -Job $jobInfo.Job -ErrorAction SilentlyContinue
        Remove-Job -Job $jobInfo.Job -ErrorAction SilentlyContinue
    }
    Write-Success "Port forwarding stopped"
}
