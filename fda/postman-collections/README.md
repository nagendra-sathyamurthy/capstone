# Capstone E-Commerce Postman Collections

This directory contains comprehensive Postman collections for testing all microservices in the Capstone e-commerce platform.

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

8. **Capstone-Environment.postman_environment.json**
   - Environment variables for all services
   - Shared authentication tokens
   - Base URLs for each service
   - Test data variables

## Setup Instructions

### 1. Import Collections and Environment

1. Open Postman
2. Click **Import** button
3. Import all `.postman_collection.json` files
4. Import the `Capstone-Environment.postman_environment.json` file
5. Select the "Capstone Services Environment" from the environment dropdown

### 2. Configure Service URLs

Update the environment variables if your services run on different ports:

- `auth_base_url`: Authentication service URL
- `catalog_base_url`: Catalog service URL
- `cart_base_url`: Cart service URL
- `crm_base_url`: CRM service URL
- `order_base_url`: Order service URL
- `payment_base_url`: Payment service URL

### 3. Start Services

Ensure all microservices are running:

```powershell
# Start individual services or use docker-compose
cd c:\dotnet\capstone\fda\src\services
# Start each service as needed
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
3. Select the "Capstone Services Environment"
4. Run all requests in sequence

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

### Environment Reset

To reset environment variables between test runs:
1. Go to Environment settings
2. Clear values for: `auth_token`, `cart_id`, `item_id`, `customer_id`, `order_id`, `payment_id`
3. Or duplicate the environment for fresh testing

## API Documentation

Each collection includes detailed descriptions for:
- Request purposes and functionality
- Required headers and authentication
- Request body schemas and examples
- Expected response formats
- Error handling scenarios

## Integration with CI/CD

These collections can be integrated with automated testing:

```bash
# Run collections via Newman (Postman CLI)
newman run Authentication-Service.postman_collection.json -e Capstone-Environment.postman_environment.json
newman run Capstone-Workflow.postman_collection.json -e Capstone-Environment.postman_environment.json
```

## Support

For issues or questions regarding these Postman collections:
1. Verify all microservices are running and accessible
2. Check environment variable configurations
3. Review service logs for specific error details
4. Ensure proper authentication flow completion