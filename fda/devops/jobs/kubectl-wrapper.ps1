# Kubectl Wrapper Script for Rancher Desktop
# 
# Note: Rancher Desktop uses kuberlr which may fail if it tries to download
# a non-existent Kubernetes version due to firewall restrictions.
#
# Workaround: Use Docker-based deployment (docker-compose-local.yml) instead
# of Kubernetes deployment for local development.
#
# Alternative: Use rdctl shell to execute kubectl commands inside Rancher's VM
#
# Usage:
#   .\kubectl-wrapper.ps1 get pods
#   or
#   rdctl shell kubectl get pods

$kubectlPath = "C:\Program Files\Rancher Desktop\resources\resources\win32\bin\kubectl.exe"

Write-Host "⚠️  Warning: Rancher Desktop's kubectl may fail due to kuberlr version download issues." -ForegroundColor Yellow
Write-Host "   If this fails, use: rdctl shell kubectl $($args -join ' ')" -ForegroundColor Yellow
Write-Host ""

try {
    & $kubectlPath $args
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "❌ kubectl failed. Try alternative method:" -ForegroundColor Red
        Write-Host "   rdctl shell kubectl $($args -join ' ')" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Or use Docker Compose for local deployment:" -ForegroundColor Green
        Write-Host "   docker-compose -f docker-compose-local.yml up -d" -ForegroundColor Cyan
    }
} catch {
    Write-Host "Error executing kubectl: $_" -ForegroundColor Red
}
