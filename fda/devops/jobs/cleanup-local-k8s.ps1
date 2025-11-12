# Cleanup Local Kubernetes Deployments
# Remove all local Kubernetes deployments and resources

Write-Host "=== Cleaning up Capstone Local Kubernetes Deployments ===" -ForegroundColor Red

# Remove application deployments
Write-Host "`n1. Removing application services..." -ForegroundColor Yellow
$services = @("authentication", "catalog", "crm", "cart")
foreach ($service in $services) {
    Write-Host "   Removing $service service..." -ForegroundColor Cyan
    kubectl delete -f "..\kubernetes\local\$service.yaml" -n capstone-services --ignore-not-found=true
}

# Remove MongoDB infrastructure
Write-Host "`n2. Removing MongoDB infrastructure..." -ForegroundColor Yellow
kubectl delete -f ..\kubernetes\local\mongodb.yaml -n capstone-services --ignore-not-found=true
kubectl delete -f ..\kubernetes\local\mongodb-secret.yaml -n capstone-services --ignore-not-found=true
kubectl delete -f ..\kubernetes\local\mongodb-config.yaml -n capstone-services --ignore-not-found=true

# Remove namespace (this will clean up everything)
Write-Host "`n3. Removing namespace..." -ForegroundColor Yellow
kubectl delete namespace capstone-services --ignore-not-found=true

Write-Host "`n=== Cleanup Complete ===" -ForegroundColor Green
Write-Host "All local Kubernetes resources have been removed." -ForegroundColor White