# Food Delivery API Testing - Service-Specific Collections

This directory contains comprehensive API testing for the Food Delivery microservices using Postman collections and Newman automation. The collections have been separated by service for better organization and independent testing workflows.

## 📁 Structure

```
postman/
├── collections/                          # Service-specific Postman collections
│   ├── Authentication-Service.postman_collection.json
│   ├── Food-Menu-Catalog.postman_collection.json
│   ├── CRM-Service.postman_collection.json
│   ├── Cart-Service.postman_collection.json
│   └── Legacy-Catalog.postman_collection.json
├── environments/                         # Environment configurations
│   └── Food-Delivery-Local.postman_environment.json
├── scripts/                             # Automation scripts
│   ├── run-food-delivery-tests.ps1     # Main test runner (all services)
│   ├── test-auth-service.ps1           # Authentication service only
│   ├── test-menu-service.ps1           # Food Menu Catalog service only
│   └── stop-services.ps1               # Stop all services
└── test-results/                        # Generated test reports
```

## 🚀 Quick Start

### Run All Services & Tests
```powershell
.\scripts\run-food-delivery-tests.ps1
```

### Run Specific Service Tests
```powershell
# Authentication Service only
.\scripts\test-auth-service.ps1

# Food Menu Catalog Service only
.\scripts\test-menu-service.ps1

# Custom collection selection
.\scripts\run-food-delivery-tests.ps1 -Collections @("Authentication Service", "Food Menu Catalog")
```

### Keep Services Running for Manual Testing
```powershell
.\scripts\run-food-delivery-tests.ps1 -KeepServicesRunning
```

## 📚 Service Collections

### 🔐 Authentication Service Collection
**File:** `collections/Authentication-Service.postman_collection.json`
**Service Port:** 5001
**Coverage:**
- Health checks
- User registration & login
- Token management & validation
- User profile management
- Password recovery
- Roles & permissions
- OTP authentication

**Key Endpoints:**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/auth/validate` - Token validation
- `GET /api/auth/profile` - User profile
- `POST /api/auth/generate-otp` - OTP generation
- `GET /api/auth/roles` - Available roles

### 🍽️ Food Menu Catalog Collection
**File:** `collections/Food-Menu-Catalog.postman_collection.json`
**Service Port:** 5002
**Coverage:**
- Health checks
- Menu browsing & item retrieval
- Category management
- Dietary preference filtering
- Search & advanced filtering
- Admin operations (CRUD)

**Key Endpoints:**
- `GET /api/items` - Get all menu items
- `GET /api/items/{id}` - Get specific item
- `GET /api/categories` - Get all categories
- `GET /api/items/dietary/vegetarian` - Vegetarian items
- `GET /api/items/search` - Search items
- `POST /api/items` - Create item (admin)

### 👥 CRM Service Collection
**File:** `collections/CRM-Service.postman_collection.json`
**Service Port:** 5003
**Coverage:**
- Health checks
- Customer management (CRUD)
- Customer search & analytics
- Customer preferences
- Address management

**Key Endpoints:**
- `GET /api/customers` - Get all customers
- `POST /api/customers` - Create customer
- `GET /api/customers/search` - Search customers
- `GET /api/customers/analytics` - Customer analytics
- `PUT /api/customers/{id}/preferences` - Update preferences

### 🛒 Cart Service Collection
**File:** `collections/Cart-Service.postman_collection.json`
**Service Port:** 5004
**Coverage:**
- Health checks
- Cart management
- Cart item operations
- Cart summary & calculations

**Key Endpoints:**
- `GET /api/cart` - Get user cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update item quantity
- `DELETE /api/cart/items/{id}` - Remove item
- `GET /api/cart/summary` - Cart summary

### 📚 Legacy Catalog Collection
**File:** `collections/Legacy-Catalog.postman_collection.json`
**Service Port:** 5005
**Coverage:**
- Health checks
- Legacy catalog operations (backward compatibility)
- PascalCase property formats
- Legacy CRUD operations

**Key Endpoints:**
- `GET /api/Items` - Get items (legacy format)
- `POST /api/Items` - Create item (legacy format)
- `PUT /api/Items/{id}` - Update item (legacy format)

## 🌍 Environment Configuration

The `Food-Delivery-Local.postman_environment.json` file contains all necessary environment variables:

```json
{
  "auth_base_url": "http://localhost:5001",
  "menu_base_url": "http://localhost:5002", 
  "crm_base_url": "http://localhost:5003",
  "cart_base_url": "http://localhost:5004",
  "legacy_base_url": "http://localhost:5005",
  "auth_token": "",
  "admin_email": "admin@restaurant.com",
  "admin_password": "AdminPass123!"
}
```

## 🔧 Script Parameters

### Main Test Runner (`run-food-delivery-tests.ps1`)
```powershell
-SkipBuild          # Skip building services before testing
-KeepServicesRunning # Keep services running after tests
-Collections        # Array of specific collections to test
-Sequential         # Run collections sequentially (default: true)
```

### Individual Service Scripts
```powershell
-SkipServiceStart   # Skip starting the service (if already running)
-KeepServiceRunning # Keep service running after tests
```

## 📊 Test Reports

All test runs generate comprehensive reports in the `test-results/` directory:

- **JSON Reports:** Machine-readable results for CI/CD integration
- **HTML Reports:** Beautiful visual reports with charts and graphs
- **JUnit Reports:** XML format for test result integration

Report naming convention: `newman-{service}-{timestamp}.{format}`

## 🎯 Testing Strategies

### Development Workflow
1. **Individual Service Testing:** Use service-specific scripts during development
2. **Integration Testing:** Run main script with all collections
3. **Targeted Testing:** Use `-Collections` parameter for specific service combinations

### CI/CD Integration
```powershell
# Run all tests and generate reports
.\scripts\run-food-delivery-tests.ps1 -SkipBuild

# Run specific services for pipeline stages
.\scripts\run-food-delivery-tests.ps1 -Collections @("Authentication Service") -Sequential
```

### Manual Testing Support
```powershell
# Start services and keep them running
.\scripts\run-food-delivery-tests.ps1 -KeepServicesRunning

# Access Swagger UIs:
# - Authentication: http://localhost:5001/swagger
# - Food Menu: http://localhost:5002/swagger
# - CRM: http://localhost:5003/swagger
```

## 🚦 Service Dependencies

Some collections have dependencies on others:
- **Cart Service** requires **Authentication Service** (for user tokens)
- **CRM Service** requires **Authentication Service** (for user management)
- **Food Menu Catalog** admin operations require **Authentication Service**

The main test runner handles these dependencies automatically by running collections in the correct order.

## 🔍 Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```
   ❌ Port 5001 is already in use
   ```
   **Solution:** Stop existing services or use `-SkipServiceStart` if services are already running

2. **Service Health Check Timeout**
   ```
   ❌ Authentication service health check timeout
   ```
   **Solution:** Check service logs, ensure MongoDB is running, verify build succeeded

3. **Authentication Token Issues**
   ```
   ❌ 401 Unauthorized responses
   ```
   **Solution:** Ensure Authentication Service tests run first to generate valid tokens

### Debug Mode
Add verbose logging to Newman:
```powershell
newman run collection.json --verbose
```

### Manual Service Testing
Access service endpoints directly:
- Health Check: `GET http://localhost:{port}/api/health`
- Swagger UI: `http://localhost:{port}/swagger`

## 📈 Best Practices

1. **Run Authentication Tests First:** Other services depend on valid tokens
2. **Use Environment Variables:** Configure URLs and credentials in environment file
3. **Check Service Health:** Always verify services are healthy before testing
4. **Review HTML Reports:** Visual reports provide better insight into failures
5. **Service-Specific Development:** Use individual scripts during feature development
6. **Integration Testing:** Use main script for full system validation

## 🔗 Related Files

- **Service Implementations:** `../authentication/`, `../catalog/`, `../crm/`
- **Original Monolithic Collection:** `Food-Delivery-API-Collection.postman_collection.json` (archived)
- **Service Configuration:** Each service's `appsettings.json` and `Program.cs`

---

*This documentation covers the separated service collections approach. Each collection can be imported individually into Postman for manual testing or used with Newman for automated testing.*