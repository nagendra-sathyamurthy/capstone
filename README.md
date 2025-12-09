# Food Delivery Application (FDA) - Capstone Project

A comprehensive microservices-based food delivery platform built with .NET 8.0, MongoDB, React, and Kubernetes.

## 🎯 Quick Start

### Prerequisites
- Docker Desktop with Kubernetes enabled
- .NET 8.0 SDK
- Node.js 18+
- kubectl command-line tool
- PowerShell (Windows) or Bash (Linux/macOS)
- 8GB+ RAM

### Deploy in 5 Steps

**Windows (PowerShell):**
```powershell
# Navigate to scripts directory
cd devops/jobs/powershell

# 1. Verify Kubernetes setup
.\verify.ps1

# 2. Build Docker images
.\build.ps1

# 3. Apply secrets
.\secrets.ps1

# 4. Deploy to Kubernetes
.\deploy.ps1

# 5. Seed sample data
.\seed.ps1
```

**Linux/macOS (Bash):**
```bash
# Navigate to scripts directory
cd devops/jobs/bash

# Make scripts executable (first time only)
chmod +x *.sh

# 1. Verify Kubernetes setup
./verify.sh

# 2. Build Docker images
./build.sh

# 3. Apply secrets
./secrets.sh

# 4. Deploy to Kubernetes
./deploy.sh

# 5. Seed sample data
./seed.sh
```

**Access the application:**
- Customer App: http://localhost:30080
- API Gateway: http://localhost:30005

---

## 📁 Project Structure

```
fda/
├── src/
│   ├── services/           # Backend microservices (.NET 8.0)
│   │   ├── authentication/ # User authentication & RBAC
│   │   ├── catalog/        # Menu & restaurant catalog
│   │   ├── crm/            # Customer & restaurant management
│   │   ├── cart/           # Shopping cart service
│   │   └── order/          # Order management
│   ├── gateway/            # API Gateway (Node.js/Express)
│   └── customer-app/       # Frontend (React)
├── devops/
│   └── jobs/              # Deployment & utility scripts
│       ├── powershell/    # Windows PowerShell scripts
│       │   ├── verify.ps1
│       │   ├── build.ps1
│       │   ├── secrets.ps1
│       │   ├── deploy.ps1
│       │   ├── seed.ps1
│       │   ├── test.ps1
│       │   ├── setup.ps1
│       │   └── cleanup.ps1
│       ├── bash/          # Linux/macOS Bash scripts
│       │   ├── verify.sh
│       │   ├── build.sh
│       │   ├── secrets.sh
│       │   ├── deploy.sh
│       │   ├── seed.sh
│       │   ├── test.sh
│       │   ├── setup.sh
│       │   └── cleanup.sh
│       └── data/          # Seed data files
│           └── seed-menu-items.json
└── postman-collections/   # API test collections
```

---

## 🏗️ Architecture

### Microservices Overview

The application follows a microservices architecture with:

- **5 Backend Services**: Authentication, Catalog, CRM, Cart, Order (ASP.NET Core)
- **API Gateway**: Node.js/Express for request routing and aggregation
- **Frontend**: React SPA with responsive design
- **Database**: MongoDB (shared instance for development)
- **Container Orchestration**: Kubernetes (K3s/Docker Desktop)
- **CI/CD**: PowerShell automation scripts

### System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Internet/Users                       │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │   Customer App (React) │
        │   Port: 30080          │
        └────────────┬───────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │   API Gateway (Node)   │
        │   Port: 30005          │
        └────────────┬───────────┘
                     │
        ┌────────────┴───────────────────────────┐
        │                                        │
        ▼                                        ▼
┌──────────────┐                        ┌──────────────┐
│   Backend    │                        │   Backend    │
│   Services   │◄──────────────────────►│   Services   │
│ (.NET Core)  │                        │ (.NET Core)  │
└──────┬───────┘                        └──────┬───────┘
       │                                       │
       │          ┌──────────────┐             │
       └─────────►│   MongoDB    │◄────────────┘
                  │  Port: 27017 │
                  └──────────────┘
```

### Service Ports

**Local Kubernetes:**
- MongoDB: 30000
- Authentication: 30001
- Catalog: 30002
- CRM: 30003
- Cart: 30004
- Order: 30005
- Gateway: 30500
- Customer App: 30080

---

## 🚀 Deployment Guide

### Available Deployment Scripts

The project includes cross-platform scripts for both Windows (PowerShell) and Linux/macOS (Bash):

| Script | Description | Usage |
|--------|-------------|-------|
| **verify** | Verify Kubernetes setup and connectivity | Run first to check prerequisites |
| **setup** | Setup .NET user secrets for local debugging | For VS Code F5 debugging only |
| **build** | Build all Docker images | Required before deployment |
| **secrets** | Apply Kubernetes secrets | Run before deploy |
| **deploy** | Deploy complete stack to Kubernetes | Deploys all services |
| **seed** | Seed sample menu data | Run after deployment |
| **test** | Run Newman API tests | Validate deployment |
| **cleanup** | Clean up Kubernetes resources | Remove all resources |

### Full Deployment Workflow

#### Windows (PowerShell)
```powershell
cd devops/jobs/powershell

# 1. Verify Kubernetes setup
.\verify.ps1

# 2. Build Docker images
.\build.ps1
# Time: ~10-15 minutes (first build), ~5 minutes (subsequent)

# 3. Apply MongoDB secrets
.\secrets.ps1

# 4. Deploy all services
.\deploy.ps1
# Deploys: MongoDB, Authentication, Catalog, CRM, Cart, Order, Gateway, Customer App

# 5. Seed sample data
.\seed.ps1
# Creates 10 sample menu items from data/seed-menu-items.json

# 6. Run API tests
.\test.ps1
# Runs: User Registration, Restaurant Owner, Operator workflows
```

#### Linux/macOS (Bash)
```bash
cd devops/jobs/bash

# Make scripts executable (first time only)
chmod +x *.sh

# 1. Verify Kubernetes setup
./verify.sh

# 2. Build Docker images
./build.sh

# 3. Apply MongoDB secrets
./secrets.sh

# 4. Deploy all services
./deploy.sh

# 5. Seed sample data
./seed.sh

# 6. Run API tests
./test.sh
```

### Verification

#### Check Deployment Status
```bash
# View all pods
kubectl get pods -n capstone-services
kubectl get pods -n capstone-gateway
kubectl get pods -n capstone-frontend

# View all services
kubectl get svc -A | grep capstone

# Check service logs
kubectl logs -f <pod-name> -n capstone-services
```

#### Access Points
Once deployed, access the application at:
- **Customer App**: http://localhost:30080
- **API Gateway**: http://localhost:30005
- **Authentication Service**: http://localhost:30001
- **Catalog Service**: http://localhost:30002
- **CRM Service**: http://localhost:30003
- **Cart Service**: http://localhost:30004
- **Order Service**: http://localhost:30005
- **MongoDB**: mongodb://localhost:30000

### Cleanup

#### Remove Kubernetes Resources
**Windows:**
```powershell
# Remove all Kubernetes resources (keeps Docker images)
.\cleanup.ps1

# Remove resources AND Docker images
.\cleanup.ps1 -DeleteImages

# Skip confirmation prompt
.\cleanup.ps1 -Force
```

**Linux/macOS:**
```bash
# Remove all Kubernetes resources (keeps Docker images)
./cleanup.sh

# Remove resources AND Docker images
./cleanup.sh --delete-images

# Skip confirmation prompt
./cleanup.sh --force

# Combined flags
./cleanup.sh -d -f
```

### Local Development (VS Code F5 Debugging)

For running services locally without Kubernetes:

#### Setup User Secrets
**Windows:**
```powershell
.\setup.ps1
```

**Linux/macOS:**
```bash
./setup.sh
```

This configures MongoDB connection strings using .NET user secrets for local debugging.

#### Build & Debug
```bash
# Build all services
dotnet build src/services/capstone.sln

# Or use VS Code
# Press F5 → Select service to debug
```

**Services start on ports:**
- Authentication: 30001
- Catalog: 30002
- CRM: 30003
- Cart: 30004
- Order: 30005

### Seed Data

Sample menu items are stored in `devops/jobs/data/seed-menu-items.json` and include:
- **10 Menu Items** from 6 restaurants
- **Categories**: Appetizer, Main Course, Dessert, Beverage, Salad
- **Cuisines**: Italian, American, Indian, Thai, French, Mediterranean, Health Food
- Complete nutritional information, allergens, and dietary flags

### Script Parameters

#### Cleanup Script Options
- **PowerShell**: `-DeleteImages`, `-Force`
- **Bash**: `--delete-images` or `-d`, `--force` or `-f`

#### Seed Script Options
- **PowerShell**: `-GatewayUrl "http://localhost:5000"`
- **Bash**: `./seed.sh "http://localhost:5000"`

### Troubleshooting

#### Permission Denied (Linux/macOS)
```bash
chmod +x devops/jobs/bash/*.sh
```

#### PowerShell Execution Policy (Windows)
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

#### Docker Not Running
```bash
# Start Docker Desktop or your container runtime
# Windows: Start Docker Desktop
# Linux: sudo systemctl start docker
```

#### Kubernetes Not Accessible
- Enable Kubernetes in Docker Desktop settings (Settings → Kubernetes → Enable)
- Or ensure K3s/Kind cluster is running

#### Pods Stuck in Pending
```bash
# Check events
kubectl get events -n capstone-services --sort-by='.lastTimestamp'

# Check pod details
kubectl describe pod <pod-name> -n capstone-services

# Common issue: Insufficient resources
# Solution: Increase Docker Desktop memory to 8GB+ (Settings → Resources)
```

#### Services Not Responding
```bash
# Check pod logs
kubectl logs <pod-name> -n capstone-services

# Restart deployment
kubectl rollout restart deployment/<deployment-name> -n capstone-services
```

---

## 🔐 Role-Based Access Control (RBAC)

### User Roles

#### 1. Customer (external_users)
- Order food and track deliveries
- Manage profile and addresses
- Rate restaurants and provide feedback
- View order history

#### 2. Biller (Restaurant Owner)
- UPI payment processing
- Full menu management (CRUD)
- Staff management (Operators, Workers)
- Financial reporting and analytics
- Restaurant profile management

#### 3. Operator (Order Manager)
- Order confirmation and management
- Customer service coordination
- Menu item management
- Order handover to delivery agents

#### 4. Worker (Kitchen Staff)
- View assigned orders
- Update food preparation status
- View menu items
- Mark orders ready for delivery

#### 5. DeliveryAgent (fda_delivery_network)
- View assigned deliveries
- Update delivery status
- Mark orders delivered
- Provide delivery proof

#### 6. IT Department Roles (fda_it_department)
- **Developer**: Full system access
- **Tester**: QA and testing access
- **NetworkAdmin**: Monitoring and health checks
- **DatabaseAdmin**: Database management

### Authentication Flow

1. **Registration**: Phone/email with OTP verification
2. **Login**: JWT token-based authentication
3. **Authorization**: Role-based permissions via JWT claims
4. **Token Refresh**: Automatic token renewal

### JWT Claims Structure

```json
{
  "sub": "user_id",
  "role": "Customer|Biller|Operator|Worker|DeliveryAgent",
  "organization": "external_users|restaurant_name|fda_delivery_network",
  "restaurantName": "restaurant_name", // for restaurant staff
  "upiId": "biller_upi_id",          // for Billers
  "permissions": ["read:menu", "write:order", ...]
}
```

---

## 🗄️ Database Structure

### MongoDB Collections

#### Authentication Service
- **users**: User accounts with credentials
- **roles**: Role definitions and permissions
- **sessions**: Active user sessions
- **otps**: OTP verification records

#### Catalog Service
- **restaurants**: Restaurant profiles
- **menus**: Menu items and categories
- **cuisine_types**: Available cuisines

#### CRM Service
- **customers**: Customer profiles
- **restaurants**: Restaurant business data
- **addresses**: Delivery addresses

#### Cart Service
- **carts**: Shopping cart items
- **cart_items**: Individual cart entries

#### Order Service
- **orders**: Order records
- **order_items**: Order line items
- **order_history**: Order status tracking

---

## 📜 Available Scripts (devops/jobs/)

### Building & Deployment

| Script | Purpose | Usage |
|--------|---------|-------|
| `build-images-local.ps1` | Build all Docker images | `.\build-images-local.ps1` |
| `deploy-local.ps1` | Deploy backend services | `.\deploy-local.ps1` |
| `deploy-local-full-stack.ps1` | Deploy complete stack | `.\deploy-local-full-stack.ps1` |

### Cleanup

| Script | Purpose | Usage |
|--------|---------|-------|
| `cleanup-local.ps1` | Quick cleanup (keep images) | `.\cleanup-local.ps1` |
| `cleanup-complete.ps1` | Complete cleanup | `.\cleanup-complete.ps1 -DeleteImages` |

### Configuration

| Script | Purpose | Usage |
|--------|---------|-------|
| `apply-secrets.ps1` | Apply Kubernetes secrets | `.\apply-secrets.ps1` |
| `setup-local-user-secrets.ps1` | Setup .NET user secrets | `.\setup-local-user-secrets.ps1` |
| `setup-user-secrets.ps1` | General secrets setup | `.\setup-user-secrets.ps1` |

### Utilities

| Script | Purpose | Usage |
|--------|---------|-------|
| `seed-sample-data.ps1` | Seed sample menu data | `.\seed-sample-data.ps1` |
| `verify-k8s-setup.ps1` | Verify Kubernetes setup | `.\verify-k8s-setup.ps1` |
| `run-newman-tests.ps1` | Run API tests | `.\run-newman-tests.ps1` |
| `port-forward.ps1` | Port forwarding utility | `.\port-forward.ps1` |

---

## 🧪 Testing

### Automated Test Suite

**Location:** `fda/devops/jobs/powershell/test.ps1`

The centralized test script runs all Postman API collections against deployed services with comprehensive reporting.

#### Test Collections (66+ API requests)

1. **User Registration** - Authentication and account creation
2. **Restaurant Owner Workflows** - Restaurant and menu management  
3. **Operator Service Workflows** - Kitchen operations and order management
4. **Customer Workflows** - Customer profile, orders, and payments

#### Run All Tests

```powershell
# Navigate to scripts directory
cd fda/devops/jobs/powershell

# Run all API tests
.\test.ps1
```

#### Test Features
- ✅ Kubernetes service validation
- ✅ Sequential test execution
- ✅ HTML and JSON report generation
- ✅ Comprehensive test summary
- ✅ CI/CD ready with exit codes

#### Test Reports

Reports are generated in `fda/test-results/`:
- `user-registration-report.html`
- `restaurant-owner-report.html`
- `operator-service-report.html`
- `customer-workflows-report.html`
- JSON reports for automation

**Expected Duration:** 8-12 minutes for complete suite

#### Prerequisites
- Newman installed: `npm install -g newman newman-reporter-htmlextra`
- Services deployed and running in Kubernetes
- Environment file: `postman-collections/Capstone-Local-Environment.postman_environment.json`

#### Test Collections Details

**User Registration Workflows**
- Customer, Restaurant Owner, Kitchen Worker, Delivery Agent, IT Admin registration
- OTP verification and authentication
- Profile management
- Role-based access control testing

**Restaurant Owner Workflows**
- Restaurant registration with business information
- Menu management (Create, Read, Update, Delete)
- Menu item availability control
- Business hours and contact management
- Restaurant status updates

**Operator Service Workflows**
- View and accept orders
- Inventory management and low-stock alerts
- Menu item availability updates
- Order packaging with special instructions
- Secure order handover with OTP generation/verification
- End-to-end kitchen workflow

**Customer Workflows**
- User profile management (name, email, phone, preferences)
- Address management (add, update, delete delivery addresses)
- Complete order flow (browse → cart → checkout → order placement)
- Order history and real-time tracking
- Order cancellation (when allowed)

### API Endpoint Reference

#### Gateway Configuration
- **Gateway URL**: `http://localhost:30500`
- **Gateway Routes**:
  - `/api/auth` → Authentication Service
  - `/api/catalog` → Catalog Service (Menu, Restaurants, Inventory)
  - `/api/crm` → CRM Service (Customer Management)
  - `/api/cart` → Cart Service
  - `/api/order` → Order Service

#### Authentication Service Endpoints
**Base**: `/api/auth`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/register` | Register new user (all roles) |
| POST | `/login` | Login with email/phone + password |
| POST | `/login/email-otp` | Login with email + OTP |
| POST | `/login/phone-otp` | Login with phone + OTP |
| POST | `/generate-otp` | Generate OTP for verification |
| POST | `/verify-otp` | Verify OTP code |
| GET | `/validate` | Validate JWT token |
| GET | `/profile` | Get user profile |
| PUT | `/password` | Update password |

#### Catalog Service Endpoints
**Base**: `/api/catalog`

**Restaurant Management** (`/restaurant`):
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/restaurant/register` | Register new restaurant |
| GET | `/restaurant` | Get active restaurants |
| GET | `/restaurant/{id}` | Get restaurant by ID |
| GET | `/restaurant/owner/{ownerId}` | Get owner's restaurants |
| PUT | `/restaurant/{id}` | Update restaurant |
| PATCH | `/restaurant/{id}/status` | Update restaurant status |
| PATCH | `/restaurant/{id}/contact` | Update contact info |
| PATCH | `/restaurant/{id}/hours` | Update business hours |

**Menu Management** (`/menu`):
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/menu` | Get available menu items |
| GET | `/menu/{id}` | Get menu item by ID |
| GET | `/menu/restaurant/{restaurantId}` | Get restaurant menu |
| GET | `/menu/owner/{ownerId}` | Get owner's menu items |
| GET | `/menu/category/{category}` | Filter by category |
| GET | `/menu/cuisine/{cuisine}` | Filter by cuisine |
| GET | `/menu/vegetarian` | Get vegetarian items |
| GET | `/menu/vegan` | Get vegan items |
| GET | `/menu/gluten-free` | Get gluten-free items |
| GET | `/menu/search?searchTerm=pizza` | Search menu items |
| POST | `/menu` | Create menu item |
| POST | `/menu/bulk` | Create multiple items |
| PUT | `/menu/{id}` | Update menu item |
| PATCH | `/menu/{id}/availability` | Update availability |
| DELETE | `/menu/{id}` | Delete menu item |

**Operator Inventory** (`/operator/inventory`):
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/operator/inventory/{restaurantId}` | View restaurant inventory |
| GET | `/operator/inventory/{restaurantId}/low-stock` | Get low-stock alerts |
| PATCH | `/operator/inventory/{menuItemId}/availability` | Update item availability |

#### Order Service Endpoints
**Base**: `/api/order`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/order` | Create new order |
| GET | `/order/{orderId}` | Get order details |
| GET | `/order/restaurant/{restaurantId}` | Get restaurant orders |
| GET | `/order/restaurant/{restaurantId}/pending` | Get pending orders |
| GET | `/order/restaurant/{restaurantId}/ready` | Get ready orders |
| GET | `/order/customer/{customerId}` | Get customer order history |
| POST | `/order/{orderId}/accept` | Accept order (Operator) |
| POST | `/order/{orderId}/decline` | Decline order (Operator) |
| PATCH | `/order/{orderId}/status` | Update order status |
| POST | `/order/{orderId}/package` | Add packaging details |
| POST | `/order/{orderId}/generate-handover-otp` | Generate handover OTP |
| POST | `/order/handover` | Complete handover with OTP |

#### CRM Service Endpoints
**Base**: `/api/crm`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/customer` | Get all customers |
| GET | `/customer/{id}` | Get customer by ID |
| POST | `/customer` | Create customer profile |
| PUT | `/customer/{id}` | Update customer |
| DELETE | `/customer/{id}` | Delete customer |
| GET | `/customer/{id}/addresses` | Get customer addresses |
| POST | `/customer/{id}/addresses` | Add delivery address |
| PUT | `/customer/{id}/addresses/{addressId}` | Update address |
| DELETE | `/customer/{id}/addresses/{addressId}` | Delete address |

#### Cart Service Endpoints
**Base**: `/api/cart`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/cart/{userId}` | Get user's cart |
| POST | `/cart/{userId}/add` | Add item to cart |
| PUT | `/cart/{userId}/update` | Update cart item |
| DELETE | `/cart/{userId}/remove/{itemId}` | Remove cart item |
| DELETE | `/cart/{userId}/clear` | Clear entire cart |

### Environment Variables for Testing

```json
{
  "gateway_base_url": "http://localhost:30500",
  "auth_base_url": "http://localhost:30500/api/auth",
  "catalog_base_url": "http://localhost:30500/api/catalog",
  "crm_base_url": "http://localhost:30500/api/crm",
  "cart_base_url": "http://localhost:30500/api/cart",
  "order_base_url": "http://localhost:30500/api/order"
}
```

### Testing Best Practices

1. **Run Tests After Deployment**: Always run test suite after deploying changes
2. **Review HTML Reports**: Check detailed reports for any failures
3. **Sequential Execution**: Tests run in order (User Registration → Restaurant → Operator → Customer)
4. **Environment Validation**: Script validates Kubernetes services before testing
5. **Clean Test Data**: Each test suite uses isolated test data to avoid conflicts

---

## 🔧 Configuration

### Centralized Configuration Architecture

The project uses a centralized configuration approach with Kubernetes ConfigMaps and Secrets to eliminate duplication and ensure consistency across all services.

#### Configuration Structure

**1. `common-config.yaml` - Shared Non-Sensitive Configuration**

Contains 24 environment variables common across all .NET services:
- ASP.NET Core settings (environment, URLs)
- Logging configurations
- Service URLs for inter-service communication
- MongoDB connection pool settings
- CORS settings
- JWT configuration (non-secret)
- Rate limiting defaults

**Benefits:**
- ✅ Single source of truth for common variables
- ✅ Reduced configuration from ~120 lines to ~98 lines (18% reduction)
- ✅ Guaranteed consistency across all services
- ✅ Easy updates (change one file instead of six)
- ✅ No configuration drift

**2. `mongodb-secret.yaml` - Sensitive Database Credentials**

Kubernetes Secret containing MongoDB connection strings for each service (base64 encoded).

**3. Service-Specific Configuration**

Each service defines only its unique variables:
- Database name
- Collection name
- Service-specific ports or settings

#### How Services Load Configuration

Each .NET service deployment follows this pattern:

```yaml
spec:
  containers:
  - name: service-name
    # Load ALL common variables from ConfigMap
    envFrom:
    - configMapRef:
        name: common-service-config
    # Add service-specific variables
    env:
    - name: MONGO_CONNECTION_STRING
      valueFrom:
        secretKeyRef:
          name: mongodb-secret
          key: service-connection-string
    - name: DATABASE_SETTINGS__DATABASE_NAME
      value: "servicedb"
```

#### Configuration Priority

When variables are defined in multiple places:
1. **`env` variables** (service-specific) - Highest priority, overrides ConfigMap
2. **`envFrom` ConfigMap** variables - Common defaults
3. Container defaults - Lowest priority

#### Environment Variable Naming Convention

**RULE: Always use SCREAMING_SNAKE_CASE for environment variable names**

All environment variables follow SCREAMING_SNAKE_CASE naming:
- `ASPNETCORE_ENVIRONMENT`
- `MONGO_CONNECTION_STRING`
- `DATABASE_SETTINGS__DATABASE_NAME`
- `LOGGING__LOG_LEVEL__DEFAULT`

**Note:** .NET Core automatically maps these to hierarchical configuration:
- `DATABASE_SETTINGS__DATABASE_NAME` → `DatabaseSettings:DatabaseName`
- `LOGGING__LOG_LEVEL__DEFAULT` → `Logging:LogLevel:Default`

Double underscores (`__`) represent configuration hierarchy levels.

### Common Environment Variables

From `common-config.yaml`:

| Variable | Value | Description |
|----------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Development | ASP.NET Core environment |
| `ASPNETCORE_URLS` | http://+:8080 | Service listening URL |
| `LOGGING__LOG_LEVEL__DEFAULT` | Information | Default log level |
| `LOGGING__LOG_LEVEL__MICROSOFT` | Warning | Microsoft log level |
| `LOGGING__LOG_LEVEL__MICROSOFT__HOSTING__LIFETIME` | Information | Hosting log level |
| `AUTHENTICATION_SERVICE_URL` | http://authentication-service:8080 | Auth service URL |
| `CATALOG_SERVICE_URL` | http://catalog-service:8080 | Catalog service URL |
| `CRM_SERVICE_URL` | http://crm-service:8080 | CRM service URL |
| `CART_SERVICE_URL` | http://cart-service:8080 | Cart service URL |
| `ORDER_SERVICE_URL` | http://order-service:8080 | Order service URL |
| `GATEWAY_SERVICE_URL` | http://gateway-service:5000 | Gateway URL |
| `DATABASE_SETTINGS__MAX_CONNECTION_POOL_SIZE` | 100 | MongoDB max connections |
| `DATABASE_SETTINGS__MIN_CONNECTION_POOL_SIZE` | 10 | MongoDB min connections |
| `DATABASE_SETTINGS__SERVER_SELECTION_TIMEOUT` | 30000 | MongoDB timeout (ms) |
| `DATABASE_SETTINGS__CONNECT_TIMEOUT` | 30000 | MongoDB connect timeout (ms) |
| `CORS_ORIGIN` | http://localhost:3000 | Allowed CORS origin |
| `CORS_ALLOW_CREDENTIALS` | true | CORS credentials setting |
| `JWT_ISSUER` | CapstoneAuthService | JWT token issuer |
| `JWT_AUDIENCE` | CapstoneServices | JWT token audience |
| `JWT_EXPIRY_MINUTES` | 480 | JWT expiry (8 hours) |
| `RATE_LIMIT_WINDOW_MS` | 60000 | Rate limit window |
| `RATE_LIMIT_MAX_REQUESTS` | 100 | Max requests per window |
| `ENVIRONMENT` | local | Environment name |
| `TIMEZONE` | UTC | Server timezone |

### Service-Specific Variables

Each service defines in its own YAML:
- `MONGO_CONNECTION_STRING` - From mongodb-secret (sensitive)
- `DATABASE_SETTINGS__DATABASE_NAME` - Service's database name
- `DATABASE_SETTINGS__COLLECTION_NAME` - Primary collection name

### Updating Configuration

#### Update Common Settings (Affects All Services)
```powershell
# 1. Edit common-config.yaml
code fda/devops/kubernetes/local/common-config.yaml

# 2. Apply changes
kubectl apply -f fda/devops/kubernetes/local/common-config.yaml

# 3. Restart all services to pick up changes
kubectl rollout restart deployment -n capstone-services
```

#### Update Service-Specific Settings
```powershell
# 1. Edit service YAML (e.g., authentication.yaml)
code fda/devops/kubernetes/local/authentication.yaml

# 2. Apply changes (service auto-restarts)
kubectl apply -f fda/devops/kubernetes/local/authentication.yaml
```

#### Update Secrets (MongoDB Connection Strings)
```powershell
# 1. Edit mongodb-secret.yaml
code fda/devops/kubernetes/local/mongodb-secret.yaml

# 2. Apply changes
kubectl apply -f fda/devops/kubernetes/local/mongodb-secret.yaml

# 3. Restart services
kubectl rollout restart deployment -n capstone-services
```

### Verify Configuration

```powershell
# Check if ConfigMap exists
kubectl get configmap common-service-config -n capstone-services

# View ConfigMap contents
kubectl describe configmap common-service-config -n capstone-services

# View all environment variables in a running pod
kubectl exec deployment/authentication-deployment -n capstone-services -- printenv | sort

# View specific variable
kubectl exec deployment/authentication-deployment -n capstone-services -- printenv | grep ASPNETCORE_ENVIRONMENT
```

### Configuration Best Practices

1. **Common Variables**: If 3+ services use a variable, add it to `common-config.yaml`
2. **Secrets Management**: Never put sensitive data in ConfigMaps - use Secrets
3. **Naming Convention**: Always use SCREAMING_SNAKE_CASE for variable names
4. **Service-Specific**: Keep database names, collection names in service YAMLs
5. **Documentation**: Update README.md when adding new common variables (don't create separate docs)
6. **Testing**: Always restart services and verify after configuration changes

### Deployment Order

The deployment scripts apply configuration in this order:
1. Namespace
2. **common-config.yaml** (common configuration)
3. mongodb-secret.yaml (secrets)
4. mongodb-config.yaml and mongodb.yaml
5. All service deployments

This ensures common configuration exists before services reference it.

---

## 🚨 Troubleshooting

### Pods Not Starting

```powershell
# Check pod status
kubectl get pods -n capstone-services

# Describe pod for events
kubectl describe pod <pod-name> -n capstone-services

# View logs
kubectl logs <pod-name> -n capstone-services
```

### Image Pull Errors

```powershell
# Ensure images are built
.\devops\jobs\build-images-local.ps1

# Verify images exist
docker images | grep services-
```

### MongoDB Connection Issues

```powershell
# Test MongoDB connectivity
kubectl exec -it mongodb-<pod-id> -n capstone-services -- mongosh

# Check secrets
kubectl get secrets -n capstone-services
```

### Port Conflicts

```powershell
# Find process using port
netstat -ano | findstr :30001

# Kill process
taskkill /PID <process-id> /F
```

### Service Connectivity Issues

```powershell
# Test service connectivity from pod
kubectl exec -it <pod-name> -n capstone-services -- curl http://catalog-service:5002/health

# Check service DNS
kubectl exec -it <pod-name> -n capstone-services -- nslookup catalog-service.capstone-services.svc.cluster.local
```

---

## 📊 Monitoring & Health Checks

### Health Check Endpoints

All services expose health check endpoints:

```bash
# Authentication Service
curl http://localhost:30001/health

# Catalog Service
curl http://localhost:30002/health

# Gateway
curl http://localhost:30005/health

# Customer App
curl http://localhost:30080/health
```

### Kubernetes Health Probes

Services use liveness and readiness probes:

```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 5
```

### View Logs

```powershell
# Tail logs in real-time
kubectl logs -f <pod-name> -n capstone-services

# View last 100 lines
kubectl logs --tail=100 <pod-name> -n capstone-services

# View logs for all pods with label
kubectl logs -l app=catalog-api -n capstone-services --tail=50
```

---

## 🔒 Security Best Practices

### Implemented
✅ JWT-based authentication
✅ Role-based access control (RBAC)
✅ Password hashing (bcrypt)
✅ OTP verification for registration
✅ MongoDB authentication enabled
✅ User secrets for local development
✅ Kubernetes secrets for deployment
✅ CORS configuration
✅ Rate limiting (Gateway)
✅ Input validation

### Production Recommendations
⚠️ Enable HTTPS/TLS
⚠️ Implement API key management
⚠️ Use managed secrets service (Azure Key Vault, AWS Secrets Manager)
⚠️ Enable audit logging
⚠️ Implement WAF (Web Application Firewall)
⚠️ Regular security scanning
⚠️ Database encryption at rest
⚠️ Network policies (Kubernetes)
⚠️ Pod security policies

---

## 📈 Scaling & Performance

### Kubernetes Horizontal Pod Autoscaler (HPA)

For production deployments:

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: catalog-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: catalog-deployment
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

### Resource Limits

Each service has defined resource requests and limits:

```yaml
resources:
  requests:
    memory: "256Mi"
    cpu: "250m"
  limits:
    memory: "512Mi"
    cpu: "500m"
```

### Performance Tips

1. **Database Indexing**: Ensure MongoDB collections have appropriate indexes
2. **Caching**: Implement Redis for frequently accessed data
3. **Connection Pooling**: MongoDB connection pooling enabled
4. **Async Operations**: Use async/await patterns consistently
5. **API Pagination**: Implement pagination for large datasets

---

## 🔄 CI/CD Pipeline

### Recommended Workflow

```yaml
1. Code Commit → GitHub
2. Build & Test → GitHub Actions
3. Build Docker Images → Container Registry
4. Run Integration Tests → Newman/Postman
5. Deploy to Staging → Kubernetes (ArgoCD)
6. Smoke Tests → Automated
7. Deploy to Production → Manual Approval
```

### GitHub Actions Example

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Images
        run: ./devops/jobs/build-images-local.ps1
      - name: Run Tests
        run: ./devops/jobs/run-newman-tests.ps1
      - name: Deploy
        run: ./devops/jobs/deploy-local.ps1
```

---

## 📚 API Documentation

### Authentication Service

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "phoneNumber": "+1234567890",
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "phoneNumber": "+1234567890",
  "password": "SecurePass123!"
}
```

### Catalog Service

#### Get Restaurants
```http
GET /api/restaurants
Authorization: Bearer <jwt-token>
```

#### Get Menu Items
```http
GET /api/restaurants/{restaurantId}/menu
Authorization: Bearer <jwt-token>
```

#### Create Menu Item (Biller/Operator only)
```http
POST /api/restaurants/{restaurantId}/menu
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "name": "Margherita Pizza",
  "description": "Classic pizza with tomato and mozzarella",
  "price": 12.99,
  "category": "Pizza",
  "isAvailable": true
}
```

### Cart Service

#### Add to Cart
```http
POST /api/cart/items
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "menuItemId": "64a1b2c3d4e5f6",
  "quantity": 2
}
```

#### Get Cart
```http
GET /api/cart
Authorization: Bearer <jwt-token>
```

### Order Service

#### Create Order
```http
POST /api/orders
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "restaurantId": "64a1b2c3d4e5f6",
  "deliveryAddressId": "64a1b2c3d4e5f7",
  "items": [
    {
      "menuItemId": "64a1b2c3d4e5f8",
      "quantity": 2,
      "price": 12.99
    }
  ]
}
```

#### Get Order Status
```http
GET /api/orders/{orderId}
Authorization: Bearer <jwt-token>
```

---

## 🛠️ Development Workflow

### Adding a New Feature

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/new-feature
   ```

2. **Make Changes**
   - Update service code
   - Add tests
   - Update API documentation

3. **Test Locally**
   ```powershell
   # Build and test
   dotnet build
   dotnet test
   
   # Deploy to local K8s
   cd devops/jobs
   .\build-images-local.ps1
   .\deploy-local.ps1
   
   # Run API tests
   .\run-newman-tests.ps1
   ```

4. **Commit and Push**
   ```bash
   git add .
   git commit -m "feat: Add new feature"
   git push origin feature/new-feature
   ```

5. **Create Pull Request**
   - Code review
   - CI/CD pipeline runs
   - Merge to develop/main

### Database Migration

1. **Create Migration Script**
   ```javascript
   // migrations/001_add_customer_preferences.js
   db.customers.updateMany(
     {},
     { $set: { preferences: { notifications: true } } }
   );
   ```

2. **Test Migration Locally**
   ```bash
   mongosh mongodb://localhost:27017/crmdb < migrations/001_add_customer_preferences.js
   ```

3. **Apply to Production**
   - Backup database first
   - Run during maintenance window
   - Verify with test queries

---

## 📞 Support & Contributing

### Getting Help

- **Issues**: Create an issue on GitHub
- **Documentation**: Check this README
- **Logs**: Review service logs with `kubectl logs`

### Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

### Code Style

- **C#**: Follow Microsoft C# coding conventions
- **JavaScript**: Use ESLint configuration
- **Formatting**: Use Prettier for JS/TS files

---

## 📝 License

This project is part of a capstone project for educational purposes.

---

## 🎓 Project Information

- **Repository**: https://github.com/nagendra-sathyamurthy/capstone
- **Technologies**: .NET 8.0, MongoDB, Node.js, React, Kubernetes, Docker
- **Maintainer**: Nagendra Sathyamurthy
- **Last Updated**: December 1, 2025

---

## 📋 Appendix

### MongoDB Connection String Examples

**Local Development:**
```
mongodb://admin:password@localhost:27017/authenticationdb?authSource=admin
```

**Kubernetes Service:**
```
mongodb://admin:password@mongodb.capstone-services.svc.cluster.local:27017/authenticationdb?authSource=admin
```

**MongoDB Atlas (Production):**
```
mongodb+srv://username:password@cluster.mongodb.net/database?retryWrites=true&w=majority
```

### Useful Kubernetes Commands

```powershell
# Get all resources
kubectl get all -n capstone-services

# Describe deployment
kubectl describe deployment catalog-deployment -n capstone-services

# Scale deployment
kubectl scale deployment catalog-deployment --replicas=3 -n capstone-services

# Restart deployment
kubectl rollout restart deployment catalog-deployment -n capstone-services

# View deployment history
kubectl rollout history deployment catalog-deployment -n capstone-services

# Execute command in pod
kubectl exec -it <pod-name> -n capstone-services -- /bin/sh

# Copy files from pod
kubectl cp <pod-name>:/path/to/file ./local-file -n capstone-services
```

### Docker Commands

```powershell
# List images
docker images

# Remove image
docker rmi <image-id>

# View logs
docker logs <container-id>

# Clean up
docker system prune -a

# Build specific service
docker build -t services-catalog:latest -f src/services/catalog/Dockerfile src/services/
```

---

**🚀 Happy Coding!**
