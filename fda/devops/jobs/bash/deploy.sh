#!/bin/bash
# Deploy All Services to Local Kubernetes
# This script deploys the complete Capstone application stack to local Kubernetes

set -e

echo "========================================"
echo "Deploying Complete Stack to Kubernetes"
echo "========================================"
echo ""

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo "Error: kubectl is not installed or not in PATH"
    exit 1
fi

# Check if Docker is running
if ! docker ps &> /dev/null; then
    echo "Error: Docker is not running"
    exit 1
fi

# Get the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "Step 0: Cleaning up old namespaces (if any)..."
kubectl delete namespace capstone-gateway --ignore-not-found=true 2>/dev/null || true
kubectl delete namespace capstone-frontend --ignore-not-found=true 2>/dev/null || true
echo "✓ Old namespaces cleaned up"
echo ""

echo "Step 1: Deploying Infrastructure (MongoDB, Namespace)..."
kubectl apply -f ../../kubernetes/local/namespace.yaml
kubectl apply -f ../../kubernetes/local/mongodb-secret.yaml
kubectl apply -f ../../kubernetes/local/mongodb-config.yaml
kubectl apply -f ../../kubernetes/local/mongodb.yaml
echo "✓ Infrastructure deployed"
echo ""

echo "Waiting for MongoDB to be ready..."
kubectl wait --for=condition=available --timeout=180s deployment/mongodb-deployment -n capstone-services
echo ""

echo "Step 2: Deploying Backend Services..."
kubectl apply -f ../../kubernetes/local/authentication.yaml
kubectl apply -f ../../kubernetes/local/catalog.yaml
kubectl apply -f ../../kubernetes/local/crm.yaml
kubectl apply -f ../../kubernetes/local/cart.yaml
kubectl apply -f ../../kubernetes/local/order.yaml
echo "✓ Backend services deployed"
echo ""

echo "Step 3: Deploying Gateway..."
kubectl apply -f ../../kubernetes/local/gateway.yaml
echo "✓ Gateway deployed"
echo ""

echo "Step 4: Deploying Customer App (Frontend)..."
kubectl apply -f ../../kubernetes/local/customer-app.yaml
echo "✓ Customer App deployed"
echo ""

echo "Step 5: Waiting for all services to be ready..."
SERVICES=(
    "authentication-deployment:capstone-services"
    "catalog-deployment:capstone-services"
    "crm-deployment:capstone-services"
    "cart-deployment:capstone-services"
    "order-deployment:capstone-services"
    "gateway:capstone-services"
    "customer-app:capstone-services"
)

for service in "${SERVICES[@]}"; do
    IFS=':' read -r name namespace <<< "$service"
    echo "  Waiting for $name..."
    if kubectl wait --for=condition=available --timeout=180s deployment/$name -n $namespace 2>/dev/null; then
        echo "  ✓ $name is ready"
    else
        echo "  ⚠ $name is not ready yet"
    fi
done
echo ""

echo "========================================"
echo "Deployment Summary"
echo "========================================"
echo ""
echo "Access Points:"
echo "  Customer App:        http://localhost:30080"
echo "  API Gateway:         http://localhost:30500"
echo "  Authentication:      http://localhost:30001"
echo "  Catalog Service:     http://localhost:30002"
echo "  CRM Service:         http://localhost:30003"
echo "  Cart Service:        http://localhost:30004"
echo "  MongoDB:             http://localhost:30000"
echo ""
echo "Namespace: capstone-services (all services in single namespace)"
echo ""
echo "Useful Commands:"
echo "  View all pods:       kubectl get pods -n capstone-services"
echo "  View all services:   kubectl get services -n capstone-services"
echo "  View Gateway logs:   kubectl logs -n capstone-services -l app=gateway --tail=100 -f"
echo "  View App logs:       kubectl logs -n capstone-services -l app=customer-app --tail=100 -f"
echo "  View All logs:       kubectl logs -n capstone-services -l app --tail=50"
echo ""
