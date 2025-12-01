# Build Docker Images for Local Kubernetes Deployment
# Uses nerdctl (Rancher Desktop's container runtime)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Docker Images for Kubernetes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"
$rootPath = "C:\dotnet\capstone\fda"

# Use Docker from Rancher Desktop
$containerCmd = "docker"
if (-not (Get-Command $containerCmd -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Docker is not available. Make sure Rancher Desktop is running." -ForegroundColor Red
    exit 1
}

Write-Host "Step 1: Building Backend Services..." -ForegroundColor Yellow
Write-Host ""

# Authentication Service
Write-Host "  Building Authentication Service..." -ForegroundColor Gray
Set-Location "$rootPath\src\services"
& $containerCmd build -t services-authentication:latest -f authentication/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build authentication service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ Authentication Service built" -ForegroundColor Green

# Catalog Service
Write-Host "  Building Catalog Service..." -ForegroundColor Gray
& $containerCmd build -t services-catalog:latest -f catalog/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build catalog service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ Catalog Service built" -ForegroundColor Green

# CRM Service
Write-Host "  Building CRM Service..." -ForegroundColor Gray
& $containerCmd build -t services-crm:latest -f crm/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build crm service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ CRM Service built" -ForegroundColor Green

# Cart Service
Write-Host "  Building Cart Service..." -ForegroundColor Gray
& $containerCmd build -t services-cart:latest -f cart/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build cart service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ Cart Service built" -ForegroundColor Green

# Order Service
Write-Host "  Building Order Service..." -ForegroundColor Gray
& $containerCmd build -t services-order:latest -f order/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build order service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ Order Service built" -ForegroundColor Green

# Payment Service
Write-Host "  Building Payment Service..." -ForegroundColor Gray
& $containerCmd build -t services-payment:latest -f payment/Dockerfile .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build payment service" -ForegroundColor Red; exit 1 }
Write-Host "  ✓ Payment Service built" -ForegroundColor Green

Write-Host ""
Write-Host "Step 2: Building Gateway..." -ForegroundColor Yellow
Set-Location "$rootPath\src\gateway"
& $containerCmd build -t gateway:latest .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build gateway" -ForegroundColor Red; exit 1 }
Write-Host "✓ Gateway built" -ForegroundColor Green

Write-Host ""
Write-Host "Step 3: Building Customer App (Frontend)..." -ForegroundColor Yellow
Set-Location "$rootPath\src\customer-app"
& $containerCmd build -t customer-app:latest --build-arg REACT_APP_GATEWAY_URL=http://localhost:30005 .
if ($LASTEXITCODE -ne 0) { Write-Host "Failed to build customer app" -ForegroundColor Red; exit 1 }
Write-Host "✓ Customer App built" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Images built:" -ForegroundColor White
& $containerCmd images | Select-String -Pattern "services-|gateway|customer-app"
Write-Host ""
Write-Host "Next step: Run .\deploy-full-stack-local.ps1 to deploy to Kubernetes" -ForegroundColor Yellow
