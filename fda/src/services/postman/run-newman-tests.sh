#!/bin/bash
# Start All Capstone Services for Newman Testing
# This script starts all microservices in the background for API testing

echo "🚀 Starting Capstone Microservices for Newman Testing..."

# Set the base directory
BASE_DIR="c:/dotnet/capstone/fda/src/services"

# Function to start a service
start_service() {
    local service_name=$1
    local port=$2
    local service_dir="$BASE_DIR/$service_name/Services"
    
    echo "Starting $service_name service on port $port..."
    cd "$service_dir"
    
    # Start the service in background
    dotnet run --urls="http://localhost:$port" > "../logs/$service_name.log" 2>&1 &
    
    # Store the PID for later cleanup
    echo $! > "../logs/$service_name.pid"
    
    echo "✅ $service_name service started (PID: $!)"
}

# Create logs directory if it doesn't exist
mkdir -p "$BASE_DIR/authentication/logs"
mkdir -p "$BASE_DIR/catalog/logs" 
mkdir -p "$BASE_DIR/crm/logs"
mkdir -p "$BASE_DIR/cart/logs"

# Start all services
start_service "authentication" 5001
start_service "catalog" 5002
start_service "crm" 5003
start_service "cart" 5004

echo ""
echo "⏳ Waiting 30 seconds for services to fully start..."
sleep 30

echo ""
echo "🔍 Checking service health..."

# Check health of each service
check_health() {
    local service_name=$1
    local port=$2
    local health_endpoint=$3
    
    if curl -s "http://localhost:$port$health_endpoint" > /dev/null; then
        echo "✅ $service_name service is healthy"
    else
        echo "❌ $service_name service is not responding"
    fi
}

check_health "Authentication" 5001 "/api/health"
check_health "Catalog" 5002 "/health"
check_health "CRM" 5003 "/api/health"
check_health "Cart" 5004 "/api/health"

echo ""
echo "🧪 Running Newman tests..."
cd "$BASE_DIR/postman"

# Run Newman with comprehensive reporting
newman run "Capstone-Services.postman_collection.json" \
    -e "local-environment.json" \
    --reporter-json-export "test-results/newman-report.json" \
    --reporter-cli \
    --color on \
    --timeout-request 10000

echo ""
echo "📊 Test Results:"
echo "- JSON Report: $BASE_DIR/postman/test-results/newman-report.json"
echo "- Summary Report: $BASE_DIR/postman/test-results/newman-test-summary.md"

echo ""
echo "🛑 To stop all services, run:"
echo "kill \$(cat $BASE_DIR/*/logs/*.pid) 2>/dev/null"

echo ""
echo "📝 Service Logs:"
echo "- Authentication: $BASE_DIR/authentication/logs/authentication.log"
echo "- Catalog: $BASE_DIR/catalog/logs/catalog.log"
echo "- CRM: $BASE_DIR/crm/logs/crm.log"
echo "- Cart: $BASE_DIR/cart/logs/cart.log"