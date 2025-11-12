# Kubernetes Local Deployment Strategy
# Deploy all services using local Kubernetes manifests with shared MongoDB

Write-Host "=== Capstone Services - Local Kubernetes Deployment ===" -ForegroundColor Green

# Deploy MongoDB infrastructure
Write-Host "`n1. Deploying MongoDB infrastructure..." -ForegroundColor Yellow
kubectl apply -f ..\kubernetes\local\namespace.yaml
kubectl apply -f ..\kubernetes\local\mongodb-config.yaml -n capstone-services
kubectl apply -f ..\kubernetes\local\mongodb-secret.yaml -n capstone-services
kubectl apply -f ..\kubernetes\local\mongodb.yaml -n capstone-services

Write-Host "   ✅ MongoDB deployed" -ForegroundColor Green

# Deploy application services
Write-Host "`n2. Deploying application services..." -ForegroundColor Yellow

$services = @("authentication", "catalog", "crm", "cart")
foreach ($service in $services) {
    Write-Host "   Deploying $service service..." -ForegroundColor Cyan
    kubectl apply -f "..\kubernetes\local\$service.yaml" -n capstone-services
}

Write-Host "   ✅ All application services deployed" -ForegroundColor Green

# Wait for deployments to be ready
Write-Host "`n3. Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check deployment status
Write-Host "`n4. Deployment Status:" -ForegroundColor Yellow
kubectl get pods -n capstone-services
kubectl get svc -n capstone-services

Write-Host "`n=== Deployment Complete ===" -ForegroundColor Green
Write-Host "Services are available on the following NodePorts:" -ForegroundColor White
Write-Host "  • Authentication: http://localhost:30001" -ForegroundColor Cyan
Write-Host "  • Catalog:        http://localhost:30002" -ForegroundColor Cyan  
Write-Host "  • CRM:            http://localhost:30003" -ForegroundColor Cyan
Write-Host "  • Cart:           http://localhost:30004" -ForegroundColor Cyan
Write-Host "  • MongoDB:        mongodb://localhost:30000" -ForegroundColor Cyan
Write-Host "`nNote: Order and Payment services are not yet implemented." -ForegroundColor Yellow