# Deploy all services to Kubernetes
# Apply in order: namespace, secrets/config, then services

kubectl apply -f namespace.yaml
kubectl apply -f mongodb-secret.yaml
kubectl apply -f mongodb-config.yaml
kubectl apply -f mongodb.yaml
kubectl apply -f authentication.yaml
kubectl apply -f catalog.yaml
kubectl apply -f crm.yaml
kubectl apply -f cart.yaml

# Wait for deployments to be ready
kubectl wait --for=condition=available --timeout=300s deployment/mongodb-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/authentication-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/catalog-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/crm-deployment -n capstone-services
kubectl wait --for=condition=available --timeout=300s deployment/cart-deployment -n capstone-services

# Show status
kubectl get all -n capstone-services