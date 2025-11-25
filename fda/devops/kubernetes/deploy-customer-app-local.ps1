# Deploy Customer App to Local Kubernetes
# This script deploys the customer-app frontend application to local Kubernetes cluster

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deploying Customer App to Kubernetes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if kubectl is available
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "Error: kubectl is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Check if Docker is running
$dockerRunning = docker ps 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Docker is not running" -ForegroundColor Red
    exit 1
}

# Set the location to the script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "Step 1: Building Customer App Docker Image..." -ForegroundColor Yellow
Set-Location "..\..\src\customer-app"
docker build -t capstone-customer-app:latest .
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to build customer-app Docker image" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Customer App image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Applying Kubernetes Configuration..." -ForegroundColor Yellow
Set-Location "$scriptPath\local"
kubectl apply -f customer-app.yaml
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to apply Kubernetes configuration" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Kubernetes configuration applied successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Waiting for deployment to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=300s deployment/customer-app -n capstone-frontend
if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Deployment did not become ready within timeout" -ForegroundColor Yellow
} else {
    Write-Host "✓ Deployment is ready" -ForegroundColor Green
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Customer App Deployment Information" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Namespace: capstone-frontend" -ForegroundColor White
Write-Host "Service: customer-app-service" -ForegroundColor White
Write-Host "NodePort: 30080" -ForegroundColor White
Write-Host ""
Write-Host "Access the application at: http://localhost:30080" -ForegroundColor Green
Write-Host ""
Write-Host "To view pods:" -ForegroundColor Yellow
Write-Host "  kubectl get pods -n capstone-frontend" -ForegroundColor White
Write-Host ""
Write-Host "To view logs:" -ForegroundColor Yellow
Write-Host "  kubectl logs -n capstone-frontend -l app=customer-app --tail=100 -f" -ForegroundColor White
Write-Host ""
Write-Host "To delete deployment:" -ForegroundColor Yellow
Write-Host "  kubectl delete -f local/customer-app.yaml" -ForegroundColor White
Write-Host ""
