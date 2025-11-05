# Deploy Local Development Environment
# Uses shared MongoDB instance for resource efficiency
param(
    [switch]$Clean,
    [switch]$SkipBuild
)

Write-Host "🚀 Deploying Capstone Services - LOCAL DEVELOPMENT Environment" -ForegroundColor Green
Write-Host "📍 Architecture: Shared MongoDB for all services" -ForegroundColor Yellow

$LocalPath = "c:\dotnet\capstone\fda\devops\kubernetes\local"

if ($Clean) {
    Write-Host "🧹 Cleaning up existing deployments..." -ForegroundColor Yellow
    kubectl delete namespace capstone-services --ignore-not-found=true
    Start-Sleep -Seconds 10
}

Write-Host "📦 Creating namespace..." -ForegroundColor Cyan
kubectl apply -f "$LocalPath\namespace.yaml"

Write-Host "🔐 Applying secrets and config..." -ForegroundColor Cyan
kubectl apply -f "$LocalPath\mongodb-secret.yaml"
kubectl apply -f "$LocalPath\mongodb-config.yaml"

Write-Host "🗄️ Deploying shared MongoDB..." -ForegroundColor Cyan
kubectl apply -f "$LocalPath\mongodb.yaml"

Write-Host "⏳ Waiting for MongoDB to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=60s deployment/mongodb-deployment -n capstone-services

Write-Host "🌐 Deploying microservices..." -ForegroundColor Cyan
kubectl apply -f "$LocalPath\authentication.yaml"
kubectl apply -f "$LocalPath\catalog.yaml" 
kubectl apply -f "$LocalPath\crm.yaml"
kubectl apply -f "$LocalPath\cart.yaml"

Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=120s deployment/authentication-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=120s deployment/catalog-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=120s deployment/crm-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=120s deployment/cart-deployment -n capstone-services

Write-Host "`n✅ LOCAL DEVELOPMENT deployment completed!" -ForegroundColor Green
Write-Host "📊 Resource Usage: OPTIMIZED (Shared MongoDB)" -ForegroundColor Yellow

Write-Host "`n🔗 Service Endpoints:" -ForegroundColor Cyan
Write-Host "Authentication: http://localhost:30001" -ForegroundColor White
Write-Host "Catalog:       http://localhost:30002" -ForegroundColor White  
Write-Host "CRM:           http://localhost:30003" -ForegroundColor White
Write-Host "Cart:          http://localhost:30004" -ForegroundColor White
Write-Host "MongoDB:       mongodb://localhost:30000" -ForegroundColor White

Write-Host "`n📈 Check status with:" -ForegroundColor Cyan
Write-Host "kubectl get pods -n capstone-services" -ForegroundColor Gray
Write-Host "kubectl get services -n capstone-services" -ForegroundColor Gray

Write-Host "`n🔧 Port forwarding (optional):" -ForegroundColor Cyan
Write-Host ".\start-portforward.ps1" -ForegroundColor Gray