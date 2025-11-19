# Kubectl Wrapper Script
# 
# This script uses the working kubectl binary from Intel folder
# If kubectl is not in PATH, it will use the direct path
#
# Usage:
#   .\kubectl-wrapper.ps1 get pods

# Try to use kubectl from PATH first (should be Intel version now)
$kubectlInPath = Get-Command kubectl -ErrorAction SilentlyContinue

if ($kubectlInPath -and $kubectlInPath.Source -notlike "*Rancher*") {
    # kubectl is in PATH and it's not Rancher's kuberlr
    kubectl $args
} else {
    # Use the Intel kubectl directly
    $kubectlPath = "C:\Intel\Kubernetes KubeCTL 1.33.4\kubectl.exe"
    
    if (Test-Path $kubectlPath) {
        & $kubectlPath $args
    } else {
        Write-Host "❌ Error: kubectl not found" -ForegroundColor Red
        Write-Host ""
        Write-Host "Expected location: $kubectlPath" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Alternatives:" -ForegroundColor Yellow
        Write-Host "  1. Add C:\Intel\Kubernetes KubeCTL 1.33.4 to your PATH" -ForegroundColor Cyan
        Write-Host "  2. Use Docker Compose: docker-compose -f docker-compose-local.yml up -d" -ForegroundColor Cyan
        Write-Host "  3. Use rdctl shell: rdctl shell kubectl get pods" -ForegroundColor Cyan
        exit 1
    }
}
