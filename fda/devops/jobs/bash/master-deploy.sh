#!/bin/bash

###############################################################################
# Master Deployment Script - Complete workflow from verify to test
###############################################################################
# Runs the complete deployment workflow:
# 1. Verify - Check Kubernetes is ready
# 2. Build - Build all Docker images
# 3. Secrets - Apply Kubernetes secrets
# 4. Deploy - Deploy all services to Kubernetes
# 5. Seed - Seed sample data to services
# 6. Test - Run Newman API tests
#
# Usage:
#   ./master-deploy.sh              # Full deployment
#   ./master-deploy.sh --skip-build # Skip build step
#   ./master-deploy.sh --skip-seed  # Skip seed step
#   ./master-deploy.sh --skip-test  # Skip test step
###############################################################################

set -e  # Exit on error (we'll handle errors manually for some steps)

# Parse arguments
SKIP_BUILD=false
SKIP_SEED=false
SKIP_TEST=false

for arg in "$@"; do
    case $arg in
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        --skip-seed)
            SKIP_SEED=true
            shift
            ;;
        --skip-test)
            SKIP_TEST=true
            shift
            ;;
        --help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  --skip-build    Skip the Docker build step"
            echo "  --skip-seed     Skip the data seeding step"
            echo "  --skip-test     Skip the API testing step"
            echo "  --help          Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $arg"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Script metadata
SCRIPT_NAME="Master Deploy"
SCRIPT_VERSION="1.0.0"

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# Helper functions
write_step() {
    echo -e "\n${CYAN}========================================${NC}"
    echo -e "${CYAN}  $1${NC}"
    echo -e "${CYAN}========================================${NC}\n"
}

write_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

write_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

write_error() {
    echo -e "${RED}❌ $1${NC}"
}

write_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Header
echo -e "\n${MAGENTA}╔════════════════════════════════════════╗${NC}"
echo -e "${MAGENTA}║     MASTER DEPLOYMENT WORKFLOW         ║${NC}"
echo -e "${MAGENTA}║     Version: $SCRIPT_VERSION                    ║${NC}"
echo -e "${MAGENTA}╚════════════════════════════════════════╝${NC}\n"

START_TIME=$(date +%s)
START_DATETIME=$(date '+%Y-%m-%d %H:%M:%S')
write_info "Started at: $START_DATETIME"

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Track overall success
OVERALL_SUCCESS=true
FAILED_STEPS=()

# Step 1: Verify
write_step "STEP 1: VERIFY KUBERNETES"
write_info "Checking if Kubernetes cluster is ready..."

VERIFY_SCRIPT="$SCRIPT_DIR/verify.sh"
if [ -f "$VERIFY_SCRIPT" ]; then
    if bash "$VERIFY_SCRIPT"; then
        write_success "Kubernetes verification completed"
    else
        write_error "Verification failed"
        exit 1
    fi
else
    write_warning "verify.sh not found, skipping verification"
fi

# Step 2: Build
if [ "$SKIP_BUILD" = false ]; then
    write_step "STEP 2: BUILD DOCKER IMAGES"
    write_info "Building all Docker images..."
    
    BUILD_SCRIPT="$SCRIPT_DIR/build.sh"
    if [ -f "$BUILD_SCRIPT" ]; then
        if bash "$BUILD_SCRIPT"; then
            write_success "Docker images built successfully"
        else
            write_error "Build failed"
            exit 1
        fi
    else
        write_warning "build.sh not found, skipping build"
    fi
else
    write_info "Skipping build step (using existing images)"
fi

# Step 3: Secrets
write_step "STEP 3: APPLY KUBERNETES SECRETS"
write_info "Applying MongoDB secrets..."

SECRETS_SCRIPT="$SCRIPT_DIR/secrets.sh"
if [ -f "$SECRETS_SCRIPT" ]; then
    if bash "$SECRETS_SCRIPT"; then
        write_success "Kubernetes secrets applied successfully"
    else
        write_error "Secrets application failed"
        exit 1
    fi
else
    write_error "secrets.sh not found"
    exit 1
fi

# Step 4: Deploy
write_step "STEP 4: DEPLOY SERVICES"
write_info "Deploying all services to Kubernetes..."

DEPLOY_SCRIPT="$SCRIPT_DIR/deploy.sh"
if [ -f "$DEPLOY_SCRIPT" ]; then
    if bash "$DEPLOY_SCRIPT"; then
        write_success "Services deployed successfully"
    else
        write_error "Deployment failed"
        exit 1
    fi
else
    write_error "deploy.sh not found"
    exit 1
fi

# Wait for services to be ready
write_info "Waiting 30 seconds for services to stabilize..."
sleep 30

# Step 5: Seed
if [ "$SKIP_SEED" = false ]; then
    write_step "STEP 5: SEED SAMPLE DATA"
    write_info "Seeding menu items and sample data..."
    
    SEED_SCRIPT="$SCRIPT_DIR/seed.sh"
    if [ -f "$SEED_SCRIPT" ]; then
        if bash "$SEED_SCRIPT"; then
            write_success "Sample data seeded successfully"
        else
            write_warning "Seeding failed, but continuing..."
            FAILED_STEPS+=("Seed")
            OVERALL_SUCCESS=false
        fi
    else
        write_warning "seed.sh not found, skipping seed"
    fi
else
    write_info "Skipping seed step"
fi

# Step 6: Test
if [ "$SKIP_TEST" = false ]; then
    write_step "STEP 6: RUN API TESTS"
    write_info "Running Newman API tests..."
    
    TEST_SCRIPT="$SCRIPT_DIR/test.sh"
    if [ -f "$TEST_SCRIPT" ]; then
        if bash "$TEST_SCRIPT"; then
            write_success "API tests passed successfully"
        else
            write_warning "Tests failed, but deployment is complete"
            FAILED_STEPS+=("Test")
            OVERALL_SUCCESS=false
        fi
    else
        write_warning "test.sh not found, skipping tests"
    fi
else
    write_info "Skipping test step"
fi

# Summary
END_TIME=$(date +%s)
END_DATETIME=$(date '+%Y-%m-%d %H:%M:%S')
DURATION=$((END_TIME - START_TIME))
DURATION_FORMATTED=$(printf '%02d:%02d:%02d' $((DURATION/3600)) $((DURATION%3600/60)) $((DURATION%60)))

echo -e "\n${MAGENTA}╔════════════════════════════════════════╗${NC}"
echo -e "${MAGENTA}║          DEPLOYMENT SUMMARY            ║${NC}"
echo -e "${MAGENTA}╚════════════════════════════════════════╝${NC}\n"

write_info "Started:  $START_DATETIME"
write_info "Finished: $END_DATETIME"
write_info "Duration: $DURATION_FORMATTED"

if [ "$OVERALL_SUCCESS" = true ]; then
    echo -e "\n${GREEN}✅ DEPLOYMENT COMPLETED SUCCESSFULLY!${NC}"
    echo -e "\n${CYAN}Services are ready at:${NC}"
    echo -e "${NC}  Gateway:      http://localhost:5000${NC}"
    echo -e "${NC}  Customer App: http://localhost:4200${NC}"
    echo -e "\n${YELLOW}Next steps:${NC}"
    echo -e "${NC}  1. Open browser to http://localhost:4200${NC}"
    echo -e "${NC}  2. Check service logs: kubectl logs -n capstone-services <pod-name>${NC}"
    echo -e "${NC}  3. Run additional tests from postman-collections/${NC}"
    exit 0
else
    echo -e "\n${YELLOW}⚠️  DEPLOYMENT COMPLETED WITH WARNINGS${NC}"
    if [ ${#FAILED_STEPS[@]} -gt 0 ]; then
        echo -e "\n${YELLOW}Failed/Warning Steps:${NC}"
        for step in "${FAILED_STEPS[@]}"; do
            echo -e "${RED}  • $step${NC}"
        done
    fi
    echo -e "\n${CYAN}Core services may still be running. Check with:${NC}"
    echo -e "${NC}  kubectl get pods -n capstone-services${NC}"
    exit 1
fi
