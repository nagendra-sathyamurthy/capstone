# Kubernetes Configuration Setup Script
# Run this after enabling Kubernetes in Docker Desktop

Write-Host "`n=== Kubernetes Configuration Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if Kubernetes is running
Write-Host "Checking Kubernetes status..." -ForegroundColor Yellow
$maxAttempts = 3
$attempt = 0
$connected = $false

while ($attempt -lt $maxAttempts -and -not $connected) {
    $attempt++
    try {
        $null = kubectl cluster-info 2>&1
        if ($LASTEXITCODE -eq 0) {
            $connected = $true
            Write-Host "✓ Kubernetes is running!" -ForegroundColor Green
            break
        }
    } catch {}
    
    if ($attempt -lt $maxAttempts) {
        Write-Host "  Attempt $attempt/$maxAttempts - Waiting for Kubernetes..." -ForegroundColor Gray
        Start-Sleep -Seconds 2
    }
}

if (-not $connected) {
    Write-Host "`n✗ Kubernetes is not running" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please enable Kubernetes in Docker Desktop:" -ForegroundColor Yellow
    Write-Host "  1. Right-click Docker Desktop icon → Settings" -ForegroundColor White
    Write-Host "  2. Go to 'Kubernetes' section" -ForegroundColor White
    Write-Host "  3. Check ☑ 'Enable Kubernetes'" -ForegroundColor White
    Write-Host "  4. Click 'Apply & Restart'" -ForegroundColor White
    Write-Host "  5. Wait 2-3 minutes for startup" -ForegroundColor White
    Write-Host "  6. Run this script again" -ForegroundColor White
    Write-Host ""
    exit 1
}

# Display cluster info
Write-Host "`nCluster Information:" -ForegroundColor Cyan
kubectl cluster-info

# Set docker-desktop as current context
Write-Host "`nSetting docker-desktop as current context..." -ForegroundColor Yellow
kubectl config use-context docker-desktop 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Context set to docker-desktop" -ForegroundColor Green
}

# Display current context
Write-Host "`nCurrent context:" -ForegroundColor Cyan
kubectl config current-context

# Display nodes
Write-Host "`nCluster nodes:" -ForegroundColor Cyan
kubectl get nodes

# Display namespaces
Write-Host "`nAvailable namespaces:" -ForegroundColor Cyan
kubectl get namespaces

Write-Host "`n=== Setup Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Your Kubernetes cluster is ready!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  • Deploy all services:" -ForegroundColor White
Write-Host "    cd C:\dotnet\capstone\fda\devops\kubernetes" -ForegroundColor Gray
Write-Host "    .\deploy-full-stack-local.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "  • Or deploy individual components:" -ForegroundColor White
Write-Host "    .\build-images-local.ps1    # Build Docker images" -ForegroundColor Gray
Write-Host "    .\deploy-gateway-local.ps1   # Deploy API Gateway" -ForegroundColor Gray
Write-Host "    .\deploy-customer-app-local.ps1  # Deploy Customer App" -ForegroundColor Gray
Write-Host ""
Write-Host "Useful kubectl commands:" -ForegroundColor Yellow
Write-Host "  kubectl get pods -A              # List all pods" -ForegroundColor Gray
Write-Host "  kubectl get services -A          # List all services" -ForegroundColor Gray
Write-Host "  kubectl logs <pod-name> -n <namespace>  # View pod logs" -ForegroundColor Gray
Write-Host "  kubectl describe pod <pod-name> -n <namespace>  # Pod details" -ForegroundColor Gray
Write-Host ""
