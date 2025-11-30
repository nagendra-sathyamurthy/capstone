# Kubernetes Setup Verification Script
# Run this after enabling Kubernetes in Docker Desktop

Write-Host "`n=== Kubernetes Configuration Check ===" -ForegroundColor Cyan

# Check kubectl installation
Write-Host "`n1. Checking kubectl..." -ForegroundColor Yellow
try {
    $kubectlVersion = kubectl version --client --output=json 2>$null | ConvertFrom-Json
    Write-Host "   ✓ kubectl installed: v$($kubectlVersion.clientVersion.gitVersion)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ kubectl not found or not in PATH" -ForegroundColor Red
    exit 1
}

# Check kubeconfig
Write-Host "`n2. Checking kubeconfig..." -ForegroundColor Yellow
$kubeconfigPath = "$env:USERPROFILE\.kube\config"
if (Test-Path $kubeconfigPath) {
    Write-Host "   ✓ Kubeconfig exists at: $kubeconfigPath" -ForegroundColor Green
} else {
    Write-Host "   ✗ Kubeconfig not found" -ForegroundColor Red
    Write-Host "   → Enable Kubernetes in Docker Desktop to create it" -ForegroundColor Yellow
    exit 1
}

# Check cluster connection
Write-Host "`n3. Checking cluster connection..." -ForegroundColor Yellow
try {
    $clusterInfo = kubectl cluster-info 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Connected to Kubernetes cluster" -ForegroundColor Green
        kubectl cluster-info | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }
    } else {
        Write-Host "   ✗ Cannot connect to cluster" -ForegroundColor Red
        Write-Host "   → Make sure Kubernetes is enabled and running in Docker Desktop" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "   ✗ Error connecting to cluster" -ForegroundColor Red
    exit 1
}

# Check current context
Write-Host "`n4. Checking current context..." -ForegroundColor Yellow
$currentContext = kubectl config current-context 2>$null
if ($currentContext) {
    Write-Host "   ✓ Current context: $currentContext" -ForegroundColor Green
} else {
    Write-Host "   ✗ No context set" -ForegroundColor Red
}

# List available contexts
Write-Host "`n5. Available contexts:" -ForegroundColor Yellow
kubectl config get-contexts | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }

# Check nodes
Write-Host "`n6. Checking cluster nodes..." -ForegroundColor Yellow
try {
    $nodes = kubectl get nodes 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Cluster nodes:" -ForegroundColor Green
        kubectl get nodes | ForEach-Object { Write-Host "     $_" -ForegroundColor Gray }
    } else {
        Write-Host "   ✗ Cannot list nodes" -ForegroundColor Red
    }
} catch {
    Write-Host "   ✗ Error checking nodes" -ForegroundColor Red
}

# Check namespaces
Write-Host "`n7. Checking namespaces..." -ForegroundColor Yellow
try {
    $namespaces = kubectl get namespaces -o json 2>$null | ConvertFrom-Json
    if ($namespaces.items.Count -gt 0) {
        Write-Host "   ✓ Available namespaces:" -ForegroundColor Green
        $namespaces.items | ForEach-Object { Write-Host "     - $($_.metadata.name)" -ForegroundColor Gray }
    }
} catch {
    Write-Host "   ✗ Cannot list namespaces" -ForegroundColor Red
}

Write-Host "`n=== Configuration Check Complete ===" -ForegroundColor Cyan
Write-Host "`nYour Kubernetes cluster is ready for deployment!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  • Deploy services: cd fda\devops\kubernetes; .\deploy-full-stack-local.ps1" -ForegroundColor White
Write-Host "  • Check pods: kubectl get pods -A" -ForegroundColor White
Write-Host "  • View logs: kubectl logs -n <namespace> <pod-name>" -ForegroundColor White
Write-Host ""
