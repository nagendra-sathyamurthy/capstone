# Gateway Build and Deploy Script for Local Development
# This script builds the gateway Docker image and deploys it to local Kubernetes

param(
    [switch]$SkipBuild = $false,
    [switch]$SkipDeploy = $false
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Capstone API Gateway - Local Deployment  " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"
$gatewayPath = "C:\dotnet\capstone\fda\src\gateway"

# Build Docker image
if (-not $SkipBuild) {
    Write-Host "📦 Building Gateway Docker image..." -ForegroundColor Yellow
    
    Push-Location $gatewayPath
    try {
        docker build -t capstone-gateway:latest .
        if ($LASTEXITCODE -ne 0) {
            throw "Docker build failed"
        }
        Write-Host "✅ Gateway image built successfully" -ForegroundColor Green
    }
    finally {
        Pop-Location
    }
    Write-Host ""
}

# Deploy to Kubernetes
if (-not $SkipDeploy) {
    Write-Host "🚀 Deploying Gateway to Kubernetes..." -ForegroundColor Yellow
    
    # Apply namespace and deployment
    kubectl apply -f C:\dotnet\capstone\fda\devops\kubernetes\local\gateway.yaml
    
    if ($LASTEXITCODE -ne 0) {
        throw "Kubernetes deployment failed"
    }
    
    Write-Host "✅ Gateway deployed successfully" -ForegroundColor Green
    Write-Host ""
    
    # Wait for deployment to be ready
    Write-Host "⏳ Waiting for gateway pods to be ready..." -ForegroundColor Yellow
    kubectl wait --for=condition=ready pod -l app=gateway -n capstone-gateway --timeout=120s
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Gateway pods are ready" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Timeout waiting for pods. Check status with: kubectl get pods -n capstone-gateway" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Deployment Summary                        " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Gateway Service: http://localhost:30005" -ForegroundColor Green
Write-Host ""
Write-Host "Check status:" -ForegroundColor Cyan
Write-Host "  kubectl get all -n capstone-gateway" -ForegroundColor White
Write-Host ""
Write-Host "View logs:" -ForegroundColor Cyan
Write-Host "  kubectl logs -f deployment/gateway -n capstone-gateway" -ForegroundColor White
Write-Host ""
Write-Host "Test health:" -ForegroundColor Cyan
Write-Host "  curl http://localhost:30005/health" -ForegroundColor White
Write-Host ""
