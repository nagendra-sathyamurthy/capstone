#!/bin/bash
# Setup User Secrets for Local Development
# This script configures MongoDB connection strings in user secrets for each service
# Run this script after cloning the repository to set up your local development environment

set -e

echo "Setting up user secrets for local development..."
echo ""

# Default MongoDB connection string for local development
MONGO_HOST="localhost:27017"
MONGO_USER="admin"

# Prompt for password
read -sp "Enter MongoDB password (default: admin123): " MONGO_PASS
echo ""

# Use default if empty
if [ -z "$MONGO_PASS" ]; then
    MONGO_PASS="admin123"
fi

echo ""
echo "Configuring user secrets for each service..."

# Get script directory and navigate to project root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR/../.."

# Authentication Service
echo "  - Authentication Service"
AUTH_CONN_STR="mongodb://${MONGO_USER}:${MONGO_PASS}@${MONGO_HOST}/authenticationdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" "$AUTH_CONN_STR" --project "$PROJECT_ROOT/src/services/authentication/API"

# Cart Service
echo "  - Cart Service"
CART_CONN_STR="mongodb://${MONGO_USER}:${MONGO_PASS}@${MONGO_HOST}/cartdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" "$CART_CONN_STR" --project "$PROJECT_ROOT/src/services/cart/API"

# Catalog Service
echo "  - Catalog Service"
CATALOG_CONN_STR="mongodb://${MONGO_USER}:${MONGO_PASS}@${MONGO_HOST}/catalogdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" "$CATALOG_CONN_STR" --project "$PROJECT_ROOT/src/services/catalog/API"

# CRM Service
echo "  - CRM Service"
CRM_CONN_STR="mongodb://${MONGO_USER}:${MONGO_PASS}@${MONGO_HOST}/crmdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" "$CRM_CONN_STR" --project "$PROJECT_ROOT/src/services/crm/API"

echo ""
echo "User secrets configured successfully!"
echo ""
echo "You can now run and debug services from VS Code (F5)"
echo ""
echo "To view secrets for a service, run:"
echo "  dotnet user-secrets list --project fda/src/services/<service-name>/API"
echo ""
echo "To update a secret, run:"
echo "  dotnet user-secrets set \"MONGO_CONNECTION_STRING\" \"<new-value>\" --project fda/src/services/<service-name>/API"
echo ""
