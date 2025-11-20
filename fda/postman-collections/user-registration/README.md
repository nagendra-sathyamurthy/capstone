# User Registration Flow

This collection provides a complete user registration and authentication workflow for the Food Delivery Application. It supports registration, login, and token validation for all user roles in the system.

## Overview

The User Registration Flow collection enables bulk user registration across different roles using data-driven testing. It automates the process of:
1. Registering new user accounts
2. Logging in with created credentials
3. Validating authentication tokens

## Collection Structure

### Requests

#### 1. **Register User**
- **Method**: POST
- **Endpoint**: `{{auth_base_url}}/api/auth/register`
- **Purpose**: Creates a new user account in the authentication service
- **Input**: User data from data files (email, password, role, organization)
- **Output**: User ID (stored in collection variable)
- **Success Codes**: 200 (Created), 201 (Created), 409 (Already Exists)

#### 2. **Login User**
- **Method**: POST
- **Endpoint**: `{{auth_base_url}}/api/auth/login`
- **Purpose**: Authenticates the user and retrieves JWT token
- **Input**: Email and password from variables
- **Output**: JWT authentication token (stored in `auth_token` variable)
- **Success Code**: 200 OK

#### 3. **Validate Token**
- **Method**: GET
- **Endpoint**: `{{auth_base_url}}/api/auth/validate`
- **Purpose**: Verifies the JWT token is valid and retrieves user information
- **Authentication**: Bearer token (from `auth_token` variable)
- **Output**: User profile data (email, role, permissions)
- **Success Code**: 200 OK

## Working Principle

### Data Flow

```
Data File → Register User → Store Email & User ID
              ↓
         Login User → Store JWT Token
              ↓
         Validate Token → Verify Authentication
```

### Variable Management

The collection uses collection-level variables to maintain state across requests:

| Variable      | Source                    | Purpose                              |
|---------------|---------------------------|--------------------------------------|
| `user_email`  | Request body (Register)   | User's email for login               |
| `user_id`     | Response (Register/Login) | User's unique identifier             |
| `auth_token`  | Response (Login)          | JWT token for authenticated requests |

### Test Scripts

Each request includes test scripts that:
- Validate response status codes
- Extract data from responses
- Store values in collection variables
- Chain requests together automatically

### Data-Driven Testing

The collection uses external JSON data files for bulk operations:
- Each data file contains multiple user records
- Newman runs the collection once per data record
- Variables are populated from the data file for each iteration

## Usage

### Prerequisites

1. Services must be running:
   - Authentication Service (default: `http://localhost:30001`)
   - MongoDB database

2. Environment file configured:
   - `Capstone-Local-Environment.postman_environment.json`
   - Variables: `auth_base_url`, `crm_base_url`

### Running with Newman

#### Single User Registration
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -e ../Capstone-Local-Environment.postman_environment.json \
  --env-var "user_data={\"email\":\"test@example.com\",\"password\":\"Test123!\",\"role\":0,\"organization\":\"external_users\"}" \
  --env-var "user_password=Test123!"
```

#### Bulk User Registration (Recommended)

**Register Customers (5 accounts)**
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -d data/customer-registration.json \
  -e ../Capstone-Local-Environment.postman_environment.json
```

**Register Restaurant Owners (8 accounts)**
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -d data/restaurant-owner-registration.json \
  -e ../Capstone-Local-Environment.postman_environment.json
```

**Register Kitchen Workers (10 accounts)**
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -d data/kitchen-worker-registration.json \
  -e ../Capstone-Local-Environment.postman_environment.json
```

**Register Delivery Agents (10 accounts)**
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -d data/delivery-agent-registration.json \
  -e ../Capstone-Local-Environment.postman_environment.json
```

**Register IT Admins (10 accounts)**
```powershell
newman run User-Registration-Flow.postman_collection.json \
  -d data/it-admin-registration.json \
  -e ../Capstone-Local-Environment.postman_environment.json
```

#### Register All Users at Once
```powershell
# From the postman-collections directory
$dataFiles = @(
    "customer-registration.json",
    "restaurant-owner-registration.json",
    "kitchen-worker-registration.json",
    "delivery-agent-registration.json",
    "it-admin-registration.json"
)

foreach ($file in $dataFiles) {
    Write-Host "Registering users from $file..." -ForegroundColor Cyan
    newman run user-registration/User-Registration-Flow.postman_collection.json `
        -d "user-registration/data/$file" `
        -e Capstone-Local-Environment.postman_environment.json
}
```

### Running in Postman GUI

1. Import the collection: `User-Registration-Flow.postman_collection.json`
2. Select the environment: `Capstone-Local-Environment`
3. **For single user**:
   - Set collection variables manually:
     - `user_data`: JSON string with user details
     - `user_password`: Password for login
   - Run the collection

4. **For bulk users**:
   - Use Collection Runner
   - Select data file from `data/` folder
   - Preview data to verify
   - Run collection

## Data File Format

Each data file contains an array of user records:

```json
[
  {
    "user_data": "{\"email\":\"user@example.com\",\"password\":\"Password123!\",\"role\":0,\"organization\":\"org_name\"}",
    "user_password": "Password123!",
    "user_role": "RoleName",
    "description": "Description of this user account"
  }
]
```

### Fields Explained

| Field          | Type   | Description                                      |
|----------------|--------|--------------------------------------------------|
| `user_data`    | String | JSON string with registration data (escaped)     |
| `user_password`| String | Plain password for login step                    |
| `user_role`    | String | Human-readable role name (for documentation)     |
| `description`  | String | Account description/purpose (for documentation)  |

### user_data JSON Structure

```json
{
  "email": "user@example.com",      // User's email (unique identifier)
  "password": "Password123!",        // User's password (min 6 chars)
  "role": 0,                         // Role ID (integer)
  "organization": "org_name"         // Organization/company name
}
```

## Role Mappings

| Role             | ID | Organization Examples              |
|------------------|----|------------------------------------|
| Customer         | 0  | external_users                     |
| Biller (Owner)   | 1  | Pizza Palace, Sushi Spot          |
| Operator         | 2  | Operations Team                    |
| Worker (Kitchen) | 3  | Pizza Palace, Italian Bistro      |
| DeliveryAgent    | 4  | Fast Delivery Co                   |
| Developer        | 5  | FoodDelivery IT                    |
| Tester           | 6  | FoodDelivery IT                    |
| NetworkAdmin     | 7  | FoodDelivery IT                    |
| DatabaseAdmin    | 8  | FoodDelivery IT                    |

## Expected Results

### Successful Flow
```
✓ Register User: 201 Created (or 409 if exists)
✓ Login User: 200 OK
✓ Validate Token: 200 OK
```

### Output Example
```
newman

User Registration Flow

→ Register User
  POST http://localhost:30001/api/auth/register [201 Created, 234B, 45ms]
  ✓ Registration successful

→ Login User
  POST http://localhost:30001/api/auth/login [200 OK, 1.2kB, 23ms]
  ✓ Login successful
  ✓ Token received

→ Validate Token
  GET http://localhost:30001/api/auth/validate [200 OK, 567B, 12ms]
  ✓ Token is valid
  ✓ User info returned

┌─────────────────────────┬──────────┬──────────┐
│                         │ executed │   failed │
├─────────────────────────┼──────────┼──────────┤
│              iterations │        1 │        0 │
├─────────────────────────┼──────────┼──────────┤
│                requests │        3 │        0 │
├─────────────────────────┼──────────┼──────────┤
│            test-scripts │        3 │        0 │
├─────────────────────────┼──────────┼──────────┤
│              assertions │        5 │        0 │
└─────────────────────────┴──────────┴──────────┘
```

## Troubleshooting

### Issue: Registration returns 409 Conflict
**Cause**: User with that email already exists  
**Solution**: This is expected behavior. The test passes with 409. Users can still login.

### Issue: Login returns 401 Unauthorized
**Cause**: Incorrect password or user doesn't exist  
**Solution**: 
- Verify password in data file matches registration
- Check if registration succeeded (200/201/409)
- Verify authentication service is running

### Issue: Validate Token returns 401 Unauthorized
**Cause**: JWT token is invalid or expired  
**Solution**: 
- Check if login succeeded
- Verify `auth_token` variable is set
- JWT tokens expire after 1 hour by default

### Issue: Cannot connect to service
**Cause**: Authentication service not running  
**Solution**:
```powershell
# Check if service is running
docker ps | Select-String "authentication"

# Start services if needed
docker-compose -f docker-compose-local.yml up -d
```

## Integration with Other Services

After successful registration and authentication, the JWT token can be used with:

- **CRM Service** - Create customer profiles, user profiles
- **Catalog Service** - Browse menu items, create items (owners)
- **Cart Service** - Manage shopping cart
- **Order Service** - Place and track orders
- **Payment Service** - Process payments

## Security Notes

⚠️ **Important Security Considerations**:

1. **Test Data Only**: Data files contain plain-text passwords for testing only
2. **Production**: Never store passwords in plain text in production
3. **Token Storage**: JWT tokens in variables are session-based
4. **Password Policy**: Enforce strong passwords (min 8 chars, uppercase, lowercase, number, special char)
5. **Environment Variables**: Use environment variables for sensitive configuration

## Extending the Collection

### Adding New User Types

1. Create a new data file in `data/` folder
2. Follow the existing format
3. Set appropriate role ID and organization
4. Run with Newman using the new data file

### Customizing Test Assertions

Edit the test scripts in each request:
```javascript
pm.test('Custom test name', function () {
    // Your test logic
});
```

### Adding Post-Registration Steps

Add new requests after "Validate Token":
- Create user profile in CRM
- Assign permissions
- Send welcome email
- Set initial preferences

## API Reference

### Authentication Service Endpoints

| Endpoint         | Method | Auth Required | Purpose                  |
|------------------|--------|---------------|--------------------------|
| /api/auth/register | POST   | No            | Register new user        |
| /api/auth/login    | POST   | No            | Authenticate user        |
| /api/auth/validate | GET    | Yes (Bearer)  | Validate JWT token       |

### Request/Response Examples

**Register Request**:
```json
{
  "email": "customer@example.com",
  "password": "Customer123!",
  "role": 0,
  "organization": "external_users"
}
```

**Register Response (201)**:
```json
{
  "id": "691eb9a2dc562628fb027cda",
  "email": "customer@example.com",
  "role": 0
}
```

**Login Request**:
```json
{
  "email": "customer@example.com",
  "password": "Customer123!"
}
```

**Login Response (200)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "id": "691eb9a2dc562628fb027cda",
  "email": "customer@example.com",
  "role": 0
}
```

**Validate Response (200)**:
```json
{
  "id": "691eb9a2dc562628fb027cda",
  "email": "customer@example.com",
  "role": 0,
  "organization": "external_users",
  "permissions": []
}
```

## Performance Considerations

- **Batch Size**: Register users in batches of 10-50 for optimal performance
- **Rate Limiting**: Add delays between requests if hitting rate limits
- **Parallel Execution**: Newman supports parallel runs with `--parallel` flag
- **Timeout**: Default timeout is 10 seconds per request

## Support

For issues or questions:
1. Check service logs: `docker logs capstone-authentication`
2. Verify environment configuration
3. Review test results for specific error messages
4. Consult API documentation

## Version History

- **v1.0.0** (2025-11-20): Initial release
  - User registration flow
  - JWT authentication
  - Data-driven testing support
  - 5 role-based data files (43 test accounts)
