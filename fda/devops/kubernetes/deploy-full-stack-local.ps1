# Deploy All Services to Local Kubernetes
# This script deploys the complete Capstone application stack to local Kubernetes

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deploying Complete Stack to Kubernetes" -ForegroundColor Cyan
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

Write-Host "Step 1: Deploying Infrastructure (MongoDB, Namespace)..." -ForegroundColor Yellow
kubectl apply -f local/namespace.yaml
kubectl apply -f local/mongodb-secret.yaml
kubectl apply -f local/mongodb-config.yaml
kubectl apply -f local/mongodb.yaml
Write-Host "✓ Infrastructure deployed" -ForegroundColor Green
Write-Host ""

Write-Host "Waiting for MongoDB to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=180s deployment/mongodb-deployment -n capstone-services
Write-Host ""

Write-Host "Step 2: Deploying Backend Services..." -ForegroundColor Yellow
kubectl apply -f local/authentication.yaml
kubectl apply -f local/catalog.yaml
kubectl apply -f local/crm.yaml
kubectl apply -f local/cart.yaml
kubectl apply -f local/order.yaml
kubectl apply -f local/payment.yaml
Write-Host "✓ Backend services deployed" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Deploying Gateway..." -ForegroundColor Yellow
kubectl apply -f local/gateway.yaml
Write-Host "✓ Gateway deployed" -ForegroundColor Green
Write-Host ""

Write-Host "Step 4: Deploying Customer App (Frontend)..." -ForegroundColor Yellow
kubectl apply -f local/customer-app.yaml
Write-Host "✓ Customer App deployed" -ForegroundColor Green
Write-Host ""

Write-Host "Step 5: Waiting for all services to be ready..." -ForegroundColor Yellow
$services = @(
    @{Name="authentication-service-deployment"; Namespace="capstone-services"},
    @{Name="catalog-service-deployment"; Namespace="capstone-services"},
    @{Name="crm-service-deployment"; Namespace="capstone-services"},
    @{Name="cart-service-deployment"; Namespace="capstone-services"},
    @{Name="gateway"; Namespace="capstone-gateway"},
    @{Name="customer-app"; Namespace="capstone-frontend"}
)

foreach ($service in $services) {
    Write-Host "  Waiting for $($service.Name)..." -ForegroundColor Gray
    kubectl wait --for=condition=available --timeout=180s deployment/$($service.Name) -n $($service.Namespace) 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ $($service.Name) is ready" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ $($service.Name) is not ready yet" -ForegroundColor Yellow
    }
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access Points:" -ForegroundColor White
Write-Host "  Customer App:        http://localhost:30080" -ForegroundColor Green
Write-Host "  API Gateway:         http://localhost:30005" -ForegroundColor Green
Write-Host "  Authentication:      http://localhost:30001" -ForegroundColor Cyan
Write-Host "  Catalog Service:     http://localhost:30002" -ForegroundColor Cyan
Write-Host "  CRM Service:         http://localhost:30003" -ForegroundColor Cyan
Write-Host "  Cart Service:        http://localhost:30004" -ForegroundColor Cyan
Write-Host "  MongoDB:             http://localhost:30000" -ForegroundColor Cyan
Write-Host ""
Write-Host "Useful Commands:" -ForegroundColor Yellow
Write-Host "  View all pods:       kubectl get pods -A" -ForegroundColor White
Write-Host "  View all services:   kubectl get services -A" -ForegroundColor White
Write-Host "  View Gateway logs:   kubectl logs -n capstone-gateway -l app=gateway --tail=100 -f" -ForegroundColor White
Write-Host "  View App logs:       kubectl logs -n capstone-frontend -l app=customer-app --tail=100 -f" -ForegroundColor White
Write-Host "  Port forwarding:     .\start-portforward.ps1" -ForegroundColor White
Write-Host ""
