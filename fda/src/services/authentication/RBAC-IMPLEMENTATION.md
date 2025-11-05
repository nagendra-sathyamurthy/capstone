# FDA Authentication Service - RBAC Implementation

## Overview
This document summarizes the comprehensive Role-Based Access Control (RBAC) system implemented in the FDA Authentication Service using .NET 9.0, JWT authentication, and MongoDB storage.

## Implemented User Roles

### 1. Customer (External Users)
- **Organization**: `external_users`
- **Role**: End users who order food and provide feedback
- **Permissions**:
  - View menu
  - Place orders
  - Track orders
  - Manage profile
  - View order history
  - Provide delivery feedback
  - Rate delivered items

### 2. Biller (Payment Recipient)
- **Organization**: Restaurant-specific (e.g., `tonys_pizzeria`)
- **Role**: Business owner who receives payments through UPI/bank
- **Key Features**: UPI ID management, payment processing, financial oversight
- **Permissions**:
  - Receive payments
  - Manage UPI settings
  - View payment history
  - Generate payment reports
  - Manage restaurant profile
  - View financial summary
  - Process refunds

### 3. Operator (Order Receiver & SPOC)
- **Organization**: Restaurant-specific
- **Role**: Single Point of Contact for order management and customer queries
- **Key Features**: Order confirmation, customer service coordination
- **Permissions**:
  - Receive orders
  - Confirm orders
  - Handle customer queries
  - Manage order queue
  - Coordinate with kitchen
  - Coordinate with delivery
  - View order status
  - Handle complaints

### 4. Worker (Kitchen Staff)
- **Organization**: Restaurant-specific
- **Role**: Food preparation, packaging, labeling, and dispatch
- **Key Features**: Complete food preparation workflow from order to dispatch
- **Permissions**:
  - View order items
  - Prepare food
  - Pack orders
  - Label packages
  - Mark order ready
  - Dispatch to delivery
  - Update preparation status

### 5. DeliveryAgent
- **Organization**: `fda_delivery_network`
- **Role**: Pickup, transport, and confirm delivery of orders
- **Key Features**: Complete delivery workflow with customer interaction
- **Permissions**:
  - Pickup orders
  - Transport orders
  - Confirm delivery
  - Access delivery address
  - Contact customer
  - Update delivery status
  - Report delivery issues
  - View delivery history

### 6. IT Technicians (FDA IT Department)
- **Organization**: `fda_it_department`

#### 6.1 Developer & Tester
- **Role**: Full system access for development and testing
- **Key Features**: Complete API access across all environments
- **Permissions**:
  - Access all endpoints
  - Access development API
  - Access testing API
  - Access production API
  - View all logs
  - Manage test data
  - Run system tests
  - Deploy applications (Developer only)

#### 6.2 NetworkAdmin
- **Role**: Network monitoring and healthcheck access only
- **Key Features**: Limited to system health monitoring
- **Permissions**:
  - Access healthcheck API
  - View service status
  - Monitor network health

#### 6.3 DatabaseAdmin
- **Role**: Database management and operations only
- **Key Features**: Direct database access without application APIs
- **Permissions**:
  - Access database
  - Manage database
  - Perform data backup
  - Optimize database
  - View database logs

## Key Components Implemented

### 1. Data Models
- **UserAccount.cs**: Comprehensive user model with role-specific information
- **AuthDtos.cs**: Request/response DTOs for authentication operations
- **Permissions.cs**: Static permission matrix and organizational structure
- **Address.cs**: Address information for users

### 2. Services
- **AuthService.cs**: Business logic for authentication, JWT generation, and permission checking
- **AuthController.cs**: REST API endpoints for authentication operations

### 3. Data Access
- **IRepository.cs**: Generic repository interface with filtering support
- **MongoRepository.cs**: MongoDB implementation with LINQ support

### 4. Authorization Middleware
- **AuthorizationMiddleware.cs**: Custom authorization policies and handlers
- Role-based policies (CustomerOnly, RestaurantStaff, ITStaff, etc.)
- Permission-based policies (CanManageMenu, CanViewFinancialReports, etc.)

### 5. Test Controller
- **TestController.cs**: Sample endpoints demonstrating RBAC functionality

## API Endpoints

### Authentication Endpoints

#### Registration & Core Authentication
- `POST /api/auth/register/{role}` - Role-based user registration (phone number required)
- `GET /api/auth/validate` - Token validation
- `GET /api/auth/profile` - Get user profile
- `PUT /api/auth/profile` - Update user profile

#### Multiple Login Methods
- `POST /api/auth/login` - General login (supports all methods)
- `POST /api/auth/login/email` - Email + password login
- `POST /api/auth/login/phone` - Phone + password login
- `POST /api/auth/login/email-otp` - Email + OTP login
- `POST /api/auth/login/phone-otp` - Phone + OTP login

#### OTP Management
- `POST /api/auth/generate-otp` - Generate OTP for login/verification
- `POST /api/auth/verify-otp` - Verify OTP for various purposes

### Authorization Test Endpoints
- `GET /api/test/customer-only` - Customer role only
- `GET /api/test/restaurant-staff` - Restaurant staff roles
- `GET /api/test/it-staff` - IT technician roles
- `GET /api/test/management` - Management level access
- `GET /api/test/payment-processing` - Payment processing (Biller only)
- `GET /api/test/order-confirmation` - Order confirmation (Operator only)
- `GET /api/test/food-preparation` - Food preparation (Worker only)
- `GET /api/test/delivery-confirmation` - Delivery confirmation (DeliveryAgent only)
- `GET /api/test/full-system-access` - Full system access (Developer/Tester only)
- `GET /api/test/healthcheck-access` - Healthcheck access (NetworkAdmin only)
- `GET /api/test/database-access` - Database access (DatabaseAdmin only)
- `GET /api/test/delivery-agent` - Delivery agent role features
- `GET /api/test/my-permissions` - View current user permissions
- `GET /api/test/check-permission/{permission}` - Check specific permission

## JWT Token Structure

The JWT tokens include the following claims:
- `email`: User's email address
- `role`: User's assigned role
- `organization`: User's organization
- `permission`: Multiple claims for each user permission
- `restaurant_name`: Restaurant name (for restaurant staff)
- `employee_id`: Employee ID (for staff members)
- `vehicle_type`: Vehicle type (for delivery agents)
- `department`: Department (for IT staff)

## Authorization Policies

### Role-Based Policies
- `CustomerOnly`: Customer role only
- `BillerOnly`: Biller role only
- `OperatorOnly`: Operator role only
- `WorkerOnly`: Worker role only
- `DeliveryAgentOnly`: DeliveryAgent role only
- `DeveloperOnly`: Developer role only
- `TesterOnly`: Tester role only
- `NetworkAdminOnly`: NetworkAdmin role only
- `DatabaseAdminOnly`: DatabaseAdmin role only

### Combined Role Policies
- `RestaurantStaff`: Biller, Operator, or Worker
- `ITStaff`: Developer, Tester, NetworkAdmin, or DatabaseAdmin
- `ManagementLevel`: Biller or Operator

### Permission-Based Policies
- `CanReceivePayments`: Payment processing permission (Biller)
- `CanConfirmOrders`: Order confirmation permission (Operator)
- `CanPrepareFood`: Food preparation permission (Worker)
- `CanConfirmDelivery`: Delivery confirmation permission (DeliveryAgent)
- `CanAccessAllEndpoints`: Full system access permission (Developer/Tester)
- `CanAccessHealthcheck`: Healthcheck API permission (NetworkAdmin)
- `CanAccessDatabase`: Database access permission (DatabaseAdmin)

## Security Features

1. **JWT Bearer Authentication**: Secure token-based authentication
2. **Role-Based Authorization**: Granular role-based access control
3. **Permission Matrix**: Fine-grained permission system
4. **Organizational Hierarchy**: Multi-tenant organization support
5. **Password Hashing**: Secure password storage (basic implementation)
6. **Token Validation**: Comprehensive JWT token validation
7. **CORS Configuration**: Cross-origin request handling

## Configuration

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "FDA-Authentication-Service"
  }
}
```

### MongoDB Connection
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://username:password@localhost:27017"
  },
  "DatabaseName": "AuthenticationDb"
}
```

## Build Status
✅ **Successfully Built**: All components compile without errors
✅ **JWT Integration**: JWT Bearer authentication properly configured
✅ **MongoDB Integration**: Database connectivity established
✅ **Swagger Documentation**: API documentation with JWT support
✅ **Docker Ready**: Containerization support available

## Next Steps
1. **Testing**: Run comprehensive tests for all user roles and permissions
2. **Environment Setup**: Configure development and production environments
3. **Integration**: Connect with other microservices in the FDA ecosystem
4. **Security Hardening**: Implement production-level security measures
5. **Monitoring**: Add logging and monitoring capabilities

## Security Considerations for Production
- Use strong JWT secret keys (256-bit minimum)
- Implement proper password hashing with salt (bcrypt/Argon2)
- Enable HTTPS in production
- Configure proper CORS policies
- Implement rate limiting
- Add input validation and sanitization
- Set up proper logging and monitoring
- Implement token refresh mechanism
- Add password complexity requirements
- Configure secure MongoDB connection with TLS

## Refined RBAC Implementation Features

### Enhanced JWT Token Claims
The refined RBAC system includes detailed role-specific claims in JWT tokens:

**Biller Claims:**
- `restaurantName`: Restaurant business name
- `upiId`: UPI ID for payment processing
- `isUpiVerified`: UPI verification status
- `businessLicense`: Business license information

**Operator/Worker Claims:**
- `employeeId`: Employee identification
- `position`: Job position/title
- `department`: Department within restaurant

**DeliveryAgent Claims:**
- `employeeId`: Employee identification
- `vehicleType`: Vehicle used for delivery
- `licensePlate`: Vehicle license plate
- `averageRating`: Delivery performance rating

**IT Staff Claims:**
- `employeeId`: Employee identification
- `specialization`: Technical specialization
- `department`: IT department
- `securityClearance`: Security clearance level

**Customer Claims:**
- `dietaryPreferences`: Dietary restrictions/preferences

### Enhanced Validation Methods
The AuthService now includes comprehensive validation methods:

1. **Role-specific validation functions:**
   - `CanReceivePayments()`: Validates Biller with UPI setup
   - `CanConfirmOrders()`: Validates Operator role
   - `CanPrepareFood()`: Validates Worker role
   - `CanConfirmDelivery()`: Validates DeliveryAgent with vehicle
   - `CanAccessAllEndpoints()`: Validates Developer/Tester roles
   - `CanAccessHealthcheck()`: Validates NetworkAdmin role
   - `CanAccessDatabase()`: Validates DatabaseAdmin role

2. **Group validation functions:**
   - `IsRestaurantStaff()`: Checks for restaurant roles
   - `IsITStaff()`: Checks for IT department roles
   - `IsManagementLevel()`: Checks for management roles

3. **Registration validation:**
   - Enforces required role-specific information
   - Validates UPI ID for Billers
   - Validates employee IDs for staff roles
   - Validates vehicle information for DeliveryAgents
   - Validates technical credentials for IT roles

### Role-Specific Requirements
The system now enforces strict requirements during registration:

- **Biller**: Must provide BusinessInfo with RestaurantName and UPI ID
- **Operator/Worker**: Must provide EmployeeInfo with EmployeeId and Position
- **DeliveryAgent**: Must provide DeliveryInfo with EmployeeId, VehicleType, and LicensePlate
- **IT Roles**: Must provide TechInfo with EmployeeId and Specialization
- **Customer**: No additional requirements (minimal registration)

### Security Enhancements
1. **Comprehensive claim structure** for detailed authorization
2. **Role-specific validation** prevents invalid registrations
3. **Organizational hierarchy** maintains separation of concerns
4. **Permission matrix** enables fine-grained access control
5. **Registration validation** ensures data integrity

### Phone-Based Authentication & OTP System

#### Mandatory Phone Number
- **Phone number is now required** for all user registrations
- Unique phone number validation prevents duplicates
- Phone number serves as alternative login identifier

#### Multiple Authentication Methods
1. **Email + Password**: Traditional email-based login
2. **Phone + Password**: Phone number-based login  
3. **Email + OTP**: Email with one-time password
4. **Phone + OTP**: SMS-based OTP authentication

#### OTP Features
- **6-digit OTP codes** with 5-minute expiry
- **Multiple purposes**: Login, password reset, phone verification, email verification
- **Security controls**: Maximum 3 attempts, automatic invalidation of old OTPs
- **MongoDB storage** with proper indexing and cleanup

#### Enhanced Login Flow
```
Registration → Phone Required + Email
Login Options → 
  ├── Email/Phone + Password (Traditional)
  └── Email/Phone + OTP (Passwordless)

OTP Flow →
  Generate OTP → Verify within 5min → Login Success
```

#### API Endpoint Examples
```
POST /api/auth/generate-otp
{
  "phoneNumber": "+1234567890", 
  "purpose": "Login"
}

POST /api/auth/login/phone-otp  
{
  "phoneNumber": "+1234567890",
  "otp": "123456"
}
```

This refined RBAC implementation provides a robust, secure, and scalable foundation for the FDA food delivery application with enterprise-level authentication and authorization capabilities, flexible multi-method authentication, and comprehensive phone-based verification perfectly aligned with modern security requirements and business workflow needs.