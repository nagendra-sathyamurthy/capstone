# FDA Authentication Service - Role-Based Access Control (RBAC) System

## Overview
This document provides a comprehensive guide to the Role-Based Access Control (RBAC) system implemented in the FDA Authentication Service using .NET 9.0, JWT authentication, and MongoDB storage. It covers both the system design architecture and implementation details for the complete food delivery platform ecosystem.

## Organizational Structure

```
Food Delivery Platform (FDA)
├── IT Department (fda_it_department)
│   └── Technicians
│       ├── Developer - Full system development access
│       ├── Tester - Quality assurance and testing access
│       ├── NetworkAdmin - System monitoring and health checks
│       └── DatabaseAdmin - Database management access
├── Delivery Network (fda_delivery_network)
│   └── DeliveryAgent - Order pickup and delivery
└── Business Units (Restaurants)
│   ├── Biller - Restaurant owner/payment processor
│   ├── Operator - Order receiver and customer service SPOC
│   └── Worker - Kitchen staff and food preparation
└── External Users (external_users)
    └── Customer - End users who order food
```

## Role Definitions & Permissions

### 1. Customer (External Users)
**Organization**: `external_users`
**Primary Functions**: Order food, track deliveries, manage profile, provide feedback

**Permissions**:
- View restaurant catalogs and menus
- Place orders and make payments
- Track order status and delivery
- Manage personal profile and addresses
- View order history and favorites
- Rate restaurants, food items, and delivery agents
- Provide delivery feedback
- Contact customer support

**Access Level**: Public API endpoints
**Data Scope**: Own orders and profile only
**Authentication**: Email/phone + password or OTP-based login

### 2. Biller (Restaurant Owner/Payment Processor)
**Organization**: Restaurant-specific (e.g., `tonys_pizzeria`)
**Primary Functions**: Business ownership, UPI payment processing, financial oversight

**Core Features**:
- UPI ID management for payment processing
- Financial reporting and revenue analytics
- Staff management and oversight
- Restaurant business operations

**Permissions**:
- Receive and process payments
- Manage UPI settings and verification
- Full restaurant menu management (CRUD operations)
- Staff user management (create/manage Operators and Workers)
- Order management and fulfillment oversight
- View payment history and financial reports
- Generate payment reports and analytics
- Manage restaurant profile and business settings
- Inventory management
- Promotion and discount management
- Customer feedback and rating management
- Process refunds and financial adjustments

**Access Level**: Restaurant-scoped administrative access
**Data Scope**: All restaurant data, staff, orders, finances, and analytics
**JWT Claims**: `restaurantName`, `upiId`, `isUpiVerified`, `businessLicense`

### 3. Operator (Order Receiver & Single Point of Contact)
**Organization**: Restaurant-specific
**Primary Functions**: Order confirmation, customer service coordination, staff management

**Core Features**:
- Single Point of Contact (SPOC) for order management
- Customer query handling and service coordination
- Order workflow management

**Permissions**:
- Receive and confirm incoming orders
- Process order acceptance/rejection/modification
- Handle customer inquiries and complaints
- Manage order status updates
- Coordinate with kitchen staff (Workers)
- Coordinate with delivery agents
- View order queue and status
- Handle complaints and service issues
- Basic menu item availability updates
- View restaurant analytics (limited)
- Manage Worker assignments and schedules
- Process refunds and adjustments (with limits)

**Access Level**: Restaurant-scoped operational access
**Data Scope**: Restaurant orders, workers under supervision, basic analytics
**JWT Claims**: `restaurantName`, `employeeId`, `position`, `department`

### 4. Worker (Kitchen Staff)
**Organization**: Restaurant-specific
**Primary Functions**: Food preparation, packaging, labeling, and dispatch

**Core Features**:
- Complete food preparation workflow
- Order packaging and labeling
- Kitchen coordination and dispatch

**Permissions**:
- View assigned orders and preparation queue
- View order items and preparation requirements
- Prepare food according to specifications
- Pack and label orders properly
- Mark orders as ready for dispatch
- Dispatch orders to delivery agents
- Update preparation and dispatch status
- View recipe and preparation instructions
- Report ingredient shortages or issues
- Clock in/out and manage work schedules

**Access Level**: Task-specific operational access
**Data Scope**: Assigned orders and preparation tasks only
**JWT Claims**: `restaurantName`, `employeeId`, `position`, `department`

### 5. DeliveryAgent (Delivery Network)
**Organization**: `fda_delivery_network`
**Primary Functions**: Order pickup, transport, customer delivery confirmation

**Core Features**:
- Complete delivery workflow with customer interaction
- Vehicle-based delivery operations
- Real-time delivery tracking and confirmation

**Permissions**:
- Pickup orders from restaurants
- Transport orders safely to customers
- Confirm delivery with customer
- Access delivery address and contact information
- Contact customers for delivery coordination
- Accept/reject delivery assignments
- Update delivery status and location tracking
- Communicate with restaurants for pickup
- Report delivery issues and incidents
- Access navigation and route optimization tools
- View delivery history and performance metrics
- View earnings and performance analytics
- Submit feedback on restaurants and customers
- Request emergency assistance and support

**Access Level**: Delivery-scoped operational access
**Data Scope**: Assigned deliveries, delivery routes, earnings data only
**JWT Claims**: `employeeId`, `vehicleType`, `licensePlate`, `averageRating`
**Special Constraints**: 
- Cannot view customer personal data beyond delivery address
- Location tracking required during active deliveries
- Must maintain valid vehicle registration and insurance

### 6. IT Technicians (FDA IT Department)
**Organization**: `fda_it_department`

#### 6.1 Developer & Tester
**Primary Functions**: Full system access for development and testing
**Core Features**: Complete API access across all environments

**Developer Permissions**:
- Access all endpoints across all environments
- Full API access for development, testing, and production
- Database read/write for development environments
- Code repository management
- Application deployment (development/staging/production)
- Debug logs and error monitoring
- User impersonation for testing (non-production)
- View all system logs and metrics
- Manage test data across environments
- Run comprehensive system tests

**Tester Permissions**:
- Access all endpoints for testing scenarios
- Full API access for development and testing environments
- Access production API for testing validation
- Test data creation and management
- Bug reporting and tracking
- Performance testing and monitoring
- User acceptance testing coordination
- Test environment management
- View all system logs and testing metrics

**Access Level**: System-wide full access
**Data Scope**: All platform data across environments
**JWT Claims**: `employeeId`, `specialization`, `department`, `securityClearance`

#### 6.2 NetworkAdmin
**Primary Functions**: Network monitoring and health check access only
**Core Features**: Limited to system health monitoring and network operations

**Permissions**:
- Access healthcheck API endpoints
- View service status and health metrics
- Monitor network health and performance
- Server and network configuration
- Security monitoring and incident response
- Performance monitoring and optimization
- SSL certificate and domain management
- API rate limiting and security policies

**Access Level**: System monitoring and network access
**Data Scope**: System health metrics and network data only
**JWT Claims**: `employeeId`, `specialization`, `department`, `securityClearance`

#### 6.3 DatabaseAdmin
**Primary Functions**: Database management and operations only
**Core Features**: Direct database access without application APIs

**Permissions**:
- Full database access (all environments)
- Database schema management and migrations
- Performance tuning and optimization
- Data backup and recovery operations
- Database security and user management
- Data archival and cleanup operations
- Database monitoring and analytics
- Manage database users and permissions

**Access Level**: Database administrative access
**Data Scope**: All database operations and management
**JWT Claims**: `employeeId`, `specialization`, `department`, `securityClearance`

## Role Hierarchy & Reporting Structure

### Business Unit (Restaurant)
```
Biller (Restaurant Owner)
├── Manages → Operators (Order Management & Customer Service)
└── Operators coordinate with → Workers (Kitchen Staff & Food Preparation)
```

### Delivery Network
```
FDA Delivery Network Management
└── DeliveryAgents (Independent contractors/employees)
```

### IT Department
```
FDA IT Department
├── Developers (Full system development)
├── Testers (Quality assurance and validation)
├── NetworkAdmins (Infrastructure and monitoring)
└── DatabaseAdmins (Database management)
```

## Authentication & Authorization Implementation

### 1. JWT Token Structure
The JWT tokens include comprehensive role-specific claims:

```json
{
  "sub": "user_id",
  "email": "user@example.com", 
  "role": "customer|biller|operator|worker|deliveryagent|developer|tester|networkadmin|databaseadmin",
  "organization": "restaurant_id|fda_it_department|fda_delivery_network|external_users",
  "permission": ["perm1", "perm2", "..."],
  "iss": "FDA-Authentication-Service",
  "exp": "expiration_timestamp"
}
```

**Enhanced Role-Specific Claims**:

**Biller Claims:**
- `restaurantName`: Restaurant business name
- `upiId`: UPI ID for payment processing
- `isUpiVerified`: UPI verification status
- `businessLicense`: Business license information

**Operator/Worker Claims:**
- `restaurantName`: Restaurant business name
- `employeeId`: Employee identification
- `position`: Job position/title
- `department`: Department within restaurant

**DeliveryAgent Claims:**
- `employeeId`: Employee identification
- `vehicleType`: Vehicle used for delivery (bike, car, etc.)
- `licensePlate`: Vehicle license plate number
- `averageRating`: Delivery performance rating

**IT Staff Claims:**
- `employeeId`: Employee identification
- `specialization`: Technical specialization area
- `department`: IT department (development, testing, etc.)
- `securityClearance`: Security clearance level

**Customer Claims:**
- `dietaryPreferences`: Dietary restrictions/preferences (optional)

### 2. Multiple Authentication Methods

#### Mandatory Phone Number Registration
- **Phone number is required** for all user registrations
- Unique phone number validation prevents duplicates
- Phone number serves as alternative login identifier

#### Authentication Options
1. **Email + Password**: Traditional email-based login
2. **Phone + Password**: Phone number-based login  
3. **Email + OTP**: Email with one-time password
4. **Phone + OTP**: SMS-based OTP authentication

#### OTP System Features
- **6-digit OTP codes** with 5-minute expiry
- **Multiple purposes**: Login, password reset, phone verification, email verification
- **Security controls**: Maximum 3 attempts, automatic invalidation of old OTPs
- **MongoDB storage** with proper indexing and cleanup

## API Endpoints

### Authentication Endpoints

#### Registration & Core Authentication
- `POST /api/auth/register/{role}` - Role-based user registration (phone number required)
- `GET /api/auth/validate` - JWT token validation
- `GET /api/auth/profile` - Get current user profile
- `PUT /api/auth/profile` - Update user profile information

#### Multiple Login Methods
- `POST /api/auth/login` - General login (supports all authentication methods)
- `POST /api/auth/login/email` - Email + password login
- `POST /api/auth/login/phone` - Phone + password login
- `POST /api/auth/login/email-otp` - Email + OTP passwordless login
- `POST /api/auth/login/phone-otp` - Phone + OTP passwordless login

#### OTP Management
- `POST /api/auth/generate-otp` - Generate OTP for login/verification
- `POST /api/auth/verify-otp` - Verify OTP for various purposes

### Authorization Test Endpoints
- `GET /api/test/customer-only` - Customer role validation
- `GET /api/test/restaurant-staff` - Restaurant staff roles (Biller, Operator, Worker)
- `GET /api/test/it-staff` - IT technician roles (Developer, Tester, NetworkAdmin, DatabaseAdmin)
- `GET /api/test/management` - Management level access (Biller, Operator)
- `GET /api/test/payment-processing` - Payment processing access (Biller only)
- `GET /api/test/order-confirmation` - Order confirmation access (Operator only)
- `GET /api/test/food-preparation` - Food preparation access (Worker only)
- `GET /api/test/delivery-confirmation` - Delivery confirmation access (DeliveryAgent only)
- `GET /api/test/full-system-access` - Full system access (Developer/Tester only)
- `GET /api/test/healthcheck-access` - Health check access (NetworkAdmin only)
- `GET /api/test/database-access` - Database access (DatabaseAdmin only)
- `GET /api/test/delivery-agent` - Delivery agent specific features
- `GET /api/test/my-permissions` - View current user permissions
- `GET /api/test/check-permission/{permission}` - Check specific permission

## Authorization Policies

### Role-Based Policies
- `CustomerOnly`: Customer role exclusive access
- `BillerOnly`: Biller role exclusive access
- `OperatorOnly`: Operator role exclusive access
- `WorkerOnly`: Worker role exclusive access
- `DeliveryAgentOnly`: DeliveryAgent role exclusive access
- `DeveloperOnly`: Developer role exclusive access
- `TesterOnly`: Tester role exclusive access
- `NetworkAdminOnly`: NetworkAdmin role exclusive access
- `DatabaseAdminOnly`: DatabaseAdmin role exclusive access

### Combined Role Policies
- `RestaurantStaff`: Biller, Operator, or Worker roles
- `ITStaff`: Developer, Tester, NetworkAdmin, or DatabaseAdmin roles
- `ManagementLevel`: Biller or Operator roles (supervisory access)

### Permission-Based Policies
- `CanReceivePayments`: Payment processing permission (Biller with UPI setup)
- `CanConfirmOrders`: Order confirmation permission (Operator)
- `CanPrepareFood`: Food preparation permission (Worker)
- `CanConfirmDelivery`: Delivery confirmation permission (DeliveryAgent with vehicle)
- `CanAccessAllEndpoints`: Full system access permission (Developer/Tester)
- `CanAccessHealthcheck`: Health check API permission (NetworkAdmin)
- `CanAccessDatabase`: Database access permission (DatabaseAdmin)

## Data Access Patterns & Security

### Customer Data Access
- **Own Profile**: Full CRUD access to personal information
- **Orders**: Access to own order history and current orders only
- **Restaurants**: Public catalog and menu data access only
- **Ratings**: Can view and submit ratings for delivered orders

### Restaurant Staff Data Access
- **Biller**: All restaurant data + staff management + financial operations
- **Operator**: Restaurant orders + assigned workers + customer service
- **Worker**: Assigned tasks + preparation queue + order fulfillment

### Delivery Network Data Access
- **DeliveryAgent**: Assigned deliveries + route optimization + earnings data
- **Restricted Access**: Cannot view customer personal data beyond delivery requirements

### IT Department Data Access
- **Developer/Tester**: All data with environment-specific restrictions
- **NetworkAdmin**: System metrics + security logs + health monitoring
- **DatabaseAdmin**: Full database access + performance metrics + data management

## Enhanced Validation & Security Features

### Role-Specific Registration Validation
- **Biller**: Must provide BusinessInfo with RestaurantName and UPI ID
- **Operator/Worker**: Must provide EmployeeInfo with EmployeeId and Position  
- **DeliveryAgent**: Must provide DeliveryInfo with EmployeeId, VehicleType, and LicensePlate
- **IT Roles**: Must provide TechInfo with EmployeeId and Specialization
- **Customer**: Minimal requirements with phone number validation

### Enhanced Validation Methods
The AuthService includes comprehensive validation:

1. **Role-specific validation functions:**
   - `CanReceivePayments()`: Validates Biller with UPI setup
   - `CanConfirmOrders()`: Validates Operator role permissions
   - `CanPrepareFood()`: Validates Worker role permissions
   - `CanConfirmDelivery()`: Validates DeliveryAgent with vehicle registration
   - `CanAccessAllEndpoints()`: Validates Developer/Tester full access
   - `CanAccessHealthcheck()`: Validates NetworkAdmin monitoring access
   - `CanAccessDatabase()`: Validates DatabaseAdmin database access

2. **Group validation functions:**
   - `IsRestaurantStaff()`: Checks for restaurant-based roles
   - `IsITStaff()`: Checks for IT department roles
   - `IsManagementLevel()`: Checks for supervisory roles

3. **Security validation:**
   - Enforces required role-specific information during registration
   - Validates organizational membership and hierarchy
   - Prevents unauthorized role transitions
   - Ensures data scope compliance

## Security Implementation

### 1. JWT Bearer Authentication
- Secure token-based authentication with comprehensive claims
- Role-based authorization with granular permissions
- Organizational hierarchy enforcement
- Token validation with expiry and refresh mechanisms

### 2. Password Security
- Secure password hashing (implementation ready for bcrypt/Argon2)
- Password complexity requirements (configurable)
- Account lockout protection after failed attempts

### 3. Multi-Factor Authentication (MFA)
- **Required for**: All IT personnel for security access
- **Recommended for**: Restaurant owners (Billers) for financial operations
- **Available for**: All users via SMS/email verification

### 4. Audit & Logging
- All role-based operations logged with user context
- Sensitive data access tracking and monitoring
- Failed authentication attempt detection and alerting
- Regular security compliance reports and reviews

## Configuration

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key",
    "Issuer": "FDA-Authentication-Service",
    "ExpiryMinutes": 60
  }
}
```

### MongoDB Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "mongodb://username:password@localhost:27017/authenticationdb?authSource=admin"
  },
  "DatabaseSettings": {
    "DatabaseName": "authenticationdb",
    "CollectionName": "users"
  }
}
```

## Implementation Status

### ✅ Successfully Implemented
- **JWT Integration**: JWT Bearer authentication with role-based claims
- **MongoDB Integration**: User data storage with role-specific schemas
- **Multi-Method Authentication**: Email, phone, password, and OTP options
- **Role-Based Authorization**: Comprehensive RBAC with permissions matrix
- **API Documentation**: Swagger documentation with JWT support
- **Docker Containerization**: Ready for deployment
- **Phone-Based Authentication**: OTP system with SMS integration
- **Organizational Hierarchy**: Multi-tenant restaurant and department support

### 🎯 Production Considerations
- **Strong JWT Secrets**: Use 256-bit cryptographically secure keys
- **Production Password Hashing**: Implement bcrypt or Argon2
- **HTTPS Enforcement**: SSL/TLS certificates for secure communication
- **CORS Configuration**: Proper cross-origin request policies
- **Rate Limiting**: API throttling and abuse prevention
- **Input Validation**: Comprehensive data sanitization
- **Monitoring & Logging**: Production-level observability
- **Token Refresh**: Automatic token renewal mechanisms
- **Database Security**: TLS-encrypted MongoDB connections
- **Security Headers**: OWASP security headers implementation

## Test Scenarios by Role

### Customer Testing Flow
Registration → Phone/Email Verification → Menu Browsing → Order Placement → Payment → Order Tracking → Delivery Confirmation → Rating & Feedback

### Biller Testing Flow  
Registration with UPI → Staff Management → Menu Management → Order Oversight → Payment Processing → Financial Reporting → Customer Service Management

### Operator Testing Flow
Employee Registration → Order Processing → Customer Service → Worker Coordination → Order Status Management → Shift Management → Issue Resolution

### Worker Testing Flow
Employee Registration → Task Assignment → Order Preparation → Status Updates → Kitchen Coordination → Order Completion → Schedule Management

### DeliveryAgent Testing Flow
Registration with Vehicle Info → Delivery Assignment → Order Pickup → Customer Navigation → Delivery Confirmation → Earnings Tracking → Performance Metrics

### IT Technician Testing Flow
Department Registration → System Access Validation → Role-Specific Operations → Monitoring & Maintenance → Security Compliance → System Administration

This comprehensive RBAC system provides enterprise-grade security, scalability, and operational efficiency for the complete FDA food delivery platform ecosystem, ensuring proper role separation while enabling seamless workflow coordination across all organizational levels.