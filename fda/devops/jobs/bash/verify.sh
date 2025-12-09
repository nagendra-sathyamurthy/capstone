#!/bin/bash
# Kubernetes Setup Verification Script
# Run this after enabling Kubernetes in Docker Desktop

set -e

echo ""
echo "=== Kubernetes Configuration Check ==="
echo ""

# Check kubectl installation
echo "1. Checking kubectl..."
if command -v kubectl &> /dev/null; then
    KUBECTL_VERSION=$(kubectl version --client -o json 2>/dev/null | grep -o '"gitVersion":"[^"]*"' | cut -d'"' -f4)
    echo "   ✓ kubectl installed: $KUBECTL_VERSION"
else
    echo "   ✗ kubectl not found or not in PATH"
    exit 1
fi

# Check kubeconfig
echo ""
echo "2. Checking kubeconfig..."
KUBECONFIG_PATH="${HOME}/.kube/config"
if [ -f "$KUBECONFIG_PATH" ]; then
    echo "   ✓ Kubeconfig exists at: $KUBECONFIG_PATH"
else
    echo "   ✗ Kubeconfig not found"
    echo "   → Enable Kubernetes in Docker Desktop to create it"
    exit 1
fi

# Check cluster connection
echo ""
echo "3. Checking cluster connection..."
if kubectl cluster-info &> /dev/null; then
    echo "   ✓ Connected to Kubernetes cluster"
    kubectl cluster-info | sed 's/^/     /'
else
    echo "   ✗ Cannot connect to cluster"
    echo "   → Make sure Kubernetes is enabled and running in Docker Desktop"
    exit 1
fi

# Check current context
echo ""
echo "4. Checking current context..."
CURRENT_CONTEXT=$(kubectl config current-context 2>/dev/null || echo "")
if [ -n "$CURRENT_CONTEXT" ]; then
    echo "   ✓ Current context: $CURRENT_CONTEXT"
else
    echo "   ✗ No context set"
fi

# List available contexts
echo ""
echo "5. Available contexts:"
kubectl config get-contexts | sed 's/^/     /'

# Check nodes
echo ""
echo "6. Checking cluster nodes..."
if kubectl get nodes &> /dev/null; then
    echo "   ✓ Cluster nodes:"
    kubectl get nodes | sed 's/^/     /'
else
    echo "   ✗ Cannot list nodes"
fi

# Check namespaces
echo ""
echo "7. Checking namespaces..."
if kubectl get namespaces &> /dev/null; then
    echo "   ✓ Available namespaces:"
    kubectl get namespaces -o jsonpath='{.items[*].metadata.name}' | tr ' ' '\n' | sed 's/^/     - /'
else
    echo "   ✗ Cannot list namespaces"
fi

echo ""
echo "=== Configuration Check Complete ==="
echo ""
echo "Your Kubernetes cluster is ready for deployment!"
echo ""
echo "Next steps:"
echo "  • Deploy services: cd fda/devops/jobs && ./deploy.sh"
echo "  • Check pods: kubectl get pods -A"
echo "  • View logs: kubectl logs -n <namespace> <pod-name>"
echo ""
