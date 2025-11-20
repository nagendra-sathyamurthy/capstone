# Comprehensive Test Suite - Food Delivery Application

Automated testing suite for all Postman workflow collections in the Food Delivery Application.

---

## Overview

The `Run-All-Tests.ps1` PowerShell script provides comprehensive automated testing across all workflow collections:

- **User Registration**: 43 user accounts across 9 roles
- **Restaurant Owner Workflows**: Restaurant and menu management (6 test suites)
- **Operator Service Workflows**: Order, inventory, and delivery management (8 test suites)

**Total**: 19 test suites covering the complete application workflow

---

## Quick Start

### Basic Usage

```powershell
# Run all tests against local environment
.\Run-All-Tests.ps1
```

### Advanced Usage

```powershell
# Run against production environment
.\Run-All-Tests.ps1 -Environment Production

# Run with verbose output
.\Run-All-Tests.ps1 -Verbose

# Run without generating HTML reports (faster)
.\Run-All-Tests.ps1 -GenerateReports $false
```

---

## Prerequisites

### Required Software

1. **Newman** (Postman CLI)
   ```powershell
   npm install -g newman
   ```

2. **newman-reporter-htmlextra** (for HTML reports)
   ```powershell
   npm install -g newman-reporter-htmlextra
   ```

### Required Files

- `Capstone-Local-Environment.postman_environment.json` (for local testing)
- `Capstone-Production-Environment.postman_environment.json` (for production testing)
- All workflow collection files in their respective folders

### Running Services

Ensure the following services are running before executing tests:

**Local Environment**:
```powershell
# Start all services
docker-compose -f docker-compose-local.yml up -d

# Verify services
docker ps
```

**Required Services**:
- Authentication Service (port 30001)
- Catalog Service (port 30002)
- CRM Service (port 30003)
- Cart Service (port 30004)
- Order Service (port 30005)
- Payment Service (port 30006)
- MongoDB databases

---

## Script Parameters

### -Environment

Specifies the target environment for testing.

**Values**: `Local` (default), `Production`

```powershell
# Test against local environment
.\Run-All-Tests.ps1 -Environment Local

# Test against production environment
.\Run-All-Tests.ps1 -Environment Production
```

### -GenerateReports

Controls HTML report generation.

**Type**: Boolean  
**Default**: `$true`

```powershell
# Generate HTML reports (default)
.\Run-All-Tests.ps1 -GenerateReports $true

# Skip report generation (faster execution)
.\Run-All-Tests.ps1 -GenerateReports $false
```

### -Verbose

Shows detailed test output from Newman.

**Type**: Switch  
**Default**: `$false`

```powershell
# Show detailed output
.\Run-All-Tests.ps1 -Verbose
```

---

## Test Phases

### Phase 1: User Registration (5 suites)

Registers and authenticates 43 user accounts across 9 roles:

1. **Customers** (5 accounts)
   - Role: Customer (0)
   - Organization: external_users
   - Use Case: Order placement, cart operations

2. **Restaurant Owners** (8 accounts)
   - Role: Biller (1)
   - Organizations: Pizza Palace, Sushi Spot, Burger Hub, etc.
   - Use Case: Restaurant and menu management

3. **Kitchen Workers** (10 accounts)
   - Role: Worker (3)
   - Organizations: Various restaurants
   - Use Case: Order preparation, kitchen operations

4. **Delivery Agents** (10 accounts)
   - Role: DeliveryAgent (4)
   - Organizations: Fast Delivery Co, Quick Courier, etc.
   - Use Case: Order pickup and delivery

5. **IT Admins** (10 accounts)
   - Roles: Developer (5), Tester (6), NetworkAdmin (7), DatabaseAdmin (8)
   - Organization: FoodDelivery IT
   - Use Case: System administration, testing, maintenance

### Phase 2: Restaurant Owner Workflows (6 suites)

Tests restaurant and menu management operations:

6. **Restaurant Registration**
   - Register restaurants with complete business information
   - Address, contact info, business hours

7. **Add Menu Items**
   - Add items with pricing, categories, dietary info
   - Time-based availability, inventory tracking

8. **Update Menu Items**
   - Modify prices, descriptions, availability
   - Toggle item status

9. **Restaurant Status Updates**
   - Activate/deactivate restaurant
   - Manage visibility to customers

10. **Business Hours Management**
    - Update operating hours by day
    - Set closed days
    - Handle midnight crossing times

11. **Contact Information Updates**
    - Update phone, email, website
    - Validate contact formats

### Phase 3: Operator Service Workflows (8 suites)

Tests order management, inventory, and delivery operations:

12. **View Orders**
    - View pending orders
    - View accepted orders
    - View order history

13. **Accept Orders**
    - Accept customer orders
    - Start order preparation
    - Update order status

14. **Menu Availability**
    - Update item availability based on inventory
    - Toggle items on/off
    - Manage out-of-stock items

15. **Inventory Management**
    - Check current inventory levels
    - View low-stock alerts
    - Update stock quantities

16. **Packaging Management**
    - Update packaging details
    - Add special instructions
    - Mark fragile/eco-friendly packaging

17. **Generate Handover OTP**
    - Generate OTP for order handover
    - Secure delivery agent verification

18. **Verify Handover OTP**
    - Verify OTP from delivery agent
    - Complete order handover
    - Update order status to "Picked Up"

19. **Complete Operator Workflow (E2E)**
    - End-to-end workflow test
    - Order acceptance to handover
    - All operator operations in sequence

---

## Output and Reports

### Console Output

The script provides color-coded console output:

- **Cyan**: Headers and section titles
- **Green**: Success messages and passed tests
- **Red**: Error messages and failed tests
- **Yellow**: Information and warnings
- **White**: General information

### HTML Reports

When `GenerateReports` is enabled, HTML reports are generated in the `test-results/` directory:

```
test-results/
├── 00-test-summary.txt                      # Text summary of all tests
├── 01-user-registration-customers.html      # Customer registration report
├── 02-user-registration-owners.html         # Owner registration report
├── 03-user-registration-workers.html        # Worker registration report
├── 04-user-registration-agents.html         # Agent registration report
├── 05-user-registration-admins.html         # Admin registration report
├── 06-restaurant-registration.html          # Restaurant registration report
├── 07-menu-add-items.html                   # Menu addition report
├── 08-menu-update-items.html                # Menu update report
├── 09-restaurant-status.html                # Status management report
├── 10-restaurant-hours.html                 # Hours management report
├── 11-restaurant-contact.html               # Contact updates report
├── 12-operator-view-orders.html             # View orders report
├── 13-operator-accept-orders.html           # Accept orders report
├── 14-operator-menu-availability.html       # Menu availability report
├── 15-operator-inventory.html               # Inventory management report
├── 16-operator-packaging.html               # Packaging management report
├── 17-operator-generate-otp.html            # OTP generation report
├── 18-operator-verify-otp.html              # OTP verification report
└── 19-operator-complete-workflow.html       # Complete workflow report
```

### Summary Report

At the end of execution, a summary is displayed:

```
========================================
  Test Execution Summary
========================================

Execution Details:
  Environment:      Local
  Started:          2025-11-20 14:30:00
  Completed:        2025-11-20 14:45:30
  Total Duration:   930.50 seconds (15.51 minutes)

Test Results:
  Total Suites:     19
  Passed:           19
  Failed:           0
  Success Rate:     100%

Reports Generated:
  Location:         C:\dotnet\capstone\fda\postman-collections\test-results
  Total Reports:    19 HTML files
```

---

## Test Data

### Data Files Used

**User Registration**:
- `user-registration/data/customer-registration.json` (5 accounts)
- `user-registration/data/restaurant-owner-registration.json` (8 accounts)
- `user-registration/data/kitchen-worker-registration.json` (10 accounts)
- `user-registration/data/delivery-agent-registration.json` (10 accounts)
- `user-registration/data/it-admin-registration.json` (10 accounts)

**Restaurant Owner Workflows**:
- `restaurant-owner-workflows/data/restaurants.json` (8 restaurant profiles)
- `restaurant-owner-workflows/data/menu-items.json` (15 menu items)

**Operator Service Workflows**:
- `operator-service-workflows/data/operators.json` (3 operator accounts)
- `operator-service-workflows/data/test-orders.json` (5 order scenarios)
- `operator-service-workflows/data/menu-items-inventory.json` (15 inventory items)
- `operator-service-workflows/data/delivery-agents.json` (3 agent profiles)

---

## Execution Time

**Typical execution times** (approximate):

| Phase | Suites | Estimated Time |
|-------|--------|----------------|
| User Registration | 5 | 2-3 minutes |
| Restaurant Owner Workflows | 6 | 3-4 minutes |
| Operator Service Workflows | 8 | 5-7 minutes |
| **Total** | **19** | **10-15 minutes** |

**Factors affecting execution time**:
- Network latency
- Service response times
- Number of data iterations
- Report generation (adds ~10-20% overhead)
- Concurrent system load

---

## Troubleshooting

### Newman Not Found

**Error**: `newman : The term 'newman' is not recognized`

**Solution**:
```powershell
npm install -g newman
```

### Reporter Not Found

**Error**: `Reporter htmlextra not found`

**Solution**:
```powershell
npm install -g newman-reporter-htmlextra
```

### Services Not Running

**Error**: `ECONNREFUSED` or connection timeout errors

**Solution**:
```powershell
# Check running services
docker ps

# Start services if needed
docker-compose -f docker-compose-local.yml up -d

# Check service logs
docker logs capstone-authentication
docker logs capstone-catalog
```

### Environment File Not Found

**Error**: `Environment file not found: Capstone-Local-Environment.postman_environment.json`

**Solution**:
- Ensure you're running the script from the `postman-collections` directory
- Verify the environment file exists and has the correct name
- Check file permissions

### Test Failures

If tests fail:

1. **Check Service Health**:
   ```powershell
   docker ps
   docker logs <service-name>
   ```

2. **Review HTML Reports**:
   - Open the HTML report for the failed suite
   - Check the "Failed Tests" section
   - Review request/response details

3. **Verify Test Data**:
   - Ensure data files exist and are valid JSON
   - Check for duplicate emails or conflicting data

4. **Re-run Specific Suite**:
   ```powershell
   # Example: Re-run operator workflows only
   newman run operator-service-workflows/Operator-Service-Workflows.postman_collection.json `
     -e Capstone-Local-Environment.postman_environment.json
   ```

### Permission Errors

**Error**: `Access denied` or file write errors

**Solution**:
```powershell
# Run PowerShell as Administrator
# Or adjust script execution policy
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## CI/CD Integration

### GitHub Actions

```yaml
name: API Tests

on:
  push:
    branches: [ master, develop ]
  pull_request:
    branches: [ master ]

jobs:
  test:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Install Newman
      run: |
        npm install -g newman
        npm install -g newman-reporter-htmlextra
    
    - name: Start Services
      run: docker-compose -f docker-compose-local.yml up -d
    
    - name: Wait for Services
      run: Start-Sleep -Seconds 30
    
    - name: Run Tests
      run: |
        cd fda/postman-collections
        .\Run-All-Tests.ps1 -Environment Local
    
    - name: Upload Test Reports
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-reports
        path: fda/postman-collections/test-results/
```

### Azure DevOps

```yaml
trigger:
- master
- develop

pool:
  vmImage: 'windows-latest'

steps:
- task: NodeTool@0
  inputs:
    versionSpec: '18.x'
  displayName: 'Install Node.js'

- script: |
    npm install -g newman
    npm install -g newman-reporter-htmlextra
  displayName: 'Install Newman'

- script: |
    docker-compose -f docker-compose-local.yml up -d
  displayName: 'Start Services'

- task: PowerShell@2
  inputs:
    filePath: 'fda/postman-collections/Run-All-Tests.ps1'
    arguments: '-Environment Local'
  displayName: 'Run API Tests'

- task: PublishTestResults@2
  condition: always()
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: '**/test-results/*.xml'
  displayName: 'Publish Test Results'
```

---

## Best Practices

### Before Running Tests

1. ✅ Ensure all services are running and healthy
2. ✅ Verify environment file is up-to-date
3. ✅ Check for any pending database migrations
4. ✅ Clear any stale test data if needed
5. ✅ Ensure sufficient disk space for reports

### During Test Execution

1. ⏸️ Avoid making manual API calls during test runs
2. ⏸️ Don't restart services while tests are running
3. ⏸️ Monitor system resources (CPU, memory, disk)
4. ⏸️ Keep terminal window open until completion

### After Test Execution

1. 📊 Review test reports for failures
2. 📊 Check service logs for errors
3. 📊 Archive reports for historical tracking
4. 📊 Clean up test data if needed
5. 📊 Document any issues found

---

## Customization

### Running Specific Test Suites

To run only specific test suites, modify the script or run Newman directly:

```powershell
# Run only user registration tests
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/customer-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Run only operator workflows
newman run operator-service-workflows/Operator-Service-Workflows.postman_collection.json `
  -e Capstone-Local-Environment.postman_environment.json

# Run specific folder from collection
newman run restaurant-owner-workflows/Restaurant-Owner-Workflows.postman_collection.json `
  --folder "Menu Management - Add Items" `
  -e Capstone-Local-Environment.postman_environment.json
```

### Adding New Test Suites

To add a new test suite to the script:

1. Add the collection file to the appropriate folder
2. Create test data files if needed
3. Add an `Invoke-NewmanTest` call in the script:

```powershell
Invoke-NewmanTest `
    -CollectionName "My New Test Suite" `
    -CollectionPath (Join-Path $ScriptDir "my-folder\My-Collection.postman_collection.json") `
    -DataFile (Join-Path $ScriptDir "my-folder\data\my-data.json") `
    -ReportName "20-my-new-tests" `
    -Description "Description of what this suite tests"
```

### Adjusting Delays and Timeouts

Modify these values in the script if needed:

```powershell
"--delay-request", "500",        # Delay between requests (ms)
"--timeout-request", "10000"     # Request timeout (ms)
```

---

## Support

For issues or questions:

1. **Check Service Logs**: `docker logs <service-name>`
2. **Review HTML Reports**: Detailed error information in reports
3. **Consult Documentation**: See individual workflow README files
4. **Test Data Reference**: Check TEST-DATA-REFERENCE.md files

---

## Related Documentation

- [User Registration Flow](./user-registration/README.md)
- [User Registration Test Data](./user-registration/TEST-DATA-REFERENCE.md)
- [Restaurant Owner Workflows](./restaurant-owner-workflows/README.md)
- [Restaurant Owner Test Data](./restaurant-owner-workflows/TEST-DATA-REFERENCE.md)
- [Operator Service Workflows](./operator-service-workflows/README.md)
- [Operator Service Test Data](./operator-service-workflows/TEST-DATA-REFERENCE.md)
- [RBAC Documentation](../docs/services/RBAC-COMPREHENSIVE.md)

---

**Version**: 1.0.0  
**Last Updated**: November 20, 2025  
**Maintained By**: FDA QA Team  
**Total Test Suites**: 19 (43 user accounts + restaurant/operator workflows)
