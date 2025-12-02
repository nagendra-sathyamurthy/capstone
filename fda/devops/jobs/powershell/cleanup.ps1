# Complete Kubernetes Cleanup Script for Capstone Project
# Removes all capstone-related Kubernetes resources and Docker images

param(
    [switch]$DeleteImages = $false,
    [switch]$Force = $false
)

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Capstone Kubernetes Complete Cleanup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not $Force) {
    Write-Host "This will delete all capstone Kubernetes resources." -ForegroundColor Yellow
    Write-Host "Press Ctrl+C to cancel or any key to continue..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Write-Host ""
}

# Function to safely delete Kubernetes resources
function Remove-K8sResource {
    param(
        [string]$ResourceType,
        [string]$Name,
        [string]$Namespace = ""
    )
    
    $nsArg = if ($Namespace) { "-n $Namespace" } else { "" }
    $cmd = "kubectl delete $ResourceType $Name $nsArg --ignore-not-found=true 2>&1"
    
    try {
        Invoke-Expression $cmd | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Step 1: Delete deployments and services in capstone-services namespace
Write-Host "Step 1: Removing application deployments and services..." -ForegroundColor Yellow
$services = @("authentication", "catalog", "crm", "cart", "order", "gateway", "customer-app")

foreach ($service in $services) {
    Write-Host "  Removing $service..." -NoNewline -ForegroundColor Gray
    
    # Try to delete using manifest file
    $manifestPath = "..\kubernetes\local\$service.yaml"
    if (Test-Path $manifestPath) {
        kubectl delete -f $manifestPath -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
    }
    
    # Also try direct deletion
    kubectl delete deployment "$service-deployment" -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
    kubectl delete service "$service-service" -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
    
    Write-Host " ✓" -ForegroundColor Green
}

# Step 2: Delete MongoDB resources
Write-Host "`nStep 2: Removing MongoDB infrastructure..." -ForegroundColor Yellow
Write-Host "  Removing MongoDB..." -NoNewline -ForegroundColor Gray

kubectl delete -f ..\kubernetes\local\mongodb.yaml -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
kubectl delete deployment mongodb-deployment -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
kubectl delete service mongodb-service -n capstone-services --ignore-not-found=true 2>&1 | Out-Null

Write-Host " ✓" -ForegroundColor Green

# Step 3: Delete ConfigMaps and Secrets
Write-Host "`nStep 3: Removing ConfigMaps and Secrets..." -ForegroundColor Yellow
Write-Host "  Removing configurations..." -NoNewline -ForegroundColor Gray

kubectl delete -f ..\kubernetes\local\mongodb-secret.yaml -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
kubectl delete -f ..\kubernetes\local\mongodb-config.yaml -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
kubectl delete secret mongodb-secret -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
kubectl delete configmap mongodb-config -n capstone-services --ignore-not-found=true 2>&1 | Out-Null

Write-Host " ✓" -ForegroundColor Green

# Step 4: Delete Persistent Volume Claims
Write-Host "`nStep 4: Removing Persistent Volume Claims..." -ForegroundColor Yellow
$pvcs = kubectl get pvc -n capstone-services --no-headers 2>&1 | ForEach-Object { ($_ -split '\s+')[0] }

if ($pvcs) {
    foreach ($pvc in $pvcs) {
        if ($pvc -and $pvc -ne "") {
            Write-Host "  Removing PVC: $pvc..." -NoNewline -ForegroundColor Gray
            kubectl delete pvc $pvc -n capstone-services --ignore-not-found=true 2>&1 | Out-Null
            Write-Host " ✓" -ForegroundColor Green
        }
    }
} else {
    Write-Host "  No PVCs found" -ForegroundColor Gray
}

# Step 5: Delete namespace
Write-Host "`nStep 5: Removing namespaces..." -ForegroundColor Yellow
Write-Host "  Removing capstone-services namespace..." -NoNewline -ForegroundColor Gray
kubectl delete namespace capstone-services --ignore-not-found=true 2>&1 | Out-Null
Write-Host " ✓" -ForegroundColor Green

Write-Host "  Removing old namespaces (if any)..." -NoNewline -ForegroundColor Gray
kubectl delete namespace capstone-gateway --ignore-not-found=true 2>&1 | Out-Null
kubectl delete namespace capstone-frontend --ignore-not-found=true 2>&1 | Out-Null
Write-Host " ✓" -ForegroundColor Green

# Wait for namespace to be fully deleted
$timeout = 30
$elapsed = 0
while ((kubectl get namespace capstone-services 2>&1) -notmatch "NotFound" -and $elapsed -lt $timeout) {
    Start-Sleep -Seconds 1
    $elapsed++
}

Write-Host ""

# Step 6: Clean up Docker images (optional)
if ($DeleteImages) {
    Write-Host "`nStep 6: Removing Docker images..." -ForegroundColor Yellow
    
    $images = @(
        "services-authentication",
        "services-catalog", 
        "services-crm",
        "services-cart",
        "services-order",
        "gateway",
        "customer-app"
    )
    
    foreach ($image in $images) {
        $imageExists = docker images --format "{{.Repository}}" | Select-String -Pattern "^$image$" -Quiet
        if ($imageExists) {
            Write-Host "  Removing image: $image..." -NoNewline -ForegroundColor Gray
            docker rmi "${image}:latest" -f 2>&1 | Out-Null
            Write-Host " ✓" -ForegroundColor Green
        }
    }
    
    Write-Host "`n  Running Docker system prune..." -ForegroundColor Gray
    docker system prune -f 2>&1 | Out-Null
    Write-Host "  ✓ Docker cleanup complete" -ForegroundColor Green
}

# Step 7: Verify cleanup
Write-Host "`nStep 7: Verifying cleanup..." -ForegroundColor Yellow

$remainingPods = kubectl get pods -n capstone-services 2>&1
$remainingServices = kubectl get services -n capstone-services 2>&1
$remainingDeployments = kubectl get deployments -n capstone-services 2>&1

if ($remainingPods -match "No resources found" -or $remainingPods -match "NotFound") {
    Write-Host "  ✓ No pods remaining" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Some pods may still be terminating" -ForegroundColor Yellow
}

if ($remainingServices -match "No resources found" -or $remainingServices -match "NotFound") {
    Write-Host "  ✓ No services remaining" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Some services may still exist" -ForegroundColor Yellow
}

if ($remainingDeployments -match "No resources found" -or $remainingDeployments -match "NotFound") {
    Write-Host "  ✓ No deployments remaining" -ForegroundColor Green
} else {
    Write-Host "  ⚠ Some deployments may still exist" -ForegroundColor Yellow
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Cleanup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($DeleteImages) {
    Write-Host "Removed:" -ForegroundColor White
    Write-Host "  • All Kubernetes deployments and services" -ForegroundColor Gray
    Write-Host "  • All ConfigMaps and Secrets" -ForegroundColor Gray
    Write-Host "  • All Persistent Volume Claims" -ForegroundColor Gray
    Write-Host "  • Namespace: capstone-services" -ForegroundColor Gray
    Write-Host "  • All Docker images" -ForegroundColor Gray
} else {
    Write-Host "Removed:" -ForegroundColor White
    Write-Host "  • All Kubernetes deployments and services" -ForegroundColor Gray
    Write-Host "  • All ConfigMaps and Secrets" -ForegroundColor Gray
    Write-Host "  • All Persistent Volume Claims" -ForegroundColor Gray
    Write-Host "  • Namespace: capstone-services" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Note: Docker images were NOT deleted." -ForegroundColor Yellow
    Write-Host "      Use -DeleteImages flag to remove images as well." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "To redeploy, run:" -ForegroundColor White
Write-Host "  .\deploy-local-k8s.ps1" -ForegroundColor Cyan
Write-Host ""
