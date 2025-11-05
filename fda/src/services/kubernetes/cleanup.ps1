# Clean up all Kubernetes resources for capstone services

Write-Host "Cleaning up Capstone Services from Kubernetes..." -ForegroundColor Yellow

kubectl delete -f cart.yaml
kubectl delete -f crm.yaml
kubectl delete -f catalog.yaml
kubectl delete -f authentication.yaml
kubectl delete -f mongodb.yaml
kubectl delete -f mongodb-config.yaml
kubectl delete -f mongodb-secret.yaml
kubectl delete -f namespace.yaml

Write-Host "Cleanup complete!" -ForegroundColor Green