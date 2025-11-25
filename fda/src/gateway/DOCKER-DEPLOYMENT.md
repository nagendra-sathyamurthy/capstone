# Gateway Docker Deployment

This directory contains the API Gateway for the Capstone Food Delivery Application, configured to run in Docker while connecting to backend services running in Kubernetes.

## Architecture

```
┌─────────────────┐
│  Customer App   │ (Docker/Browser)
│  Port 3000      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  API Gateway    │ (Docker Container)
│  Port 5000      │
└────────┬────────┘
         │
         ▼  host.docker.internal
┌─────────────────────────────────┐
│  Kubernetes Services (NodePort) │
│  - Authentication:30001         │
│  - Catalog:       30002         │
│  - CRM:           30003         │
│  - Cart:          30004         │
└─────────────────────────────────┘
```

## Quick Start

### Deploy Gateway in Docker

```powershell
# Navigate to gateway directory
cd C:\dotnet\capstone\fda\src\gateway

# Deploy using the script
.\docker-deploy.ps1
```

### Other Commands

```powershell
# Rebuild and redeploy
.\docker-deploy.ps1 -Rebuild

# Stop the gateway
.\docker-deploy.ps1 -Stop

# View logs
.\docker-deploy.ps1 -Logs

# Restart the gateway
.\docker-deploy.ps1 -Restart
```

## Manual Deployment

### Build the Docker Image

```powershell
docker build -t capstone-gateway:latest .
```

### Run the Container

```powershell
docker run -d \
  --name capstone-gateway \
  -p 5000:5000 \
  --add-host=host.docker.internal:host-gateway \
  capstone-gateway:latest
```

The `--add-host=host.docker.internal:host-gateway` flag allows the container to reach services on the host machine (Kubernetes NodePort services).

## Environment Configuration

The gateway uses different environment files:

- `.env` - Local development (Node.js running on host)
- `.env.docker` - Docker deployment (copied during build)

### Docker Environment (`.env.docker`)

```properties
PORT=5000
NODE_ENV=production

# Backend services via host.docker.internal
AUTH_SERVICE_URL=http://host.docker.internal:30001
CATALOG_SERVICE_URL=http://host.docker.internal:30002
CRM_SERVICE_URL=http://host.docker.internal:30003
CART_SERVICE_URL=http://host.docker.internal:30004

CORS_ORIGIN=http://localhost:3000,http://host.docker.internal:3000
```

## Verification

### Health Check

```powershell
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-11-25T06:05:25.015Z",
  "uptime": 16.040275892,
  "environment": "production"
}
```

### View Logs

```powershell
docker logs capstone-gateway

# Follow logs in real-time
docker logs -f capstone-gateway
```

### Check Container Status

```powershell
docker ps --filter "name=capstone-gateway"
```

## API Routes

### Authentication Routes
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout
- `GET /api/auth/verify` - Verify token
- `GET /api/auth/profile` - Get user profile

### Catalog Routes
- `GET /api/catalog/items` - Get all menu items
- `GET /api/catalog/items/:id` - Get item by ID
- `GET /api/catalog/items/category/:category` - Get items by category
- `GET /api/catalog/items/search/:query` - Search items
- `POST /api/catalog/items` - Create item (Admin)
- `PUT /api/catalog/items/:id` - Update item (Admin)
- `DELETE /api/catalog/items/:id` - Delete item (Admin)

### CRM Routes (Customer Management)

#### Customer Profile (Legacy)
- `GET /api/customers` - Get all customers
- `GET /api/customers/:id` - Get customer by ID
- `POST /api/customers` - Create customer
- `PUT /api/customers/:id` - Update customer
- `DELETE /api/customers/:id` - Delete customer

#### User Profile (New)
- `GET /api/userprofile/by-user/:userId` - Get user profile
- `GET /api/userprofile/by-user/:userId/addresses` - Get user addresses
- `POST /api/userprofile/by-user/:userId/addresses` - Add address
- `PUT /api/userprofile/by-user/:userId/addresses/:addressId` - Update address
- `DELETE /api/userprofile/by-user/:userId/addresses/:addressId` - Delete address
- `PUT /api/userprofile/by-user/:userId/profile-image` - Update profile image
- `PUT /api/userprofile/by-user/:userId/food-preferences` - Update food preferences

### Cart Routes
- `GET /api/cart/:userId` - Get user's cart
- `POST /api/cart/:userId/items` - Add item to cart
- `PUT /api/cart/:userId/items/:itemId` - Update cart item
- `DELETE /api/cart/:userId/items/:itemId` - Remove from cart
- `DELETE /api/cart/:userId` - Clear cart

## Troubleshooting

### Container Won't Start

```powershell
# Check if port 5000 is already in use
netstat -ano | findstr :5000

# View container logs
docker logs capstone-gateway

# Restart the container
docker restart capstone-gateway
```

### Cannot Reach Backend Services

1. Verify Kubernetes services are running:
```powershell
kubectl get pods -n capstone-services
kubectl get services -n capstone-services
```

2. Test NodePort connectivity from host:
```powershell
curl http://localhost:30003/api/health  # CRM service
```

3. Check gateway logs for connection errors:
```powershell
docker logs capstone-gateway | Select-String -Pattern "error|fail"
```

### CORS Issues

If you get CORS errors in the browser:
1. Check the gateway logs for the CORS_ORIGIN setting
2. Ensure customer app URL is in the allowed origins
3. Restart the gateway after environment changes

### Update Gateway Routes

If you've modified route files:

```powershell
# Rebuild and restart
.\docker-deploy.ps1 -Rebuild
```

## Docker Compose Integration

The gateway is also included in the full-stack Docker Compose setup:

```powershell
cd C:\dotnet\capstone\fda\devops\docker
docker-compose up -d gateway
```

This runs all services (MongoDB, backend services, gateway) in Docker.

## Development vs Docker

### Local Development (Node.js on Host)
- Uses `.env` file
- Services at `localhost:30001`, etc.
- Run with: `npm start`

### Docker Deployment (Recommended)
- Uses `.env.docker` file
- Services at `host.docker.internal:30001`
- Run with: `.\docker-deploy.ps1`

## Network Details

- Container Network Mode: Bridge (default)
- Host Access: Via `host.docker.internal` special DNS name
- Port Mapping: `5000:5000` (host:container)
- Backend Services: Kubernetes NodePort (30001-30004)

## Security Notes

1. The gateway runs in production mode when Dockerized
2. Rate limiting is enabled (100 requests per 15 minutes)
3. CORS is restricted to specified origins
4. Health check endpoint is always accessible
5. All requests are logged

## Performance

- Node.js 20 Alpine (minimal footprint)
- Production dependencies only
- Health check every 30 seconds
- Automatic container restart on failure

## Maintenance

### Update Dependencies

```powershell
# Update package.json on host
npm update

# Rebuild Docker image
.\docker-deploy.ps1 -Rebuild
```

### View Resource Usage

```powershell
docker stats capstone-gateway
```

### Clean Up

```powershell
# Stop and remove container
.\docker-deploy.ps1 -Stop

# Remove image
docker rmi capstone-gateway:latest
```
