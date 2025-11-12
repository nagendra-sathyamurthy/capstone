# Deploy Production Environment
# Uses dedicated MongoDB instance per service for isolation and scalability
param(
    [switch]$Clean,
    [string]$Environment = "production"
)

Write-Host "🚀 Deploying Capstone Services - PRODUCTION Environment" -ForegroundColor Green
Write-Host "📍 Architecture: Dedicated MongoDB per service" -ForegroundColor Yellow

$ProductionPath = "..\kubernetes\production"

if ($Clean) {
    Write-Host "🧹 Cleaning up existing deployments..." -ForegroundColor Yellow
    kubectl delete namespace capstone-services --ignore-not-found=true
    Start-Sleep -Seconds 15
}

Write-Host "📦 Creating namespace..." -ForegroundColor Cyan
kubectl apply -f "$ProductionPath\namespace.yaml"

Write-Host "🔐 Applying production secrets..." -ForegroundColor Cyan
kubectl apply -f "$ProductionPath\mongodb-secrets.yaml"

Write-Host "🗄️ Deploying Authentication service with dedicated MongoDB..." -ForegroundColor Cyan
kubectl apply -f "$ProductionPath\authentication.yaml"

Write-Host "🗄️ Deploying Catalog service with dedicated MongoDB..." -ForegroundColor Cyan  
kubectl apply -f "$ProductionPath\catalog.yaml"

Write-Host "🗄️ Deploying CRM service with dedicated MongoDB..." -ForegroundColor Cyan
kubectl apply -f "$ProductionPath\crm.yaml"

Write-Host "🗄️ Deploying Cart service with dedicated MongoDB..." -ForegroundColor Cyan
kubectl apply -f "$ProductionPath\cart.yaml"

Write-Host "⏳ Waiting for MongoDB instances to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=180s deployment/auth-mongodb-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=180s deployment/catalog-mongodb-deployment -n capstone-services  
kubectl wait --for=condition=available --timeout=180s deployment/crm-mongodb-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=180s deployment/cart-mongodb-deployment -n capstone-services

Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=180s deployment/authentication-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=180s deployment/catalog-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=180s deployment/crm-deployment -n capstone-services  
kubectl wait --for=condition=available --timeout=180s deployment/cart-deployment -n capstone-services

Write-Host "`n✅ PRODUCTION deployment completed!" -ForegroundColor Green
Write-Host "📊 Resource Usage: HIGH AVAILABILITY (Dedicated MongoDB per service)" -ForegroundColor Yellow

Write-Host "`n🔗 Service Endpoints (LoadBalancer):" -ForegroundColor Cyan
Write-Host "Authentication: http://<external-ip>:5001" -ForegroundColor White
Write-Host "Catalog:       http://<external-ip>:5002" -ForegroundColor White
Write-Host "CRM:           http://<external-ip>:5003" -ForegroundColor White  
Write-Host "Cart:          http://<external-ip>:5004" -ForegroundColor White

Write-Host "`n📈 Check status with:" -ForegroundColor Cyan
Write-Host "kubectl get pods -n capstone-services" -ForegroundColor Gray
Write-Host "kubectl get services -n capstone-services" -ForegroundColor Gray
Write-Host "kubectl get pvc -n capstone-services" -ForegroundColor Gray

Write-Host "`n🔍 Get external IPs:" -ForegroundColor Cyan  
Write-Host "kubectl get services -n capstone-services -o wide" -ForegroundColor Gray

Write-Host "`n⚠️  Note: This is a PRODUCTION deployment with:" -ForegroundColor Red
Write-Host "   • Dedicated MongoDB per service" -ForegroundColor White
Write-Host "   • Persistent storage (PVCs)" -ForegroundColor White
Write-Host "   • Higher resource allocation" -ForegroundColor White
Write-Host "   • LoadBalancer services" -ForegroundColor White
Write-Host "   • Health checks enabled" -ForegroundColor White