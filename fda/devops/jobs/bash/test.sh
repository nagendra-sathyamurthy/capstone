#!/bin/bash
# Run all Newman tests for deployed services
# This script runs API tests against locally deployed Kubernetes services

set -e

echo "=== Capstone Services - Newman Test Suite ==="
echo ""

# Check if kubectl is available
echo "Checking Kubernetes status..."
if ! CONTEXT=$(kubectl config current-context 2>/dev/null); then
    echo "✗ Kubernetes cluster not accessible"
    exit 1
fi
echo "✓ Kubernetes context: $CONTEXT"

# Check if services are running
echo ""
echo "Checking services in capstone-services namespace..."
PODS=$(kubectl get pods -n capstone-services --no-headers 2>/dev/null || echo "")

if [ -z "$PODS" ]; then
    echo "✗ No services are running. Please deploy services first."
    echo "  Run: ./deploy.sh"
    exit 1
fi

echo "✓ Services are running:"
kubectl get pods -n capstone-services --no-headers | while read line; do
    POD_NAME=$(echo "$line" | awk '{print $1}')
    POD_STATUS=$(echo "$line" | awk '{print $3}')
    echo "  - $POD_NAME: $POD_STATUS"
done

# Create test results directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEST_RESULTS_DIR="$SCRIPT_DIR/../../test-results"
mkdir -p "$TEST_RESULTS_DIR"
echo ""
echo "Test results will be saved to: $TEST_RESULTS_DIR"
echo ""

# Define test collections
COLLECTIONS_PATH="$SCRIPT_DIR/../../postman-collections"
declare -A TEST_COLLECTIONS=(
    ["User Registration"]="$COLLECTIONS_PATH/user-registration/User-Registration-Flow.postman_collection.json:user-registration-report"
    ["Restaurant Owner Workflows"]="$COLLECTIONS_PATH/restaurant-owner-workflows/Restaurant-Owner-Workflows.postman_collection.json:restaurant-owner-report"
    ["Operator Service Workflows"]="$COLLECTIONS_PATH/operator-service-workflows/Operator-Service-Workflows.postman_collection.json:operator-service-report"
)

ENVIRONMENT="$COLLECTIONS_PATH/Capstone-Local-Environment.postman_environment.json"
TOTAL_TESTS=${#TEST_COLLECTIONS[@]}
CURRENT_TEST=0
PASSED_TESTS=0
FAILED_TESTS=0

# Run tests for each collection
for name in "${!TEST_COLLECTIONS[@]}"; do
    CURRENT_TEST=$((CURRENT_TEST + 1))
    IFS=':' read -r COLLECTION_PATH REPORT_NAME <<< "${TEST_COLLECTIONS[$name]}"
    
    echo "[$CURRENT_TEST/$TOTAL_TESTS] Testing $name..."
    echo "─────────────────────────────────────────────────────────────"
    
    REPORT_PATH="$TEST_RESULTS_DIR/${REPORT_NAME}.html"
    JSON_REPORT_PATH="$TEST_RESULTS_DIR/${REPORT_NAME}.json"
    
    # Check if collection file exists
    if [ ! -f "$COLLECTION_PATH" ]; then
        echo "✗ Collection file not found: $COLLECTION_PATH"
        FAILED_TESTS=$((FAILED_TESTS + 1))
        echo ""
        continue
    fi
    
    # Run newman with both HTML and JSON reporters
    if npx newman run "$COLLECTION_PATH" \
        -e "$ENVIRONMENT" \
        --reporters cli,htmlextra,json \
        --reporter-htmlextra-export "$REPORT_PATH" \
        --reporter-json-export "$JSON_REPORT_PATH" \
        --timeout-request 10000 \
        --color on 2>&1; then
        echo "✓ $name tests completed successfully"
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        echo "✗ $name tests failed (exit code: $?)"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    
    echo ""
done

# Summary
echo "=== Test Summary ==="
echo "Total Test Collections: $TOTAL_TESTS"
echo "Passed: $PASSED_TESTS"
echo "Failed: $FAILED_TESTS"
echo ""
echo "Test reports available in: $TEST_RESULTS_DIR"
echo ""

if [ $FAILED_TESTS -gt 0 ]; then
    echo "⚠ Some tests failed. Check the reports for details."
    exit 1
else
    echo "✓ All tests passed!"
    exit 0
fi
