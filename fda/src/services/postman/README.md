# Food Delivery API Testing Framework# Capstone Services Postman Collection - Local Debug Testing



This folder contains the comprehensive testing framework for the Food Delivery microservices, including Postman collections, environment configurations, automated test scripts, and detailed test results.This folder contains comprehensive Postman collection and environment files for testing the Capstone microservices locally during VS Code development and debugging.



## 📁 Folder Structure## 📁 Files



```- `Capstone-Services.postman_collection.json` - Enhanced API collection with test scripts

postman/- `Capstone-Local-Debug.postman_environment.json` - Local development environment with dynamic variables

├── collections/           # Postman collections- `README.md` - This documentation file

│   └── Food-Delivery-API-Collection.postman_collection.json

├── environments/         # Environment configurations  ## 🚀 Quick Setup

│   ├── food-delivery-environment.json

│   └── local-environment.json### 1. Import into Postman

├── scripts/             # PowerShell automation scripts1. Open Postman

│   ├── run-food-delivery-tests.ps12. Import both JSON files:

│   ├── test-food-menu-api.ps1   - Collection: `Capstone-Services.postman_collection.json`

│   └── stop-services.ps1   - Environment: `Capstone-Local-Debug.postman_environment.json`

├── test-results/        # Test execution reports3. Select "Capstone Local Debug Environment" in the environment dropdown

│   ├── newman-comprehensive-results.json

│   ├── food-delivery-api-test-summary.md### 2. Start All Services

│   └── food-menu-test-*.html/jsonEnsure all four services are running locally in VS Code debug mode:

└── README.md           # This documentation

``````bash

# Service URLs and Ports

## 🚀 Quick StartAuthentication Service: http://localhost:5038

Catalog Service:        http://localhost:5270

### 1. Import into PostmanCRM Service:           http://localhost:5023

```bashCart Service:          http://localhost:5124

# Import the collection and environment```

collections/Food-Delivery-API-Collection.postman_collection.json

environments/food-delivery-environment.json### 3. Verify Services

```Use the "Service Health Check" requests in each service folder to confirm all services are running.



### 2. Run Automated Tests## 🧪 Testing Workflows

```powershell

# Complete service testing with Newman CLI### Individual Service Testing

.\scripts\run-food-delivery-tests.ps11. **🔐 Authentication Service**

   - Register a new user (generates random email automatically)

# Focus on catalog service only   - Login to get JWT token (saves token automatically)

.\scripts\test-food-menu-api.ps1   - Test token validation

   - Get user profile

# Stop all services

.\scripts\stop-services.ps12. **📦 Catalog Service**

```   - View all items

   - Create new items

### 3. View Results   - Search/filter items

Check `test-results/` folder for detailed reports and analysis.

3. **👥 CRM Service**

## 🍔 Food Delivery API Collection   - Create customer profiles

   - Manage customer data

### Comprehensive Test Coverage (66 Assertions)   - Search customers



#### 🏥 Health & Status Monitoring4. **🛒 Cart Service**

- Service health checks   - Create shopping carts

- Swagger documentation access   - Add items to cart

- Service version validation   - Remove items from cart

   - Manage cart contents

#### 🍽️ Menu Browsing & Management

- Complete menu retrieval### Integration Testing

- Menu item detailsUse the "🧪 Integration Test Scenarios" folder for end-to-end testing:

- Administrative menu operations1. Complete user journey from registration to cart creation

2. Multi-service workflows

#### 🏷️ Category & Cuisine Organization3. Data consistency testing

- Category filtering (Appetizers, Mains, Desserts, Beverages)

- Cuisine filtering (Italian, Mexican, Indian, American, Asian)## 🔧 Features

- Category-specific menu browsing

### Automatic Variable Management

#### 🌱 Dietary Preferences- **Dynamic Data Generation**: Emails, timestamps, and names are generated automatically

- Vegetarian menu filtering- **Token Management**: JWT tokens are captured and stored automatically from login requests

- Vegan options- **ID Tracking**: User, customer, item, and cart IDs are saved automatically for subsequent requests

- Gluten-free selections

- Dietary preference combinations### Built-in Test Scripts

- **Response Validation**: All requests include test scripts to validate responses

#### 🌶️ Advanced Filtering- **Performance Testing**: Response time validation for all endpoints

- Spice level filtering (Mild, Medium, Spicy, Extra Spicy)- **Error Handling**: Proper error detection and reporting

- Price range filtering and sorting- **Debug Logging**: Console logging for troubleshooting

- Quick preparation filtering (<30 minutes)

- Multi-criteria advanced filtering### Environment Variables

The environment includes these auto-managed variables:

#### 🔍 Search Functionality- `jwt_token` - JWT authentication token

- Name-based search- `current_user_id` - Logged-in user ID

- Description-based search- `customer_id` - Created customer ID

- Combined search criteria- `item_id` - Created item ID

- `cart_id` - Created cart ID

#### 🔐 Authentication & Security- `random_email` - Generated unique email addresses

- JWT token-protected admin operations- `timestamp` - Current timestamp for unique data

- Unauthorized access validation

- Secure menu management## 📊 Service Details



## 🛠️ Service Architecture### 🔐 Authentication Service (Port 5038)

**Endpoints:**

### Food Menu Catalog Service (Port 5002)- `POST /api/auth/register` - User registration

- **Base URL**: `http://localhost:5002`- `POST /api/auth/login` - User authentication

- **Swagger**: `http://localhost:5002/swagger`- `GET /api/auth/profile` - Get user profile

- **Health**: `http://localhost:5002/api/health`- `POST /api/auth/validate` - Token validation



#### Core Features**Features:**

1. **Menu Management**: Complete CRUD operations for menu items- JWT token generation and validation

2. **Category System**: Organized food categories and cuisines- User account management

3. **Dietary Filters**: Comprehensive dietary preference support- Secure authentication flow

4. **Search Engine**: Advanced search and filtering capabilities

5. **Price Management**: Flexible pricing with UOM support### 📦 Catalog Service (Port 5270)

6. **Preparation Time**: Cook time tracking and filtering**Endpoints:**

7. **Admin Protection**: JWT-secured administrative operations- `GET /api/items` - List all items

- `POST /api/items` - Create new item

#### Menu Item Properties (25+ Fields)- `GET /api/items/{id}` - Get item by ID

- **Basic Info**: Name, description, price, preparation time- `PUT /api/items/{id}` - Update item

- **Classification**: Category, cuisine, spice level- `DELETE /api/items/{id}` - Delete item

- **Dietary**: Vegetarian, vegan, gluten-free flags

- **Packaging**: Size, unit of measure, serving details**Features:**

- **Nutritional**: Calories, allergen information- Product catalog management

- **Media**: Image URLs and descriptions- Inventory tracking

- **Availability**: In-stock status and popularity ratings- Search and filtering



## 🧪 Testing Framework### 👥 CRM Service (Port 5023)

**Endpoints:**

### Newman CLI Integration- `GET /api/customer` - List all customers

- **Collection**: 20 endpoints with 66 comprehensive assertions- `POST /api/customer` - Create new customer

- **Environment**: Production-ready configuration- `GET /api/customer/{id}` - Get customer by ID

- **Automation**: PowerShell scripts for CI/CD integration- `PUT /api/customer/{id}` - Update customer

- **Reporting**: HTML, JSON, and Markdown formats- `DELETE /api/customer/{id}` - Delete customer



### Test Categories**Features:**

1. **Health Monitoring** (3 endpoints)- Customer profile management

2. **Menu Browsing** (2 endpoints)  - Contact information tracking

3. **Categories & Cuisines** (5 endpoints)- Customer relationship management

4. **Dietary Preferences** (4 endpoints)

5. **Search & Filtering** (4 endpoints)### 🛒 Cart Service (Port 5124)

6. **Admin Operations** (2 endpoints)**Endpoints:**

- `POST /api/cart` - Create new cart

### Success Metrics- `GET /api/cart/{id}` - Get cart by ID

- **Target Success Rate**: 95%+ (63-66/66 assertions)- `POST /api/cart/{cartId}/items` - Add item to cart

- **Performance**: <50ms average response time- `DELETE /api/cart/{cartId}/items/{itemId}` - Remove item from cart

- **Coverage**: All major food delivery workflows

- **Authentication**: Complete JWT validation**Features:**

- Shopping cart management

## 📊 Test Results & Analysis- Item quantity tracking

- Cart persistence

### Latest Test Execution

- **Date**: November 6, 2025## 🐛 Debugging Tips

- **Total Requests**: 20

- **Total Assertions**: 66### Service Not Responding

- **Success Rate**: 15% (MongoDB connectivity issue)1. Check VS Code terminal for service startup messages

- **Working Features**: Service health, authentication, API structure2. Verify ports are not in use by other processes

3. Use health check endpoints to verify service status

### Known Issues4. Check MongoDB connection (services require MongoDB)

- **MongoDB Connection**: Services configured for `mongodb:27017` instead of `localhost:27017`

- **Resolution**: Update connection strings and restart MongoDB service### Authentication Issues

- **Expected Post-Fix**: 95%+ success rate1. Ensure you've run the "Login User" request first

2. Check that JWT token is saved in environment variables

### Performance Metrics3. Verify token hasn't expired

- **Working Endpoints**: 32.95ms average response time4. Check Authorization headers in requests

- **Service Status**: Healthy infrastructure

- **Authentication**: 100% working JWT protection### Test Failures

- **Error Handling**: Proper HTTP status codes1. Review Postman console for detailed error logs

2. Check service logs in VS Code terminals

## 🔧 Configuration & Setup3. Verify request body formats match API expectations

4. Ensure proper sequence for dependent requests

### Prerequisites

- .NET 9.0 SDK## 🔄 Development Workflow

- MongoDB running on localhost:27017

- Newman CLI (npm install -g newman)1. **Start Services**: Launch all 4 services in VS Code debug mode

- PowerShell 5.1+2. **Health Check**: Run health check requests to verify all services

3. **Authentication**: Register and login to get JWT token

### Environment Variables4. **Service Testing**: Test individual service endpoints

```json5. **Integration Testing**: Run complete user journey scenarios

{6. **Debug Issues**: Use VS Code breakpoints and Postman console for troubleshooting

  "catalog_base_url": "http://localhost:5002",

  "auth_base_url": "http://localhost:5001",## 📝 Environment Configuration

  "jwt_token": "Bearer {{token}}"

}The collection is pre-configured for local development with:

```- Standard debug ports (5038, 5270, 5023, 5124)

- Automatic token management

### Service Dependencies- Dynamic test data generation

- **Authentication Service**: JWT token generation- Comprehensive error handling

- **MongoDB Database**: Menu data persistence- Performance monitoring

- **Swagger UI**: API documentation

- **Health Endpoints**: Service monitoringFor production or different environments, update the base URLs in the environment file accordingly.

## 🚀 Production Deployment

### Docker Configuration
Services include Dockerfile for containerized deployment with proper MongoDB connection strings.

### CI/CD Integration
```yaml
# Example GitHub Actions integration
- name: Run Food Delivery API Tests
  run: |
    cd postman/scripts
    .\run-food-delivery-tests.ps1
    # Publish test results to artifacts
```

### Monitoring & Observability
- Health check endpoints for load balancers
- Structured logging for operations teams
- Performance metrics collection
- Error tracking and alerting

## 📋 Next Steps

1. **Fix MongoDB Connection**: Update connection strings to `localhost:27017`
2. **Complete Validation**: Achieve 95%+ test success rate
3. **Data Seeding**: Populate sample menu data
4. **Performance Testing**: Load testing with realistic data volumes
5. **Production Deployment**: Container orchestration and monitoring

## 🤝 Contributing

### Adding New Tests
1. Update the Postman collection with new requests
2. Add corresponding assertions and validation scripts
3. Update automation scripts if needed
4. Document new test scenarios in this README

### Environment Updates
1. Modify environment JSON files
2. Update automation scripts for new variables
3. Test with both local and production configurations
4. Update documentation with new requirements

---

**Food Delivery API Testing Framework v2.0.0**  
Complete testing solution for production-ready food delivery microservices.