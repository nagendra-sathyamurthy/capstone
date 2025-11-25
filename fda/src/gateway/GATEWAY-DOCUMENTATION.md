# API Gateway Documentation

## Overview

The API Gateway serves as a unified entry point for the Capstone Food Delivery customer application. It routes requests to the appropriate backend microservices, handles authentication, CORS, rate limiting, and provides a consistent API interface.

## Architecture

```
┌─────────────────────┐
│  Customer App       │
│  (React - Port 3000)│
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│  API Gateway        │
│  (Node.js - Port    │
│   5000/30005)       │
└──────────┬──────────┘
           │
    ┌──────┴──────────────────┬──────────────┬──────────────┐
    ↓                         ↓              ↓              ↓
┌──────────┐            ┌──────────┐   ┌──────────┐   ┌──────────┐
│  Auth    │            │ Catalog  │   │   CRM    │   │   Cart   │
│  Service │            │ Service  │   │ Service  │   │ Service  │
│ :30001   │            │ :30002   │   │ :30003   │   │ :30004   │
└──────────┘            └──────────┘   └──────────┘   └──────────┘
```

## Setup

### Local Development

1. **Install Dependencies**
   ```bash
   cd C:\dotnet\capstone\fda\src\gateway
   npm install
   ```

2. **Configure Environment**
   - Copy `.env.example` to `.env`
   - Default configuration works for local Kubernetes NodePort services

3. **Start Gateway**
   ```bash
   npm start           # Production mode
   npm run dev         # Development mode with auto-reload
   ```

   Gateway will be available at: `http://localhost:5000`

### Docker Deployment

1. **Build Image**
   ```bash
   cd C:\dotnet\capstone\fda\src\gateway
   docker build -t capstone-gateway:latest .
   ```

2. **Run Container**
   ```bash
   docker run -p 5000:5000 --env-file .env capstone-gateway:latest
   ```

### Kubernetes Deployment

#### Local Kubernetes

```bash
# Build and deploy
cd C:\dotnet\capstone\fda\devops\kubernetes
.\deploy-gateway-local.ps1

# Or manually
kubectl apply -f local/gateway.yaml
```

**Access**: `http://localhost:30005`

#### Production Kubernetes

```bash
# Deploy
kubectl apply -f production/gateway.yaml

# Verify
kubectl get all -n capstone-gateway
```

**Access**: Through LoadBalancer IP/Domain

## API Endpoints

### Health & Info

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/health` | GET | Health check |
| `/` | GET | Gateway info |

### Authentication (`/api/auth`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/auth/register` | POST | No | Register customer |
| `/api/auth/send-otp` | POST | No | Send OTP to phone |
| `/api/auth/verify-otp` | POST | No | Verify OTP |
| `/api/auth/login` | POST | No | Login |
| `/api/auth/validate` | GET | Yes | Validate token |
| `/api/auth/refresh` | POST | Yes | Refresh token |
| `/api/auth/logout` | POST | Yes | Logout |

### Catalog (`/api/catalog`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/catalog/restaurants` | GET | No | List restaurants |
| `/api/catalog/restaurants/:id` | GET | No | Get restaurant |
| `/api/catalog/items` | GET | No | List menu items |
| `/api/catalog/items/:id` | GET | No | Get menu item |
| `/api/catalog/restaurants/:restaurantId/items` | GET | No | Get restaurant menu |
| `/api/catalog/search` | GET | No | Search items |
| `/api/catalog/cuisines` | GET | No | Get cuisines |

### CRM (`/api/crm`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/crm/profile` | GET | Yes | Get profile |
| `/api/crm/profile` | PUT | Yes | Update profile |
| `/api/crm/addresses` | GET | Yes | Get addresses |
| `/api/crm/addresses` | POST | Yes | Add address |
| `/api/crm/addresses/:id` | PUT | Yes | Update address |
| `/api/crm/addresses/:id` | DELETE | Yes | Delete address |
| `/api/crm/orders` | GET | Yes | Order history |
| `/api/crm/favorites` | GET | Yes | Get favorites |
| `/api/crm/favorites` | POST | Yes | Add favorite |
| `/api/crm/favorites/:id` | DELETE | Yes | Remove favorite |

### Cart (`/api/cart`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/cart` | GET | Yes | Get cart |
| `/api/cart/items` | POST | Yes | Add item |
| `/api/cart/items/:itemId` | PUT | Yes | Update quantity |
| `/api/cart/items/:itemId` | DELETE | Yes | Remove item |
| `/api/cart` | DELETE | Yes | Clear cart |
| `/api/cart/coupon` | POST | Yes | Apply coupon |
| `/api/cart/coupon` | DELETE | Yes | Remove coupon |

### Orders (`/api/orders`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/orders` | POST | Yes | Create order |
| `/api/orders` | GET | Yes | Get orders |
| `/api/orders/:id` | GET | Yes | Get order |
| `/api/orders/:id/cancel` | POST | Yes | Cancel order |
| `/api/orders/:id/track` | GET | Yes | Track order |
| `/api/orders/:id/rate` | POST | Yes | Rate order |

### Payments (`/api/payments`)

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/payments/initiate` | POST | Yes | Initiate payment |
| `/api/payments/verify` | POST | Yes | Verify payment |
| `/api/payments/:id/status` | GET | Yes | Payment status |
| `/api/payments` | GET | Yes | Payment history |
| `/api/payments/:id/refund` | POST | Yes | Refund payment |

## Configuration

### Environment Variables

```env
# Server
PORT=5000
NODE_ENV=development|production

# Backend Services
AUTH_SERVICE_URL=http://localhost:30001
CATALOG_SERVICE_URL=http://localhost:30002
CRM_SERVICE_URL=http://localhost:30003
CART_SERVICE_URL=http://localhost:30004

# CORS
CORS_ORIGIN=http://localhost:3000

# Rate Limiting
RATE_LIMIT_WINDOW_MS=900000      # 15 minutes
RATE_LIMIT_MAX_REQUESTS=100      # Max requests per window

# Logging
LOG_LEVEL=info
```

### Kubernetes ConfigMap

Located in `gateway.yaml`:
- Local: Uses NodePort services (30001-30004)
- Production: Uses cluster DNS (service.namespace.svc.cluster.local)

## Features

### 1. Request Routing
- Routes requests to appropriate microservices
- Maintains consistent API structure
- Handles service discovery

### 2. Authentication
- JWT token validation
- Token refresh mechanism
- Authorization header injection

### 3. CORS Management
- Configured for React app origin
- Handles preflight requests
- Credentials support

### 4. Rate Limiting
- IP-based rate limiting
- Configurable window and max requests
- DDoS protection

### 5. Error Handling
- Centralized error transformation
- Consistent error responses
- Service availability handling

### 6. Logging
- Request/response logging
- Development vs production modes
- Morgan integration

### 7. Security
- Helmet.js security headers
- Input sanitization
- HTTPS support

## Monitoring

### Health Check
```bash
curl http://localhost:5000/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2025-11-25T10:30:00.000Z",
  "uptime": 3600,
  "environment": "development"
}
```

### Kubernetes Status
```bash
# Pods
kubectl get pods -n capstone-gateway

# Services
kubectl get svc -n capstone-gateway

# Logs
kubectl logs -f deployment/gateway -n capstone-gateway

# Describe
kubectl describe deployment gateway -n capstone-gateway
```

### Metrics (Production)
- CPU usage
- Memory usage
- Request rate
- Error rate
- Response time

## Troubleshooting

### Gateway won't start
```bash
# Check if port is in use
netstat -ano | findstr :5000

# Check environment
cat .env

# Check logs
npm start
```

### Cannot reach backend services
```bash
# Test service directly
curl http://localhost:30001/health

# Check Kubernetes services
kubectl get svc -n capstone-services

# Check gateway configuration
kubectl describe configmap gateway-config -n capstone-gateway
```

### CORS errors
- Verify `CORS_ORIGIN` matches React app URL
- Check browser console for specific errors
- Ensure credentials are configured

### Authentication failures
- Verify token format: `Bearer <token>`
- Check token expiration
- Test auth service directly

### High latency
- Check backend service response times
- Review rate limiting configuration
- Monitor resource usage

## Testing

### Manual Testing
```bash
# Health check
curl http://localhost:5000/health

# Get restaurants (no auth)
curl http://localhost:5000/api/catalog/restaurants

# Get cart (with auth)
curl -H "Authorization: Bearer <token>" \
     http://localhost:5000/api/cart
```

### Load Testing
```bash
# Using Apache Bench
ab -n 1000 -c 10 http://localhost:5000/api/catalog/restaurants

# Using wrk
wrk -t12 -c400 -d30s http://localhost:5000/health
```

## Development

### Adding New Routes

1. Create route file in `routes/`
2. Import in `server.js`
3. Register route with `app.use()`

Example:
```javascript
// routes/newService.js
const express = require('express');
const router = express.Router();

router.get('/endpoint', async (req, res) => {
  // Implementation
});

module.exports = router;

// server.js
const newServiceRoutes = require('./routes/newService');
app.use('/api/new-service', newServiceRoutes);
```

### Middleware

Located in `middleware/`:
- `authMiddleware.js` - Token validation
- `errorHandler.js` - Error transformation
- `requestLogger.js` - Request logging

## Performance

### Optimization
- Connection pooling for backend services
- Response caching (future)
- Request deduplication (future)
- Compression middleware (future)

### Scaling
- Horizontal scaling with multiple replicas
- Load balancing via Kubernetes Service
- Auto-scaling based on CPU/Memory (HPA)

## Security

### Best Practices
- Always use HTTPS in production
- Rotate secrets regularly
- Monitor for suspicious activity
- Keep dependencies updated
- Use security headers (Helmet.js)

### Production Checklist
- [ ] HTTPS enabled
- [ ] Strong CORS configuration
- [ ] Rate limiting tuned
- [ ] Secrets in Kubernetes Secrets
- [ ] Logging configured
- [ ] Monitoring enabled
- [ ] Auto-scaling configured
- [ ] Backup strategy in place

## Maintenance

### Updates
```bash
# Update dependencies
npm update

# Check for vulnerabilities
npm audit

# Fix vulnerabilities
npm audit fix
```

### Backup
- ConfigMap configuration
- Environment variables
- Route definitions
- Middleware logic

## Support

For issues or questions:
1. Check logs: `kubectl logs -f deployment/gateway -n capstone-gateway`
2. Verify services are running
3. Review configuration
4. Test backend services directly

## Future Enhancements

- [ ] Response caching with Redis
- [ ] GraphQL support
- [ ] WebSocket support for real-time features
- [ ] Request/response transformation
- [ ] API versioning
- [ ] Circuit breaker pattern
- [ ] Service mesh integration
- [ ] Distributed tracing (OpenTelemetry)
- [ ] API documentation (Swagger/OpenAPI)
