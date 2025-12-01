# Capstone Food Delivery Application - AI Agent Instructions

## Architecture Overview

This is a **microservices-based food delivery platform** with:
- **Backend**: 6 .NET 8.0 services (Authentication, Catalog, CRM, Cart, Order, Payment)
- **API Gateway**: Node.js/Express reverse proxy at `src/gateway/`
- **Frontend**: React TypeScript app at `src/customer-app/`
- **Database**: Single shared MongoDB instance across all services
- **Deployment**: Kubernetes (local) and Docker Compose configurations

### Service Communication Flow
```
React App (port 3000/30080) 
  → API Gateway (port 5000/30007) 
    → Backend Services (ports 30001-30006)
      → MongoDB (port 27017/30000)
```

Gateway routes all frontend API calls to backend services using path-based routing (`/api/auth/*`, `/api/catalog/*`, etc.).

## .NET Service Structure

Each backend service follows a **3-layer architecture**:

```
src/services/{service}/
├── Models/{Service}.Models.csproj          # Domain models with MongoDB attributes
├── DataAccess/{Service}.DataAccess.csproj  # Repository pattern implementation
└── API/{Service}.API.csproj                # Controllers, Program.cs, DI setup
```

**Critical Pattern**: All services use PascalCase naming (e.g., `Authentication.API.csproj`, `Catalog.Models.csproj`). Namespace collisions are common—explicitly qualify types like `Cart.Models.Cart` when referencing the Cart model.

### Repository Pattern
- `IRepository<T>` defines CRUD operations (GetAll, GetById, Insert, Update, Delete, Find)
- `MongoRepository<T>` provides base MongoDB implementation
- Domain-specific repositories (e.g., `MenuItemRepository`, `UserAccountRepository`) extend `MongoRepository<T>`
- All repositories handle both `ObjectId` and string-based IDs

### JWT Authentication Configuration
Services use JWT with a shared secret key (`GJ0VFqmRVBR0iE2ojyzh28HlayZgRcUI` in dev). JWT setup is in each service's `Program.cs`:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => {
    options.IssuerSigningKey = new SymmetricSecurityKey(key);
    options.ValidateIssuer = false;
    options.ValidateAudience = false;
});
```

MongoDB connection strings are read from `MONGO_CONNECTION_STRING` environment variable, defaulting to `mongodb://localhost:27017`.

## Role-Based Access Control (RBAC)

Authentication service implements comprehensive RBAC with organizational hierarchy (see `docs/services/RBAC-COMPREHENSIVE.md`):

- **external_users**: Customers (order, track, pay)
- **Restaurant Organizations**: Biller (owner/payment), Operator (order receiver/SPOC), Worker (kitchen staff)
- **fda_delivery_network**: DeliveryAgent
- **fda_it_department**: Developer, Tester, NetworkAdmin, DatabaseAdmin

JWT tokens include role and organization claims. Use `[Authorize(Policy = "RoleName")]` attributes on controllers.

## Development Workflows

### Building Services
```powershell
# Build entire solution
cd src/services
dotnet build capstone.sln -c Release

# Build specific service
dotnet build authentication/API/Authentication.API.csproj

# Run service locally
dotnet run --project catalog/API/Catalog.API.csproj
```

### Local Deployment

**Kubernetes (Recommended)**:
```powershell
cd devops/kubernetes
.\deploy-full-stack-local.ps1  # Deploys everything to k8s
```
Access: Customer App at http://localhost:30080, Gateway at http://localhost:30007

**Docker Compose**:
```powershell
cd devops/jobs
docker-compose -f docker-compose-local.yml up -d
```

**Development Mode** (Gateway + MongoDB only, services run locally):
```powershell
docker-compose -f docker-compose.dev.yml up -d
```

### Testing with Newman/Postman
All API workflows have Postman collections in `postman-collections/`:
- `user-registration/` - Customer, owner, worker, delivery agent registration
- `restaurant-owner-workflows/` - Menu management, UPI setup
- `operator-service-workflows/` - Order handling, inventory updates

Run all tests:
```powershell
cd postman-collections
.\Run-All-Tests.ps1  # Generates HTML reports in test-results/
```

## Key Integration Points

### Frontend-Gateway Communication
- React app uses axios with `REACT_APP_GATEWAY_URL` environment variable (defaults to `http://localhost:5000`)
- All API calls go through gateway at `/api/{service}/...`
- Gateway has CORS configured for `http://localhost:3000`
- See `src/customer-app/src/services/api.ts` for API client setup

### MongoDB Collections
- Authentication: `useraccounts`, `otps`
- Catalog: `restaurants`, `menuitems`, `orders`
- CRM: `userprofiles` (addresses, food preferences, profile images)
- Cart: `carts`, `cartitems`
- Order: `orders`, `orderstatus`

### Customer App State Management
- Uses React Context for cart state (`src/customer-app/src/context/CartContext.tsx`)
- User profile data stored in MongoDB via CRM service (NOT localStorage)
- Only auth tokens stored in localStorage (`authToken`, `userId`, `userPhone`)

## Project-Specific Conventions

1. **Solution Management**: When renaming/adding projects, update `capstone.sln` with `dotnet sln add {path}` and fix all `<ProjectReference>` paths in `.csproj` files.

2. **Environment Variables**: Use `MONGO_CONNECTION_STRING` for database connections, never hardcode. For Kubernetes, secrets are in `devops/kubernetes/local/mongodb-secret.yaml`.

3. **Port Standardization**:
   - NodePort services: 30000 (MongoDB), 30001-30006 (backend services), 30007 (gateway), 30080 (frontend)
   - Container ports: 80 for .NET services, 5000 for gateway, 80 for React nginx
   - Local development: 5001-5006 for backend services

4. **Swagger/OpenAPI**: All .NET services have Swagger UI enabled at `/swagger`. JWT authentication is configured in Swagger for testing authenticated endpoints.

5. **Error Handling**: Gateway centralizes error transformation and logging. Backend services return standard HTTP status codes with problem details.

## Common Pitfalls

- **Namespace Collisions**: `Cart.Models.Cart` vs namespace `Cart.Models`—always fully qualify ambiguous types
- **MongoDB Connection**: Services won't start without valid MongoDB connection. For k8s, ensure MongoDB pod is ready before deploying services.
- **JWT Secret Mismatch**: All services must use the same JWT secret key for token validation
- **Gateway Routing**: Gateway routes are defined in `src/gateway/routes/{service}.js`—update when adding new endpoints
- **Docker Images**: For k8s deployment, images must be built locally with tags like `capstone-{service}:latest`

## Migration Notes

Currently migrating from localStorage to MongoDB for user data (see `MONGODB-MIGRATION-PLAN.md`). Profile images, addresses, and food preferences now stored in CRM service's `userprofiles` collection via dedicated API endpoints.

## Useful File References

- **Service DI Setup**: `src/services/*/API/Program.cs`
- **Data Access Patterns**: `src/services/*/DataAccess/MongoRepository.cs`, `IRepository.cs`
- **Gateway Configuration**: `src/gateway/server.js`, `src/gateway/.env.example`
- **K8s Manifests**: `devops/kubernetes/local/*.yaml`
- **Deployment Scripts**: `devops/kubernetes/deploy-full-stack-local.ps1`
- **Test Data**: `postman-collections/*/data/*.json`

When making changes to services, verify builds with `dotnet build {service}.sln` and test with Newman collections before deployment.
