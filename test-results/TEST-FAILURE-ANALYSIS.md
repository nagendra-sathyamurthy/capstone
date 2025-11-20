# API Test Failure Analysis
**Date**: November 19, 2025

## Executive Summary
All Newman test collections execute successfully (exit code 0), but **HTTP requests are failing** because:
1. Collections lack test assertions to validate response status codes
2. Most endpoints require JWT authentication tokens
3. Tests don't implement proper authentication workflows

## Detailed Failure Analysis

### 1. Authentication Service ✅ Partial Success
**Status**: Mostly working, minor issues

| Endpoint | Status | Issue |
|----------|--------|-------|
| Customer Registration | 409 Conflict | User already exists (expected for re-runs) |
| Restaurant Owner Registration | 409 Conflict | User already exists (expected for re-runs) |
| Customer Login | ✅ 200 OK | Working correctly |
| Validate Token | 401 Unauthorized | Expected - no token provided |

**Analysis**: Authentication service is functional. 409 errors are expected when users already exist in the database.

### 2. Catalog Service ❌ All Requests Failing
**Status**: Requires authentication

| Endpoint | Status | Issue |
|----------|--------|-------|
| Get All Items | 401 Unauthorized | Missing JWT token |
| Get Item by ID | 401 Unauthorized | Missing JWT token |
| Create Item | 401 Unauthorized | Missing JWT token |
| Create Multiple Items (Bulk) | 401 Unauthorized | Missing JWT token |
| Update Item | 405 Method Not Allowed | Missing JWT token + wrong ID format |
| Delete Item | 405 Method Not Allowed | Missing JWT token + wrong ID format |
| Search Items by Name | 401 Unauthorized | Missing JWT token |

**Root Cause**: All catalog endpoints require JWT authentication. Test collection doesn't login and pass tokens.

### 3. CRM Service ❌ All Requests Failing
**Status**: Requires authentication

| Endpoint | Status | Issue |
|----------|--------|-------|
| Get All Customers | 401 Unauthorized | Missing JWT token |
| Get Customer by ID | 401 Unauthorized | Missing JWT token |
| Create Customer | 401 Unauthorized | Missing JWT token |
| Create Multiple Customers (Bulk) | 401 Unauthorized | Missing JWT token |
| Update Customer | 405 Method Not Allowed | Missing JWT token + wrong ID format |
| Delete Customer | 405 Method Not Allowed | Missing JWT token + wrong ID format |

**Root Cause**: All CRM endpoints require JWT authentication. Test collection doesn't login and pass tokens.

### 4. Cart Service ❌ Most Requests Failing
**Status**: Requires authentication + missing IDs

| Endpoint | Status | Issue |
|----------|--------|-------|
| Create Cart | 401 Unauthorized | Missing JWT token |
| Get Cart by ID | 405 Method Not Allowed | Wrong ID format or missing ID |
| Get Cart by User ID | 404 Not Found | User/cart doesn't exist |
| Add Item to Cart | 404 Not Found | Cart ID not set |
| Update Item Quantity | 404 Not Found | Cart ID not set |
| Remove Item from Cart | 404 Not Found | Cart ID not set |
| Clear Cart | 404 Not Found | Cart ID not set |
| Get Cart Total | 404 Not Found | Cart ID not set |

**Root Cause**: 
1. Missing JWT authentication tokens
2. Cart IDs not being set from previous responses
3. No test data setup

## Why Tests Show as "Passed"

The Newman script considers tests passed based on **exit code 0**, which happens when:
- All HTTP requests complete (even with 4xx/5xx errors)
- No assertions fail (because collections have no assertions)
- No network/timeout errors occur

**Current test metrics:**
```
assertions: 0 executed, 0 failed
```

This means the collections have **NO test assertions** to validate:
- Status codes (should be 200, 201, etc.)
- Response body content
- Error messages
- Data persistence

## Required Fixes

### Priority 1: Add Authentication Flow
All test collections need to:
1. **Login first** using Authentication Service
2. **Extract JWT token** from login response
3. **Store token** in environment variable
4. **Use token** in Authorization header for all subsequent requests

**Example Pre-request Script:**
```javascript
// If no token exists, login first
if (!pm.environment.get("auth_token")) {
    pm.sendRequest({
        url: 'http://localhost:30001/api/auth/login',
        method: 'POST',
        header: { 'Content-Type': 'application/json' },
        body: {
            mode: 'raw',
            raw: JSON.stringify({
                email: 'test@example.com',
                password: 'Test123!'
            })
        }
    }, function (err, res) {
        if (!err) {
            const token = res.json().token;
            pm.environment.set("auth_token", token);
        }
    });
}
```

**Example Authorization Header:**
```
Authorization: Bearer {{auth_token}}
```

### Priority 2: Add Test Assertions
Each request needs test scripts to validate responses:

```javascript
// Test script example
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has required fields", function () {
    const jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
    pm.expect(jsonData).to.have.property('name');
});

pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

### Priority 3: Chain Requests with Dynamic Data
Extract IDs from responses and use in subsequent requests:

```javascript
// After creating a resource
if (pm.response.code === 201) {
    const jsonData = pm.response.json();
    pm.environment.set("created_item_id", jsonData.id);
}

// In next request URL
http://localhost:30002/api/item/{{created_item_id}}
```

### Priority 4: Test Data Management
Create setup scripts to:
1. Clear test databases before each run
2. Seed initial test data
3. Clean up after tests complete

## Immediate Actions

### Option 1: Quick Fix - Use Existing Users
Modify tests to use credentials that already exist:
- Email: Customer who registered (check DB)
- Login to get token
- Use token for all tests

### Option 2: Proper Fix - Rewrite Collections
1. Create "Setup" folder with:
   - Register test users
   - Login and store tokens
2. Update all requests to use Authorization header
3. Add comprehensive test assertions
4. Add cleanup scripts

### Option 3: Use Workflow Collection
Check if `Capstone-Workflow.postman_collection.json` has proper authentication flow already implemented.

## Technical Details

### Newman Command Used
```powershell
npx newman run <collection.json> `
    -e "Capstone-Local-Environment.postman_environment.json" `
    --reporters cli,htmlextra,json `
    --reporter-htmlextra-export "test-results/<report>.html" `
    --reporter-json-export "test-results/<report>.json" `
    --timeout-request 10000
```

### Current Success Criteria
```
✓ All requests execute without network errors
✓ Zero assertions defined = zero assertions failed
✗ HTTP status codes not validated
✗ Response content not validated
✗ Authentication not implemented
```

## Recommendations

1. **Short-term**: Add authentication to existing collections
   - Login request at the start
   - Extract and store token
   - Add Authorization header to all requests

2. **Mid-term**: Add comprehensive test assertions
   - Status code validation
   - Response structure validation
   - Business logic validation

3. **Long-term**: Implement CI/CD integration
   - Automated test runs on every commit
   - Test environment setup/teardown
   - Failed test notifications

## Next Steps

Would you like me to:
1. ✅ **Fix the Postman collections** to include authentication flow and assertions?
2. 🔍 **Check the Workflow collection** to see if it already has proper authentication?
3. 📝 **Create a new comprehensive test collection** with proper authentication and assertions?
4. 🛠️ **Update the Newman script** to fail when HTTP status codes indicate errors?

---
**Note**: Current "all tests passed" message is misleading. Tests execute successfully but APIs are returning error responses (401, 404, 405, 409).
