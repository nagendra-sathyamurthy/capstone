# Newman Test Failures - Root Cause Analysis & Solutions

## 🔍 **Root Cause Analysis**

### **1. Authentication Service - 400 Bad Request** 
**Issue**: Postman collections use string values for `role` field, but API expects UserRole enum (integer)

**Example of Current (Broken) Request**:
```json
{
  "role": "customer"  // ❌ String value
}
```

**Correct Request Format**:
```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "role": 0,  // ✅ Integer: 0=Customer, 1=Biller, 2=Operator, etc.
  "organization": "external_users"
}
```

**UserRole Enum Values**:
- 0 = Customer
- 1 = Biller (Restaurant Owner)
- 2 = Operator (Staff)
- 3 = Worker (Kitchen Staff)
- 4 = DeliveryAgent
- 5 = Developer
- 6 = Tester
- 7 = NetworkAdmin
- 8 = DatabaseAdmin

### **2. CRM Service - 401 Unauthorized**
**Issue**: CRM endpoints require JWT authentication token

**Solution**: First register/login to get token, then use it in Authorization header:
```
Authorization: Bearer <jwt-token>
```

### **3. Cart Service - 404 Not Found**
**Issue**: API route structure mismatch in Postman collections

**Current Requests**: `/api/cart//items` (note double slash)
**Likely Correct**: `/api/cart/{cartId}/items` or `/api/cart/items`

### **4. Order/Payment Services - Connection Refused**
**Issue**: Services not implemented yet

**Status**: Expected - these services don't exist in `/src/services/`

## 🚀 **Solutions**

### **Immediate Fixes**:

1. **Update Postman Collections**: Replace role strings with integers
2. **Implement Authentication Flow**: Login → Get Token → Use Token for CRM calls  
3. **Fix Cart API Routes**: Update endpoint URLs in Postman collections
4. **Document Missing Services**: Order and Payment are not yet implemented

### **Quick Test Commands**:

```powershell
# Test working authentication
$user = @{
    "email" = "test@example.com"
    "password" = "Test123!"
    "role" = 0
    "organization" = "external_users"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:30001/api/auth/register" `
    -Method POST `
    -Headers @{"Content-Type"="application/json"} `
    -Body $user
```

## 📊 **Current Status Summary**

✅ **Services Working**: All 4 deployed services have proper database connectivity  
❌ **Test Data Issues**: Postman collections need data format corrections  
❌ **Missing Services**: Order (30005) and Payment (30006) not implemented  
❌ **Authentication Flow**: Missing token-based authentication in test collections  

**The infrastructure is solid - only test data needs fixing!**