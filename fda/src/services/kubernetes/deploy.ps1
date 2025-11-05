# Deploy all services to Kubernetes
# Apply in order: namespace, secrets/config, then services

Write-Host "Deploying Capstone Services to Kubernetes..." -ForegroundColor Green

kubectl apply -f namespace.yaml
kubectl apply -f mongodb-secret.yaml
kubectl apply -f mongodb-config.yaml
kubectl apply -f mongodb.yaml
kubectl apply -f authentication.yaml
kubectl apply -f catalog.yaml
kubectl apply -f crm.yaml
kubectl apply -f cart.yaml

Write-Host "Waiting for deployments to be ready..." -ForegroundColor Yellow

# Wait for deployments to be ready
kubectl wait --for=condition=available --timeout=300s deployment/mongodb-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/authentication-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/catalog-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/crm-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/cart-deployment -n capstone-services

Write-Host "Deployment complete! Showing status:" -ForegroundColor Green

# Show status
kubectl get all -n capstone-services

Write-Host "`nServices deployed successfully!" -ForegroundColor Green
Write-Host "`nAccess via NodePort (default):" -ForegroundColor Cyan
Write-Host "MongoDB:        localhost:30000" -ForegroundColor White
Write-Host "Authentication: http://localhost:30001" -ForegroundColor White
Write-Host "Catalog:        http://localhost:30002" -ForegroundColor White
Write-Host "CRM:            http://localhost:30003" -ForegroundColor White
Write-Host "Cart:           http://localhost:30004" -ForegroundColor White

Write-Host "`nTo access on Docker Compose ports:" -ForegroundColor Cyan
Write-Host "Run: .\start-portforward.ps1" -ForegroundColor Yellow
Write-Host "Then access on: 9000, 5001, 5002, 5003, 5004" -ForegroundColor White