#!/bin/bash
# Complete Kubernetes Cleanup Script for Capstone Project
# Removes all capstone-related Kubernetes resources and Docker images

set +e  # Don't exit on error

DELETE_IMAGES=false
FORCE=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --delete-images|-d)
            DELETE_IMAGES=true
            shift
            ;;
        --force|-f)
            FORCE=true
            shift
            ;;
        *)
            shift
            ;;
    esac
done

echo ""
echo "========================================"
echo "Capstone Kubernetes Complete Cleanup"
echo "========================================"
echo ""

if [ "$FORCE" != "true" ]; then
    echo "This will delete all capstone Kubernetes resources."
    echo "Press Ctrl+C to cancel or Enter to continue..."
    read
    echo ""
fi

# Step 1: Delete deployments and services in capstone-services namespace
echo "Step 1: Removing application deployments and services..."
SERVICES=("authentication" "catalog" "crm" "cart" "order" "gateway" "customer-app")

for service in "${SERVICES[@]}"; do
    echo -n "  Removing $service..."
    
    # Try to delete using manifest file
    MANIFEST_PATH="../../kubernetes/local/${service}.yaml"
    if [ -f "$MANIFEST_PATH" ]; then
        kubectl delete -f "$MANIFEST_PATH" -n capstone-services --ignore-not-found=true &>/dev/null
    fi
    
    # Also try direct deletion
    kubectl delete deployment "${service}-deployment" -n capstone-services --ignore-not-found=true &>/dev/null
    kubectl delete service "${service}-service" -n capstone-services --ignore-not-found=true &>/dev/null
    
    echo " ✓"
done

# Step 2: Delete MongoDB resources
echo ""
echo "Step 2: Removing MongoDB infrastructure..."
echo -n "  Removing MongoDB..."

kubectl delete -f ../../kubernetes/local/mongodb.yaml -n capstone-services --ignore-not-found=true &>/dev/null
kubectl delete deployment mongodb-deployment -n capstone-services --ignore-not-found=true &>/dev/null
kubectl delete service mongodb-service -n capstone-services --ignore-not-found=true &>/dev/null

echo " ✓"

# Step 3: Delete ConfigMaps and Secrets
echo ""
echo "Step 3: Removing ConfigMaps and Secrets..."
echo -n "  Removing configurations..."

kubectl delete -f ../../kubernetes/local/mongodb-secret.yaml -n capstone-services --ignore-not-found=true &>/dev/null
kubectl delete -f ../../kubernetes/local/mongodb-config.yaml -n capstone-services --ignore-not-found=true &>/dev/null
kubectl delete secret mongodb-secret -n capstone-services --ignore-not-found=true &>/dev/null
kubectl delete configmap mongodb-config -n capstone-services --ignore-not-found=true &>/dev/null

echo " ✓"

# Step 4: Delete Persistent Volume Claims
echo ""
echo "Step 4: Removing Persistent Volume Claims..."
PVCS=$(kubectl get pvc -n capstone-services --no-headers 2>&1 | awk '{print $1}')

if [ -n "$PVCS" ] && [ "$PVCS" != "No" ]; then
    for pvc in $PVCS; do
        echo -n "  Removing PVC: $pvc..."
        kubectl delete pvc "$pvc" -n capstone-services --ignore-not-found=true &>/dev/null
        echo " ✓"
    done
else
    echo "  No PVCs found"
fi

# Step 5: Delete namespace
echo ""
echo "Step 5: Removing namespaces..."
echo -n "  Removing capstone-services namespace..."
kubectl delete namespace capstone-services --ignore-not-found=true &>/dev/null
echo " ✓"

echo -n "  Removing old namespaces (if any)..."
kubectl delete namespace capstone-gateway --ignore-not-found=true &>/dev/null
kubectl delete namespace capstone-frontend --ignore-not-found=true &>/dev/null
echo " ✓"

# Wait for namespace to be fully deleted
TIMEOUT=30
ELAPSED=0
while kubectl get namespace capstone-services &>/dev/null && [ $ELAPSED -lt $TIMEOUT ]; do
    sleep 1
    ELAPSED=$((ELAPSED + 1))
done

echo ""

# Step 6: Clean up Docker images (optional)
if [ "$DELETE_IMAGES" = true ]; then
    echo ""
    echo "Step 6: Removing Docker images..."
    
    IMAGES=(
        "services-authentication"
        "services-catalog"
        "services-crm"
        "services-cart"
        "services-order"
        "gateway"
        "customer-app"
    )
    
    for image in "${IMAGES[@]}"; do
        if docker images --format "{{.Repository}}" | grep -q "^${image}$"; then
            echo -n "  Removing image: $image..."
            docker rmi "${image}:latest" -f &>/dev/null
            echo " ✓"
        fi
    done
    
    echo ""
    echo "  Running Docker system prune..."
    docker system prune -f &>/dev/null
    echo "  ✓ Docker cleanup complete"
fi

# Step 7: Verify cleanup
echo ""
echo "Step 7: Verifying cleanup..."

REMAINING_PODS=$(kubectl get pods -n capstone-services 2>&1)
if echo "$REMAINING_PODS" | grep -q "No resources found\|NotFound"; then
    echo "  ✓ No pods remaining"
else
    echo "  ⚠ Some pods may still be terminating"
fi

REMAINING_SERVICES=$(kubectl get services -n capstone-services 2>&1)
if echo "$REMAINING_SERVICES" | grep -q "No resources found\|NotFound"; then
    echo "  ✓ No services remaining"
else
    echo "  ⚠ Some services may still exist"
fi

REMAINING_DEPLOYMENTS=$(kubectl get deployments -n capstone-services 2>&1)
if echo "$REMAINING_DEPLOYMENTS" | grep -q "No resources found\|NotFound"; then
    echo "  ✓ No deployments remaining"
else
    echo "  ⚠ Some deployments may still exist"
fi

# Summary
echo ""
echo "========================================"
echo "✅ Cleanup Complete!"
echo "========================================"
echo ""

if [ "$DELETE_IMAGES" = true ]; then
    echo "Removed:"
    echo "  • All Kubernetes deployments and services"
    echo "  • All ConfigMaps and Secrets"
    echo "  • All Persistent Volume Claims"
    echo "  • Namespace: capstone-services"
    echo "  • All Docker images"
else
    echo "Removed:"
    echo "  • All Kubernetes deployments and services"
    echo "  • All ConfigMaps and Secrets"
    echo "  • All Persistent Volume Claims"
    echo "  • Namespace: capstone-services"
    echo ""
    echo "Note: Docker images were NOT deleted."
    echo "      Use --delete-images flag to remove images as well."
fi

echo ""
echo "To redeploy, run:"
echo "  ./deploy.sh"
echo ""
