#!/bin/bash
# Build Docker Images for Local Kubernetes Deployment

set -e

echo "========================================"
echo "Building Docker Images for Kubernetes"
echo "========================================"
echo ""

ROOT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    echo "Error: Docker is not available. Make sure Docker is running."
    exit 1
fi

echo "Step 1: Building Backend Services..."
echo ""

# Navigate to services directory
cd "$ROOT_PATH/src/services"

# Authentication Service
echo "  Building Authentication Service..."
docker build -t services-authentication:latest -f authentication/Dockerfile .
echo "  ✓ Authentication Service built"

# Catalog Service
echo "  Building Catalog Service..."
docker build -t services-catalog:latest -f catalog/Dockerfile .
echo "  ✓ Catalog Service built"

# CRM Service
echo "  Building CRM Service..."
docker build -t services-crm:latest -f crm/Dockerfile .
echo "  ✓ CRM Service built"

# Cart Service
echo "  Building Cart Service..."
docker build -t services-cart:latest -f cart/Dockerfile .
echo "  ✓ Cart Service built"

# Order Service
echo "  Building Order Service..."
docker build -t services-order:latest -f order/Dockerfile .
echo "  ✓ Order Service built"

echo ""
echo "Step 2: Building Gateway..."
cd "$ROOT_PATH/src/gateway"
docker build -t gateway:latest .
echo "✓ Gateway built"

echo ""
echo "Step 3: Building Customer App (Frontend)..."
cd "$ROOT_PATH/src/customer-app"
docker build -t customer-app:latest --build-arg REACT_APP_GATEWAY_URL=http://localhost:30005 .
echo "✓ Customer App built"

echo ""
echo "========================================"
echo "Build Complete!"
echo "========================================"
echo ""
echo "Images built:"
docker images | grep -E "services-|gateway|customer-app"
echo ""
echo "Next step: Run ./deploy.sh to deploy to Kubernetes"
