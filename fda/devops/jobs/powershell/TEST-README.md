# DevOps Test Scripts - README

Centralized location for all testing scripts in the Food Delivery Application.

---

## Overview

This directory contains the master test script that runs all Postman API collections against deployed Kubernetes services.

### Why Centralized?
- **Single Source of Truth:** One location for all test execution
- **DevOps Integration:** Easy CI/CD pipeline integration
- **Deployment Workflow:** Tests run after deployment validation
- **Consistent Reporting:** Unified test result format

---

## Test Script: test.ps1

### Purpose
Runs comprehensive API tests against all deployed microservices using Newman (Postman CLI).

### Location
```
fda/devops/jobs/powershell/test.ps1
```

### Features
✅ Kubernetes status validation  
✅ Service availability check  
✅ Sequential test execution  
✅ HTML and JSON report generation  
✅ Detailed test summary  
✅ Exit code for CI/CD integration  

---

## Test Collections

The script tests all major workflows in order:

### 1. User Registration
**Collection:** `user-registration/User-Registration-Flow.postman_collection.json`  
**Purpose:** Authentication and user account creation  
**Tests:**
- New user registration
- OTP verification
- User login
- Token management

**Expected Duration:** 1-2 minutes

---

### 2. Restaurant Owner Workflows
**Collection:** `restaurant-owner-workflows/Restaurant-Owner-Workflows.postman_collection.json`  
**Purpose:** Restaurant and menu management  
**Tests:**
- Restaurant registration
- Menu item creation
- Menu item updates
- Restaurant status management
- Business hours management
- Contact information updates

**Expected Duration:** 2-3 minutes

---

### 3. Operator Service Workflows
**Collection:** `operator-service-workflows/Operator-Service-Workflows.postman_collection.json`  
**Purpose:** Kitchen operations and order management  
**Tests:**
- View orders
- Accept orders
- Update menu availability
- Inventory management
- Packaging management
- Order handover (OTP generation/verification)

**Expected Duration:** 3-4 minutes

---

### 4. Customer Workflows ⭐ NEW
**Collection:** `customer-workflows/Customer-Workflows.postman_collection.json`  
**Purpose:** Customer profile and orders  
**Tests:**
- User profile management
- Address management (add/update/delete)
- Complete order flow (browse → cart → checkout → order placement)
- Order history and tracking

**Expected Duration:** 2-3 minutes

---

## Usage

### Basic Usage
```powershell
# Navigate to scripts directory
cd fda/devops/jobs/powershell

# Run all tests
.\test.ps1
```

### Full Deployment + Test Workflow
```powershell
# 1. Deploy all services
.\master-deploy.ps1

# 2. Wait for services to be ready (automatic in master-deploy)

# 3. Run tests
.\test.ps1
```

### Manual Test Execution (Advanced)
```powershell
# Run specific collection with Newman
npx newman run ..\..\..\postman-collections\customer-workflows\Customer-Workflows.postman_collection.json `
  -e ..\..\..\postman-collections\Capstone-Local-Environment.postman_environment.json `
  --reporters cli,htmlextra `
  --reporter-htmlextra-export ..\..\..\test-results\custom-report.html
```

---

## Prerequisites

### 1. Kubernetes Cluster
Services must be deployed to Kubernetes:
```powershell
# Check cluster connection
kubectl config current-context

# Check services
kubectl get pods -n capstone-services

# Deploy if needed
.\master-deploy.ps1
```

### 2. Newman (Postman CLI)
```powershell
# Check installation
npx newman --version

# Install if needed
npm install -g newman

# Install HTML reporter
npm install -g newman-reporter-htmlextra
```

### 3. Postman Collections
Collections must exist in the postman-collections directory:
```
fda/postman-collections/
├── user-registration/
│   └── User-Registration-Flow.postman_collection.json
├── restaurant-owner-workflows/
│   └── Restaurant-Owner-Workflows.postman_collection.json
├── operator-service-workflows/
│   └── Operator-Service-Workflows.postman_collection.json
└── customer-workflows/
    └── Customer-Workflows.postman_collection.json
```

### 4. Environment File
```
fda/postman-collections/Capstone-Local-Environment.postman_environment.json
```

---

## Test Results

### Output Location
```
fda/test-results/
```

### Generated Reports
After running `.\test.ps1`, the following reports are generated:

| Report File | Description | Format |
|------------|-------------|--------|
| `user-registration-report.html` | User registration tests | HTML |
| `user-registration-report.json` | User registration data | JSON |
| `restaurant-owner-report.html` | Restaurant owner tests | HTML |
| `restaurant-owner-report.json` | Restaurant owner data | JSON |
| `operator-service-report.html` | Operator service tests | HTML |
| `operator-service-report.json` | Operator service data | JSON |
| `customer-workflows-report.html` | Customer workflow tests | HTML |
| `customer-workflows-report.json` | Customer workflow data | JSON |

### HTML Reports
- Interactive test results
- Request/response details
- Test assertions
- Execution timings
- Environment variables
- Console logs

### JSON Reports
- Machine-readable format
- CI/CD integration
- Test metrics
- Failure analysis
- Performance data

---

## Script Output

### Success Output
```
=== Capstone Services - Newman Test Suite ===

Checking Kubernetes status...
✓ Kubernetes context: docker-desktop

Checking services in capstone-services namespace...
✓ Services are running:
  - gateway-deployment-xxx: Running
  - authentication-deployment-xxx: Running
  - crm-deployment-xxx: Running
  - catalog-deployment-xxx: Running
  - cart-deployment-xxx: Running
  - order-deployment-xxx: Running

Test results will be saved to: ..\..\..\test-results

[1/4] Testing User Registration...
─────────────────────────────────────────────────────────────
✓ User Registration tests completed successfully

[2/4] Testing Restaurant Owner Workflows...
─────────────────────────────────────────────────────────────
✓ Restaurant Owner Workflows tests completed successfully

[3/4] Testing Operator Service Workflows...
─────────────────────────────────────────────────────────────
✓ Operator Service Workflows tests completed successfully

[4/4] Testing Customer Workflows...
─────────────────────────────────────────────────────────────
✓ Customer Workflows tests completed successfully

=== Test Summary ===
Total Test Collections: 4
Passed: 4
Failed: 0

Test reports available in: ..\..\..\test-results

✓ All tests passed!
```

### Failure Output
```
[2/4] Testing Restaurant Owner Workflows...
─────────────────────────────────────────────────────────────
✗ Restaurant Owner Workflows tests failed (exit code: 1)

=== Test Summary ===
Total Test Collections: 4
Passed: 3
Failed: 1

Test reports available in: ..\..\..\test-results

⚠ Some tests failed. Check the reports for details.
```

---

## Exit Codes

| Code | Meaning | Action |
|------|---------|--------|
| 0 | All tests passed | Continue deployment |
| 1 | Some tests failed | Review reports, fix issues |
| 1 | Services not running | Run `.\deploy.ps1` first |
| 1 | kubectl not found | Install kubectl |

---

## Troubleshooting

### Issue 1: Kubernetes cluster not accessible
**Error:**
```
✗ Kubernetes cluster not accessible
```

**Solution:**
```powershell
# Check Docker Desktop Kubernetes
# Settings → Kubernetes → Enable Kubernetes

# Verify cluster
kubectl cluster-info

# Set context
kubectl config use-context docker-desktop
```

---

### Issue 2: No services are running
**Error:**
```
✗ No services are running. Please deploy services first.
```

**Solution:**
```powershell
# Deploy services
cd fda/devops/jobs/powershell
.\master-deploy.ps1

# Or deploy individually
.\deploy.ps1
```

---

### Issue 3: Collection file not found
**Error:**
```
✗ Collection file not found: .../Customer-Workflows.postman_collection.json
```

**Solution:**
```powershell
# Verify collections exist
Get-ChildItem -Path ..\..\..\postman-collections -Recurse -Filter "*.postman_collection.json"

# Ensure correct path
cd fda/devops/jobs/powershell
```

---

### Issue 4: Newman not installed
**Error:**
```
npx: command not found
```

**Solution:**
```powershell
# Install Node.js first (includes npx)
# Download from: https://nodejs.org/

# Install Newman
npm install -g newman newman-reporter-htmlextra

# Verify
npx newman --version
```

---

### Issue 5: Tests timing out
**Symptom:** Tests fail with timeout errors

**Solution:**
```powershell
# Increase timeout in test.ps1
# Edit line with --timeout-request parameter
--timeout-request 20000  # Increase from 10000 to 20000

# Or check service health
kubectl get pods -n capstone-services
kubectl logs <pod-name> -n capstone-services
```

---

## CI/CD Integration

### GitHub Actions Example
```yaml
name: API Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Install Newman
      run: npm install -g newman newman-reporter-htmlextra
    
    - name: Deploy Services
      run: |
        cd fda/devops/jobs/powershell
        pwsh -File master-deploy.ps1
    
    - name: Run Tests
      run: |
        cd fda/devops/jobs/powershell
        pwsh -File test.ps1
    
    - name: Upload Test Reports
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: test-reports
        path: fda/test-results/
```

### Azure DevOps Pipeline Example
```yaml
trigger:
- main
- develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: NodeTool@0
  inputs:
    versionSpec: '18.x'
  displayName: 'Install Node.js'

- script: npm install -g newman newman-reporter-htmlextra
  displayName: 'Install Newman'

- task: PowerShell@2
  inputs:
    filePath: 'fda/devops/jobs/powershell/master-deploy.ps1'
  displayName: 'Deploy Services'

- task: PowerShell@2
  inputs:
    filePath: 'fda/devops/jobs/powershell/test.ps1'
  displayName: 'Run API Tests'

- task: PublishTestResults@2
  condition: always()
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: '**/test-results/*.json'
  displayName: 'Publish Test Results'
```

---

## Best Practices

### 1. Run Tests After Every Deployment
```powershell
.\master-deploy.ps1 && .\test.ps1
```

### 2. Review Reports on Failure
```powershell
# Open HTML report in browser
explorer ..\..\..\test-results\customer-workflows-report.html
```

### 3. Keep Collections Updated
```powershell
# Ensure collections match current API
cd ..\..\..\postman-collections
git pull origin main
```

### 4. Clean Old Test Results
```powershell
# Remove old reports
Remove-Item ..\..\..\test-results\*.html
Remove-Item ..\..\..\test-results\*.json
```

### 5. Monitor Test Duration
```
Expected total duration: 8-12 minutes for all 4 collections
If tests take longer, investigate service performance
```

---

## Performance Benchmarks

### Local Development (Docker Desktop)
| Collection | Requests | Duration | Status |
|-----------|----------|----------|--------|
| User Registration | 10+ | 1-2m | ✓ |
| Restaurant Owner | 15+ | 2-3m | ✓ |
| Operator Service | 20+ | 3-4m | ✓ |
| Customer Workflows | 21 | 2-3m | ✓ |
| **Total** | **66+** | **8-12m** | ✓ |

### Kubernetes Cluster (Production-like)
| Collection | Requests | Duration | Status |
|-----------|----------|----------|--------|
| User Registration | 10+ | 30s-1m | ✓ |
| Restaurant Owner | 15+ | 1-2m | ✓ |
| Operator Service | 20+ | 2-3m | ✓ |
| Customer Workflows | 21 | 1-2m | ✓ |
| **Total** | **66+** | **5-8m** | ✓ |

---

## Related Documentation

- [Postman Collections README](../../../postman-collections/README.md)
- [Customer Workflows Guide](../../../postman-collections/customer-workflows/README.md)
- [Test Data Reference](../../../postman-collections/customer-workflows/TEST-DATA-REFERENCE.md)
- [Deployment Guide](../../QUICK-DEPLOYMENT-GUIDE.md)
- [Master Deploy Script](./master-deploy.ps1)

---

## Support

### Getting Help
1. Review test reports in HTML format
2. Check service logs: `kubectl logs <pod> -n capstone-services`
3. Verify services: `kubectl get pods -n capstone-services`
4. Check Newman output for specific errors
5. Review collection documentation

### Common Commands
```powershell
# View test results
explorer ..\..\..\test-results\

# Check service status
kubectl get pods -n capstone-services

# View service logs
kubectl logs -f deployment/gateway-deployment -n capstone-services

# Restart failed pod
kubectl delete pod <pod-name> -n capstone-services

# Re-run tests
.\test.ps1
```

---

**Last Updated:** December 4, 2025  
**Version:** 2.0.0  
**Test Collections:** 4 (User Registration, Restaurant Owner, Operator Service, Customer Workflows)  
**Total API Tests:** 66+ requests across all collections
