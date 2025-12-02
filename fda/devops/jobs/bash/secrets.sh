#!/bin/bash
# Apply MongoDB Secrets to Kubernetes
# This script applies the updated MongoDB secrets configuration to the capstone-services namespace

set -e

echo "Applying MongoDB secrets to Kubernetes..."

# Check if kubectl is available and cluster is accessible
if ! CONTEXT=$(kubectl config current-context 2>/dev/null); then
    echo "✗ kubectl not found or cluster not accessible"
    exit 1
fi
echo "✓ Kubernetes context: $CONTEXT"

# Check if namespace exists
NAMESPACE="capstone-services"
if kubectl get namespace $NAMESPACE &>/dev/null; then
    echo "✓ Namespace '$NAMESPACE' exists"
else
    echo "Creating namespace '$NAMESPACE'..."
    kubectl create namespace $NAMESPACE
fi

# Get script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Apply secrets
SECRETS_PATH="$SCRIPT_DIR/../../kubernetes/local/mongodb-secret.yaml"

echo "Applying MongoDB secrets..."
kubectl apply -f "$SECRETS_PATH"

echo "✓ MongoDB secrets applied successfully"

# Verify secrets
echo ""
echo "Verifying applied secrets:"
kubectl get secrets -n $NAMESPACE | grep "mongodb"

echo ""
echo "Secret details:"
echo "- mongodb-secret"
if kubectl get secret mongodb-secret -n $NAMESPACE &>/dev/null; then
    KEYS=$(kubectl get secret mongodb-secret -n $NAMESPACE -o jsonpath='{.data}' 2>/dev/null | grep -o '"[^"]*"' | tr -d '"' | tr '\n' ',' | sed 's/,$//')
    echo "  Keys: $KEYS"
else
    echo "  Not found"
fi

echo ""
echo "✓ MongoDB secrets configuration complete!"
echo "Services can now access MongoDB credentials securely through Kubernetes secrets."

echo ""
echo "To restart deployments and pick up new secrets:"
echo "kubectl rollout restart deployment -n $NAMESPACE"
