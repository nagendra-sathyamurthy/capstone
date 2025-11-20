# Newman API Test Fixes - Summary

## Date: November 20, 2025

## Problem Statement
Newman API tests were failing with 401 Unauthorized errors for Catalog and CRM services, despite tests showing "passed" status with exit code 0.

## Root Causes Identified

### 1. Missing Test Assertions
- Original collections had **zero assertions**
- Tests always passed regardless of HTTP status codes
- No validation of response data

### 2. JWT Authentication Not Implemented
- Collections lacked authentication flow
- No Bearer token management
- Services require JWT tokens for all operations

### 3. JWT Secret Key Mismatch  
- **Catalog service** used incorrect key: `YourSuperSecretKey`
- **Authentication/CRM/Cart** used correct key: `GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI`
- Tokens signed with one key couldn't be validated by service using different key

### 4. Collection Variable Resolution in Newman
- Collection-level Bearer auth with `{{auth_token}}` didn't resolve properly in Newman
- Pre-request async calls (pm.sendRequest) caused race conditions
- Newman doesn't wait for asynchronous operations before executing requests

### 5. User Registration Format
- Role parameter required **integer** (0 for Customer) not string ("customer")
- Organization field required: `external_users`

## Solutions Implemented

### 1. Fixed Catalog Service JWT Configuration
**File:** `fda/src/services/catalog/API/Program.cs`
```csharp
// Changed from:
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"))

// To:
var jwtKey = "GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI";
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
```

### 2. Rebuilt Catalog Docker Image
```powershell
cd c:\dotnet\capstone\fda\src\services\catalog
docker build --no-cache -t services-catalog:latest .
docker rm -f capstone-catalog
docker run -d --name capstone-catalog --network jobs_capstone-network -p 30002:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e MONGO_CONNECTION_STRING="mongodb://admin:AdminPass2024@capstone-mongodb:27017/catalogdb?authSource=admin" \
  services-catalog:latest
```

### 3. Created Authentication-Enabled Collections
**Files Created:**
- `Catalog-Service-WithAuth.postman_collection.json`
- `CRM-Service-WithAuth.postman_collection.json`

**Features:**
- Registration and login requests  
- Collection variables for token storage
- Test assertions for response validation
- Bearer token authentication

### 4. Implemented Collection-Level Pre-Request Script
```javascript
// Add Authorization header to each request using collection variable
const token = pm.collectionVariables.get('auth_token');
if (token && token !== '') {
    pm.request.headers.add({
        key: 'Authorization',
        value: `Bearer ${token}`
    });
}
```

### 5. Fixed User Registration Format
```json
{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "Test",
  "lastName": "User",
  "phoneNumber": "1234567890",
  "role": 0,  // Integer, not string
  "organization": "external_users"
}
```

### 6. Updated Test Runner Script
**File:** `fda/devops/jobs/run-newman-tests.ps1`
- Updated to use new `-WithAuth` collections
- Maintained environment variable configuration

## Test Results Comparison

### Before Fixes
```
Total Services Tested: 4
Passed: 2 (Authentication, Cart)
Failed: 2 (Catalog, CRM)
Total Assertions: 22
Failed Assertions: 16
Main Error: 401 Unauthorized
```

### After Fixes
```
Total Services Tested: 4
Passed: 2 (Authentication, Cart)  
Failed: 2 (Catalog, CRM - API issues, not auth)
Total Assertions: 31
Failed Assertions: 9 (all API-level, no auth failures)
Auth Success: ✅ All 401 errors resolved
```

### Detailed Results

#### Catalog Service
| Request | Before | After | Status |
|---------|--------|-------|--------|
| Register Test User | 409 Conflict | 409 Conflict | ✅ Expected |
| Login and Get Token | 200 OK | 200 OK | ✅ Pass |
| Get All Items | **401 Unauthorized** | **200 OK** | ✅ Fixed |
| Create Item | **401 Unauthorized** | **201 Created** | ✅ Fixed |
| Get Item by ID | **401 Unauthorized** | 404 Not Found | ⚠️ API Issue |
| Bulk Create | **401 Unauthorized** | **200 OK** | ✅ Fixed |
| Update Item | **401 → 405** | 404 Not Found | ⚠️ API Issue |
| Delete Item | **401 → 405** | 404 Not Found | ⚠️ API Issue |

#### CRM Service  
| Request | Before | After | Status |
|---------|--------|-------|--------|
| Register Test User | 409 Conflict | 409 Conflict | ✅ Expected |
| Login and Get Token | 200 OK | 200 OK | ✅ Pass |
| Get All Customers | **401 Unauthorized** | **200 OK** | ✅ Fixed |
| Create Customer | **401 Unauthorized** | 400 Bad Request | ⚠️ API Validation |
| Get Customer by ID | **401 Unauthorized** | 200 OK (empty) | ⚠️ API Issue |
| Bulk Create | **401 Unauthorized** | 400 Bad Request | ⚠️ API Validation |
| Update Customer | **401 → 405** | 405 Method Not Allowed | ⚠️ API Issue |
| Delete Customer | **401 → 405** | 405 Method Not Allowed | ⚠️ API Issue |

## Key Accomplishments

✅ **Fixed all JWT authentication issues**  
✅ **Resolved Catalog service JWT key mismatch**  
✅ **Implemented proper token management in Newman**  
✅ **Added comprehensive test assertions**  
✅ **Reduced failed assertions from 16 to 9**  
✅ **All authentication flows working correctly**

## Remaining Issues (API-Level, Not Auth)

### Catalog Service
1. **Get/Update/Delete by ID**: 404 errors - Item IDs not persisting properly
2. **Possible MongoDB collection issue** - Items created but not found by ID

### CRM Service
1. **Create/Bulk Customer**: 400 Bad Request - Request body validation issues
2. **Update/Delete**: 405 Method Not Allowed - API endpoints may not be implemented
3. **Get by ID**: Returns empty array instead of single object

## Technical Details

### JWT Token Structure
```json
{
  "userId": "691d9a51493c85a79831d73a",
  "email": "test@example.com",
  "role": "Customer",
  "organization": "external_users",
  "permissions": ["view_menu", "place_order", "track_order", ...],
  "exp": 1763577179,
  "iat": 1763548379
}
```

### Service Ports
- Authentication: `http://localhost:30001`
- Catalog: `http://localhost:30002`
- CRM: `http://localhost:30003`
- Cart: `http://localhost:30004`
- MongoDB: `mongodb://localhost:30000`

### Docker Network
- Network: `jobs_capstone-network`
- All services connected via Docker Compose

## Files Modified

1. `fda/src/services/catalog/API/Program.cs` - JWT key fix
2. `fda/postman-collections/Catalog-Service-WithAuth.postman_collection.json` - New
3. `fda/postman-collections/CRM-Service-WithAuth.postman_collection.json` - New
4. `fda/devops/jobs/run-newman-tests.ps1` - Updated collection names

## Recommendations

### Immediate
1. ✅ **Authentication fixed** - No further action needed
2. ⚠️ **Investigate Catalog item persistence** - IDs not being retrieved correctly
3. ⚠️ **Fix CRM customer creation** - Review request body validation
4. ⚠️ **Implement Update/Delete endpoints** - Currently returning 405

### Future Enhancements
1. Add more comprehensive test data
2. Implement test data cleanup between runs
3. Add integration tests for cross-service workflows
4. Document API endpoint specifications
5. Add automated test reporting with HTML output

## Conclusion

**Major Success:** All JWT authentication issues resolved. The Catalog and CRM services now properly authenticate requests using Bearer tokens. The remaining test failures are API implementation issues (404/400/405 errors), not authentication problems.

**Test Reliability:** Tests now have proper assertions and accurately reflect API health. Exit code properly indicates test failures.

**Next Steps:** Address the remaining API-level issues (item persistence, customer creation validation, missing endpoints) to achieve 100% test pass rate.
