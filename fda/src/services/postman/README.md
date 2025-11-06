# Capstone Services Postman Collection - Local Debug Testing

This folder contains comprehensive Postman collection and environment files for testing the Capstone microservices locally during VS Code development and debugging.

## 📁 Files

- `Capstone-Services.postman_collection.json` - Enhanced API collection with test scripts
- `Capstone-Local-Debug.postman_environment.json` - Local development environment with dynamic variables
- `README.md` - This documentation file

## 🚀 Quick Setup

### 1. Import into Postman
1. Open Postman
2. Import both JSON files:
   - Collection: `Capstone-Services.postman_collection.json`
   - Environment: `Capstone-Local-Debug.postman_environment.json`
3. Select "Capstone Local Debug Environment" in the environment dropdown

### 2. Start All Services
Ensure all four services are running locally in VS Code debug mode:

```bash
# Service URLs and Ports
Authentication Service: http://localhost:5038
Catalog Service:        http://localhost:5270
CRM Service:           http://localhost:5023
Cart Service:          http://localhost:5124
```

### 3. Verify Services
Use the "Service Health Check" requests in each service folder to confirm all services are running.

## 🧪 Testing Workflows

### Individual Service Testing
1. **🔐 Authentication Service**
   - Register a new user (generates random email automatically)
   - Login to get JWT token (saves token automatically)
   - Test token validation
   - Get user profile

2. **📦 Catalog Service**
   - View all items
   - Create new items
   - Search/filter items

3. **👥 CRM Service**
   - Create customer profiles
   - Manage customer data
   - Search customers

4. **🛒 Cart Service**
   - Create shopping carts
   - Add items to cart
   - Remove items from cart
   - Manage cart contents

### Integration Testing
Use the "🧪 Integration Test Scenarios" folder for end-to-end testing:
1. Complete user journey from registration to cart creation
2. Multi-service workflows
3. Data consistency testing

## 🔧 Features

### Automatic Variable Management
- **Dynamic Data Generation**: Emails, timestamps, and names are generated automatically
- **Token Management**: JWT tokens are captured and stored automatically from login requests
- **ID Tracking**: User, customer, item, and cart IDs are saved automatically for subsequent requests

### Built-in Test Scripts
- **Response Validation**: All requests include test scripts to validate responses
- **Performance Testing**: Response time validation for all endpoints
- **Error Handling**: Proper error detection and reporting
- **Debug Logging**: Console logging for troubleshooting

### Environment Variables
The environment includes these auto-managed variables:
- `jwt_token` - JWT authentication token
- `current_user_id` - Logged-in user ID
- `customer_id` - Created customer ID
- `item_id` - Created item ID
- `cart_id` - Created cart ID
- `random_email` - Generated unique email addresses
- `timestamp` - Current timestamp for unique data

## 📊 Service Details

### 🔐 Authentication Service (Port 5038)
**Endpoints:**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User authentication
- `GET /api/auth/profile` - Get user profile
- `POST /api/auth/validate` - Token validation

**Features:**
- JWT token generation and validation
- User account management
- Secure authentication flow

### 📦 Catalog Service (Port 5270)
**Endpoints:**
- `GET /api/items` - List all items
- `POST /api/items` - Create new item
- `GET /api/items/{id}` - Get item by ID
- `PUT /api/items/{id}` - Update item
- `DELETE /api/items/{id}` - Delete item

**Features:**
- Product catalog management
- Inventory tracking
- Search and filtering

### 👥 CRM Service (Port 5023)
**Endpoints:**
- `GET /api/customer` - List all customers
- `POST /api/customer` - Create new customer
- `GET /api/customer/{id}` - Get customer by ID
- `PUT /api/customer/{id}` - Update customer
- `DELETE /api/customer/{id}` - Delete customer

**Features:**
- Customer profile management
- Contact information tracking
- Customer relationship management

### 🛒 Cart Service (Port 5124)
**Endpoints:**
- `POST /api/cart` - Create new cart
- `GET /api/cart/{id}` - Get cart by ID
- `POST /api/cart/{cartId}/items` - Add item to cart
- `DELETE /api/cart/{cartId}/items/{itemId}` - Remove item from cart

**Features:**
- Shopping cart management
- Item quantity tracking
- Cart persistence

## 🐛 Debugging Tips

### Service Not Responding
1. Check VS Code terminal for service startup messages
2. Verify ports are not in use by other processes
3. Use health check endpoints to verify service status
4. Check MongoDB connection (services require MongoDB)

### Authentication Issues
1. Ensure you've run the "Login User" request first
2. Check that JWT token is saved in environment variables
3. Verify token hasn't expired
4. Check Authorization headers in requests

### Test Failures
1. Review Postman console for detailed error logs
2. Check service logs in VS Code terminals
3. Verify request body formats match API expectations
4. Ensure proper sequence for dependent requests

## 🔄 Development Workflow

1. **Start Services**: Launch all 4 services in VS Code debug mode
2. **Health Check**: Run health check requests to verify all services
3. **Authentication**: Register and login to get JWT token
4. **Service Testing**: Test individual service endpoints
5. **Integration Testing**: Run complete user journey scenarios
6. **Debug Issues**: Use VS Code breakpoints and Postman console for troubleshooting

## 📝 Environment Configuration

The collection is pre-configured for local development with:
- Standard debug ports (5038, 5270, 5023, 5124)
- Automatic token management
- Dynamic test data generation
- Comprehensive error handling
- Performance monitoring

For production or different environments, update the base URLs in the environment file accordingly.