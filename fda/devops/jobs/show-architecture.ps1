# Show Current Architecture Status
Write-Host "🏗️ CAPSTONE MICROSERVICES ARCHITECTURE STATUS" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

Write-Host "`n📊 Current Deployment: LOCAL DEVELOPMENT (Shared MongoDB)" -ForegroundColor Yellow

Write-Host "`n🗄️ MongoDB Architecture:" -ForegroundColor Cyan
Write-Host "└── mongodb-service:27017 (Shared Instance)" -ForegroundColor White
Write-Host "    ├── authenticationdb (users collection)" -ForegroundColor Gray
Write-Host "    ├── catalogdb (items collection)" -ForegroundColor Gray  
Write-Host "    ├── crmdb (customers collection)" -ForegroundColor Gray
Write-Host "    └── cartdb (carts collection)" -ForegroundColor Gray

Write-Host "`n🌐 Microservices Status:" -ForegroundColor Cyan
kubectl get pods -n capstone-services -o custom-columns="NAME:.metadata.name,STATUS:.status.phase,READY:.status.containerStatuses[0].ready" 2>$null

Write-Host "`n🔗 Service Endpoints:" -ForegroundColor Cyan  
kubectl get services -n capstone-services -o custom-columns="NAME:.metadata.name,TYPE:.spec.type,PORT:.spec.ports[0].nodePort" 2>$null

Write-Host "`n📈 Resource Usage:" -ForegroundColor Cyan
Write-Host "Current Configuration: SHARED MONGODB (Resource Efficient)" -ForegroundColor Yellow
Write-Host "• Memory per service: 128Mi-256Mi" -ForegroundColor White
Write-Host "• CPU per service: 100m-250m" -ForegroundColor White  
Write-Host "• MongoDB: Single instance serving all services" -ForegroundColor White
Write-Host "• Storage: Temporary (EmptyDir)" -ForegroundColor White

Write-Host "`n🔄 Available Deployments:" -ForegroundColor Cyan
Write-Host "1. .\deploy-local-k8s.ps1   - Current (Shared MongoDB)" -ForegroundColor Green
Write-Host "2. .\deploy-production.ps1  - Dedicated MongoDB per service" -ForegroundColor Yellow

Write-Host "`n🧹 Cleanup Options:" -ForegroundColor Cyan
Write-Host ".\cleanup-environment.ps1 -Environment local" -ForegroundColor Gray
Write-Host ".\cleanup-environment.ps1 -Environment production" -ForegroundColor Gray  
Write-Host ".\cleanup-environment.ps1 -Environment all" -ForegroundColor Gray

Write-Host "`n📚 Documentation:" -ForegroundColor Cyan
Write-Host "../../docs/devops/DEPLOYMENT-STRATEGY.md - Complete deployment guide" -ForegroundColor White

Write-Host "`n✅ Architecture Benefits (Current Setup):" -ForegroundColor Green
Write-Host "• Resource Efficient - Single MongoDB for all services" -ForegroundColor White
Write-Host "• Fast Development - Quick startup and debugging" -ForegroundColor White
Write-Host "• Cost Effective - Minimal resource requirements" -ForegroundColor White  
Write-Host "• Easy Data Management - All data in one instance" -ForegroundColor White

Write-Host "`n🎯 Next Steps:" -ForegroundColor Magenta
Write-Host "1. Test APIs using Postman collections" -ForegroundColor White
Write-Host "2. Develop and debug locally with shared DB" -ForegroundColor White
Write-Host "3. Deploy to production using .\deploy-production.ps1 when ready" -ForegroundColor White