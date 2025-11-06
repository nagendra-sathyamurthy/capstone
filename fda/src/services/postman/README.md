# Postman Configuration for Capstone Services

This directory contains Postman environment and collection files for testing the Capstone microservices when running locally through VS Code debugging.

## Files

- **`Capstone-Local-Debug.postman_environment.json`** - Environment configuration with service URLs and variables
- **`Capstone-Services.postman_collection.json`** - API collection with sample requests for all services

## Service URLs (Local Debug)

When running services through VS Code "Debug All Services":

| Service | URL | Port |
|---------|-----|------|
| 🔐 Authentication | http://localhost:5038 | 5038 |
| 📦 Catalog | http://localhost:5270 | 5270 |
| 👥 CRM | http://localhost:5023 | 5023 |
| 🛒 Cart | http://localhost:5124 | 5124 |

## Setup Instructions

### 1. Import Environment
1. Open Postman
2. Click "Import" in the top left
3. Select `Capstone-Local-Debug.postman_environment.json`
4. The environment will appear in your environments list

### 2. Import Collection
1. In Postman, click "Import"
2. Select `Capstone-Services.postman_collection.json`
3. The collection will appear in your collections list

### 3. Select Environment
1. In the top right of Postman, click the environment dropdown
2. Select "Capstone Local Debug"

## Usage Workflow

### Prerequisites
- Start all services using VS Code "Debug All Services" (F5)
- Ensure all four services are running without errors

### Testing Flow

1. **Health Checks** - Test each service is responding:
   - Authentication Service → Health Check
   - Catalog Service → Health Check
   - CRM Service → Health Check
   - Cart Service → Health Check

2. **Authentication** - Get JWT token:
   - Run "Register User" (optional, if user doesn't exist)
   - Run "Login User" - This automatically saves the JWT token to environment

3. **Test Services** (JWT token now available for authenticated requests):
   - **Catalog**: Create items, retrieve items
   - **CRM**: Create customers, retrieve customers  
   - **Cart**: Add items to cart, retrieve cart

### Environment Variables

The environment includes these variables that get automatically populated:

| Variable | Description | Auto-populated by |
|----------|-------------|-------------------|
| `jwt_token` | JWT bearer token | Login User request |
| `user_id` | Current user ID | Login User request |
| `customer_id` | Sample customer ID | Create Customer request |
| `item_id` | Sample item ID | Create Item request |
| `cart_id` | Sample cart ID | Add Item to Cart request |

### Swagger Documentation

Each service also provides Swagger UI for API exploration:
- Authentication: http://localhost:5038/
- Catalog: http://localhost:5270/
- CRM: http://localhost:5023/
- Cart: http://localhost:5124/

## Troubleshooting

### Services Not Responding
- Ensure all services are running in VS Code debugger
- Check VS Code terminal output for any service startup errors
- Verify ports are not blocked by firewall

### Authentication Errors
- Run "Login User" request first to get JWT token
- Check that `jwt_token` environment variable is populated
- Ensure the token hasn't expired (re-login if needed)

### MongoDB Connection Issues
- Ensure MongoDB is running locally or connection strings are properly configured
- Check service logs in VS Code terminal for database connection errors

## Extending the Collection

To add new requests:
1. Create new request in appropriate service folder
2. Use environment variables like `{{auth_base_url}}`, `{{jwt_token}}`
3. Add test scripts to auto-populate environment variables from responses
4. Follow the existing naming convention

## Example Test Script

To auto-save response data to environment variables:

```javascript
if (pm.response.code === 200) {
    const response = pm.response.json();
    if (response.id) {
        pm.environment.set('entity_id', response.id);
    }
}
```

This setup provides a complete testing environment for all Capstone services running in local development mode.