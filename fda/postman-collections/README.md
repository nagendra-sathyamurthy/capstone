# Food Delivery App (FDA) - Postman Collections

This directory contains comprehensive Postman collections for testing all microservices in the Food Delivery App platform across different deployment environments.

## Collections Overview

### Individual Service Collections

1. **Authentication-Service.postman_collection.json**
   - User registration and login
   - JWT token management
   - Password reset functionality
   - Base URL: `https://localhost:5001`

2. **Catalog-Service.postman_collection.json**
   - Product/item management (CRUD operations)
   - Bulk item creation
   - Item search functionality
   - Base URL: `http://localhost:5002`

3. **CRM-Service.postman_collection.json**
   - Customer management (CRUD operations)
   - Bulk customer creation
   - Requires authentication token
   - Base URL: `http://localhost:5003`

4. **Cart-Service.postman_collection.json**
   - Shopping cart functionality with item management
   - Add/remove/update cart items
   - Cart total calculations
   - Base URL: `http://localhost:5004`

5. **Order-Service.postman_collection.json**
   - Order creation and management
   - Order status tracking
   - Order history and search
   - Base URL: `http://localhost:5005` (when deployed)

6. **Payment-Service.postman_collection.json**
   - Payment processing (Credit Card, PayPal)
   - Payment validation and refunds
   - Payment history tracking
   - Base URL: `http://localhost:5006` (when deployed)

3. **CRM-Service.postman_collection.json**
   - Customer management (CRUD operations)
   - Bulk customer creation
   - Requires authentication token
   - Base URL: `http://localhost:5000`

4. **Cart-Service.postman_collection.json**
   - Shopping cart management
   - Add/remove/update cart items
   - Cart total calculations
   - Base URL: `http://localhost:5124`

5. **Order-Service.postman_collection.json**
   - Order creation and management
   - Order status tracking
   - Order history and search
   - Base URL: `http://localhost:5200`

6. **Payment-Service.postman_collection.json**
   - Payment processing (Credit Card, PayPal)
   - Payment validation and refunds
   - Payment history tracking
   - Base URL: `http://localhost:5300`

### Orchestration Collections

7. **Capstone-Workflow.postman_collection.json**
   - Complete end-to-end e-commerce workflow
   - Tests integration between all services
   - Automated variable management between requests
   - Includes test assertions

### Environment Configuration Files

8. **Capstone-Local-Environment.postman_environment.json**
   - Local development environment configuration
   - Shared MongoDB architecture (resource efficient)
   - NodePort service access (localhost:30001-30006)
   - Test data variables for development

9. **Capstone-Production-Environment.postman_environment.json**
   - Production environment configuration
   - Dedicated MongoDB per service architecture
   - LoadBalancer service access (internal Kubernetes services)
   - Production-ready test data variables

10. **Capstone-Environment.postman_environment.json** *(Deprecated)*
    - Legacy unified environment configuration
    - Use environment-specific files instead

## Setup Instructions

### 1. Import Collections and Environments

1. Open Postman
2. Click **Import** button
3. Import all `.postman_collection.json` files
4. Import **both environment files**:
   - `Capstone-Local-Environment.postman_environment.json`
   - `Capstone-Production-Environment.postman_environment.json`
5. Select the appropriate environment from the dropdown:
   - **Local Development**: "Capstone Local Development Environment"
   - **Production Testing**: "Capstone Production Environment"

**🔐 Security Note**: Both environments include MongoDB credentials with strong Athena authentication that match your deployment configuration.

### Environment Selection Guide

#### Local Development Environment
- **Use When**: Developing locally, testing individual features, resource-constrained environments
- **Architecture**: Shared MongoDB instance for all services
- **Service URLs**: NodePort access (localhost:30001-30006)
- **Deploy Command**: `.\devops\kubernetes\deploy-local.ps1`

#### Production Environment
- **Use When**: Production testing, performance testing, scalability testing
- **Architecture**: Dedicated MongoDB instance per service
- **Service URLs**: LoadBalancer access (internal Kubernetes services)
- **Deploy Command**: `.\devops\kubernetes\deploy-production.ps1`

### 2. Environment-Specific Configuration

**Local Development URLs** (when using Local Environment):
- `auth_base_url`: `http://localhost:30001`
- `catalog_base_url`: `http://localhost:30002`
- `crm_base_url`: `http://localhost:30003`
- `cart_base_url`: `http://localhost:30004`
- `order_base_url`: `http://localhost:30005`
- `payment_base_url`: `http://localhost:30006`

**Production URLs** (when using Production Environment):
- `auth_base_url`: `http://authentication-service:8080`
- `catalog_base_url`: `http://catalog-service:8080`
- `crm_base_url`: `http://crm-service:8080`
- `cart_base_url`: `http://cart-service:8080`
- `order_base_url`: `http://order-service:8080`
- `payment_base_url`: `http://payment-service:8080`

### 3. Deploy Services

Deploy services based on your testing needs:

**Local Development Deployment:**
```powershell
cd c:\dotnet\capstone\fda\devops\kubernetes
.\deploy-local.ps1
```

**Production Deployment:**
```powershell
cd c:\dotnet\capstone\fda\devops\kubernetes
.\deploy-production.ps1
```

**Verify Deployment:**
```powershell
.\show-architecture.ps1
```

**Clean Environment (when switching):**
```powershell
.\cleanup-environment.ps1
```

## Usage Guide

### Testing Individual Services

1. **Authentication Service**
   - Start with "User Registration" to create a test user
   - Use "User Login" to get authentication token
   - Token is automatically saved to environment variables

2. **Catalog Service**
   - Use "Create Item" or "Create Multiple Items (Bulk)" to populate catalog
   - Test CRUD operations with individual items
   - Item IDs are automatically captured for subsequent requests

3. **CRM Service**
   - Requires authentication token (login first)
   - Create customer profiles for testing
   - Customer IDs are automatically captured

4. **Cart Service**
   - Create cart and add items from catalog
   - Test cart management operations
   - Cart IDs are automatically managed

5. **Order Service**
   - Create orders from cart contents
   - Track order status and history
   - Order IDs are captured for payment processing

6. **Payment Service**
   - Process payments for created orders
   - Test different payment methods
   - Handle refunds and payment validation

### End-to-End Testing

Run the **Capstone-Workflow** collection for complete integration testing:

1. Select the "Capstone E-Commerce Workflow" collection
2. Click **Run** (or use Collection Runner)
3. Select the appropriate environment:
   - "Capstone Local Development Environment" for local testing
   - "Capstone Production Environment" for production testing
4. Run all requests in sequence

**Environment Switching**: When changing between local and production deployments, always:
1. Switch the Postman environment
2. Re-authenticate to get environment-appropriate tokens
3. Verify service connectivity before running full workflows

The workflow automatically:
- Registers a new user
- Authenticates and captures token
- Creates customer profile
- Adds products to catalog
- Creates and populates shopping cart
- Creates order from cart
- Processes payment
- Verifies final order status

### Variables and Authentication

The collections use several automatic features:

- **Auto Token Management**: Login requests automatically save JWT tokens
- **ID Capture**: Created resource IDs are automatically captured and reused
- **Environment Sharing**: Variables are shared across all collections
- **Test Assertions**: Workflow includes automated test verification

### Common Variables

- `{{auth_token}}`: JWT token from authentication service
- `{{user_id}}`: Current user identifier
- `{{cart_id}}`: Active shopping cart ID
- `{{item_id}}`: Product/item identifier
- `{{customer_id}}`: Customer profile ID
- `{{order_id}}`: Order identifier
- `{{payment_id}}`: Payment transaction ID

### Realistic Test Data

The collections now include comprehensive real-world test data:

**User Personas**:
- **Sarah Martinez** (sarah.martinez@gmail.com) - Busy Professional
- **Emily Chen** (emily.chen@gmail.com) - Creative Professional (Vegetarian)
- **Alex Johnson** (alex.johnson@gmail.com) - Marketing Executive
- **Michael Rodriguez** (mike.rodriguez@gmail.com) - Team Leader (Office Orders)
- **Jessica Thompson** (jess.thompson@student.university.edu) - Graduate Student (Vegan)
- **David Kim** (david.kim@creativestudio.com) - Creative Director (Premium Customer)

**Food Categories**:
- Pizza (Tony's Authentic Pizzeria)
- Indian Cuisine (Spice Garden Indian Cuisine)
- Sushi (Sakura Sushi Bar)
- American Food (Burger Junction)
- Thai Food (Bangkok Street Kitchen)
- Healthy Options (Fresh & Green Cafe)
- Desserts (Sweet Endings Bakery)

**Realistic Delivery Scenarios**:
- Family dinner orders ($60-90)
- Office lunch catering ($100-200)
- Student budget meals ($15-25)
- Premium fine dining delivery ($75-150)
- Late-night comfort food ($15-40)

See `TEST-DATA-REFERENCE.md` for complete persona, restaurant, and menu details.

### MongoDB Configuration Variables

**Common Across Environments:**
- `{{mongodb_username}}`: MongoDB username (Athena)
- `{{mongodb_password}}`: MongoDB password (Ath3n@Str0ngP@ssw0rd2024!)
- `{{mongodb_auth_source}}`: MongoDB authentication source (admin)

**Local Development Environment:**
- `{{mongodb_host_shared}}`: Shared MongoDB host (mongodb-service:27017)
- `{{mongodb_connection_auth}}`: Authentication DB (shared MongoDB)
- `{{mongodb_connection_catalog}}`: Catalog DB (shared MongoDB)
- `{{mongodb_connection_crm}}`: CRM DB (shared MongoDB)
- `{{mongodb_connection_cart}}`: Cart DB (shared MongoDB)

**Production Environment:**
- `{{mongodb_host_auth}}`: Dedicated Authentication MongoDB (authentication-mongodb-service:27017)
- `{{mongodb_host_catalog}}`: Dedicated Catalog MongoDB (catalog-mongodb-service:27017)
- `{{mongodb_host_crm}}`: Dedicated CRM MongoDB (crm-mongodb-service:27017)
- `{{mongodb_host_cart}}`: Dedicated Cart MongoDB (cart-mongodb-service:27017)
- Environment-specific connection strings for each service's dedicated database

## Troubleshooting

### Common Issues

1. **Connection Refused**: Verify service is running on correct port
2. **401 Unauthorized**: Ensure you've logged in and have valid auth token
3. **404 Not Found**: Check that resource IDs are correctly captured
4. **SSL Certificate**: For HTTPS endpoints, you may need to disable SSL verification in Postman settings

### Service Dependencies

Services should be started in this order for full functionality:
1. Authentication Service
2. Catalog Service
3. CRM Service
4. Cart Service
5. Order Service
6. Payment Service

### Environment Reset and Switching

**Reset Variables Between Test Runs:**
1. Go to Environment settings in Postman
2. Clear dynamic values for: `auth_token`, `cart_id`, `item_id`, `customer_id`, `order_id`, `payment_id`
3. Or duplicate the environment for fresh testing

**Switching Between Environments:**
1. **Deploy Target Environment**:
   - For Local: Run `.\deploy-local.ps1`
   - For Production: Run `.\deploy-production.ps1`
2. **Switch Postman Environment**:
   - Select appropriate environment from dropdown
3. **Re-authenticate**:
   - Run authentication requests to get new tokens
   - Tokens are environment-specific
4. **Verify Connectivity**:
   - Test a simple service request before running workflows

### MongoDB Connection Testing

Both environments include MongoDB connection strings for debugging:

**Local Development Testing:**
1. **Shared Database Access**: All services use single MongoDB instance
2. **NodePort Access**: Direct MongoDB access via `http://localhost:30000`
3. **Resource Efficiency**: Minimal pod count for development

**Production Testing:**
1. **Dedicated Database Access**: Each service has isolated MongoDB instance
2. **Service-to-Service**: Internal Kubernetes service communication
3. **Scalability**: Independent scaling per service database

**Connection Verification Steps:**
1. Use environment-specific connection strings
2. Test authentication with Athena credentials
3. Verify database separation (production) or sharing (local)
4. Check service-specific database access

**Security Note**: MongoDB credentials are marked as "secret" type in both environments to prevent accidental exposure in shared collections.

## API Documentation

Each collection includes detailed descriptions for:
- Request purposes and functionality
- Required headers and authentication
- Request body schemas and examples
- Expected response formats
- Error handling scenarios

## Integration with CI/CD

These collections can be integrated with automated testing using environment-specific configurations:

```bash
# Local Development Testing
newman run Authentication-Service.postman_collection.json -e Capstone-Local-Environment.postman_environment.json
newman run Capstone-Workflow.postman_collection.json -e Capstone-Local-Environment.postman_environment.json

# Production Testing
newman run Authentication-Service.postman_collection.json -e Capstone-Production-Environment.postman_environment.json
newman run Capstone-Workflow.postman_collection.json -e Capstone-Production-Environment.postman_environment.json
```

**Environment-Specific CI/CD Strategy:**
- Use Local environment for development branch testing
- Use Production environment for staging/production validation
- Separate test data and credentials per environment

## Support

For issues or questions regarding these Postman collections:
1. Verify all microservices are running and accessible
2. Check environment variable configurations
3. Review service logs for specific error details
4. Ensure proper authentication flow completion