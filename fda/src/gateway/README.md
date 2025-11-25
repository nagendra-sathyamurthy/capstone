# Capstone API Gateway

API Gateway for the Capstone Food Delivery Application. This gateway serves as a unified entry point for the customer-facing React application, routing requests to appropriate backend microservices.

## Features

- **Unified API Interface**: Single entry point for all backend services
- **Authentication Middleware**: Token validation and user authentication
- **Request Routing**: Intelligent routing to Authentication, Catalog, CRM, and Cart services
- **Error Handling**: Centralized error handling and transformation
- **CORS Support**: Configured for React app communication
- **Rate Limiting**: Protection against abuse and DDoS
- **Request Logging**: Comprehensive request/response logging
- **Health Checks**: Service health monitoring
- **Security**: Helmet.js security headers

## Architecture

```
Customer App (React)
        ↓
   API Gateway (Node.js/Express)
        ↓
   ┌─────┴─────┬─────────┬─────────┐
   ↓           ↓         ↓         ↓
Authentication Catalog   CRM      Cart
  Service     Service  Service  Service
```

## Getting Started

### Prerequisites

- Node.js 18+ installed
- Backend microservices running (Authentication, Catalog, CRM, Cart)
- Kubernetes cluster with services deployed (for production)

### Installation

1. Install dependencies:
```bash
cd C:\dotnet\capstone\fda\src\gateway
npm install
```

2. Configure environment:
```bash
# Copy example environment file
cp .env.example .env

# Edit .env with your configuration
# For local development, default values should work
```

3. Start the gateway:
```bash
# Development mode with auto-reload
npm run dev

# Production mode
npm start
```

The gateway will start on http://localhost:5000

## Environment Configuration

### Local Development (.env)
```env
PORT=5000
NODE_ENV=development

# Local Kubernetes NodePort services
AUTH_SERVICE_URL=http://localhost:30001
CATALOG_SERVICE_URL=http://localhost:30002
CRM_SERVICE_URL=http://localhost:30003
CART_SERVICE_URL=http://localhost:30004

CORS_ORIGIN=http://localhost:3000
```

### Production (.env.production)
```env
PORT=5000
NODE_ENV=production

# Kubernetes service names
AUTH_SERVICE_URL=http://authentication-service:8080
CATALOG_SERVICE_URL=http://catalog-service:8080
CRM_SERVICE_URL=http://crm-service:8080
CART_SERVICE_URL=http://cart-service:8080

CORS_ORIGIN=https://your-production-domain.com
```

## API Endpoints

### Health Check
```
GET /health - Gateway health status
GET / - Gateway information
```

### Authentication (/api/auth)
```
POST /api/auth/register - Register customer
POST /api/auth/send-otp - Send OTP
POST /api/auth/verify-otp - Verify OTP
POST /api/auth/login - Login
GET /api/auth/validate - Validate token
POST /api/auth/refresh - Refresh token
POST /api/auth/logout - Logout
```

### Catalog (/api/catalog)
```
GET /api/catalog/restaurants - List restaurants
GET /api/catalog/restaurants/:id - Get restaurant details
GET /api/catalog/items - List menu items
GET /api/catalog/items/:id - Get menu item details
GET /api/catalog/restaurants/:restaurantId/items - Get restaurant menu
GET /api/catalog/search - Search menu items
GET /api/catalog/cuisines - Get cuisines
```

### CRM (/api/crm)
```
GET /api/crm/profile - Get customer profile (auth required)
PUT /api/crm/profile - Update profile (auth required)
GET /api/crm/addresses - Get addresses (auth required)
POST /api/crm/addresses - Add address (auth required)
PUT /api/crm/addresses/:id - Update address (auth required)
DELETE /api/crm/addresses/:id - Delete address (auth required)
GET /api/crm/orders - Get order history (auth required)
GET /api/crm/favorites - Get favorites (auth required)
POST /api/crm/favorites - Add favorite (auth required)
DELETE /api/crm/favorites/:id - Remove favorite (auth required)
```

### Cart (/api/cart)
```
GET /api/cart - Get cart (auth required)
POST /api/cart/items - Add item (auth required)
PUT /api/cart/items/:itemId - Update quantity (auth required)
DELETE /api/cart/items/:itemId - Remove item (auth required)
DELETE /api/cart - Clear cart (auth required)
POST /api/cart/coupon - Apply coupon (auth required)
DELETE /api/cart/coupon - Remove coupon (auth required)
```

### Orders (/api/orders)
```
POST /api/orders - Create order (auth required)
GET /api/orders - Get customer orders (auth required)
GET /api/orders/:id - Get order details (auth required)
POST /api/orders/:id/cancel - Cancel order (auth required)
GET /api/orders/:id/track - Track order (auth required)
POST /api/orders/:id/rate - Rate order (auth required)
```

### Payments (/api/payments)
```
POST /api/payments/initiate - Initiate payment (auth required)
POST /api/payments/verify - Verify payment (auth required)
GET /api/payments/:id/status - Get payment status (auth required)
GET /api/payments - Get payment history (auth required)
POST /api/payments/:id/refund - Refund payment (auth required)
```

## Authentication

Protected routes require a Bearer token in the Authorization header:

```bash
Authorization: Bearer <your-jwt-token>
```

The gateway validates tokens with the Authentication service before forwarding requests.

## Error Handling

The gateway provides standardized error responses:

```json
{
  "error": true,
  "message": "Error description"
}
```

HTTP status codes:
- 200: Success
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 429: Too Many Requests
- 500: Internal Server Error
- 503: Service Unavailable

## Rate Limiting

Default: 100 requests per 15 minutes per IP address

Customize in `.env`:
```env
RATE_LIMIT_WINDOW_MS=900000
RATE_LIMIT_MAX_REQUESTS=100
```

## Logging

Development mode: Detailed colored console logs
Production mode: JSON structured logs

## Security Features

- **Helmet.js**: Security headers
- **CORS**: Configured origin validation
- **Rate Limiting**: DDoS protection
- **Token Validation**: JWT verification
- **Request Sanitization**: Input validation

## Testing

Test the gateway:

```bash
# Health check
curl http://localhost:5000/health

# Get restaurants (no auth)
curl http://localhost:5000/api/catalog/restaurants

# Get cart (auth required)
curl -H "Authorization: Bearer <token>" http://localhost:5000/api/cart
```

## Deployment

### Docker (Coming Soon)
```bash
docker build -t capstone-gateway .
docker run -p 5000:5000 --env-file .env capstone-gateway
```

### Kubernetes (Coming Soon)
```bash
kubectl apply -f k8s/gateway-deployment.yaml
```

## Monitoring

Monitor gateway health:
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

## Troubleshooting

### Gateway won't start
- Check if port 5000 is available
- Verify .env file exists and is configured
- Ensure Node.js 18+ is installed

### Cannot connect to backend services
- Verify backend services are running
- Check service URLs in .env
- For Kubernetes, verify services are accessible

### CORS errors
- Verify CORS_ORIGIN matches React app URL
- Check browser console for specific CORS errors

### Authentication failures
- Verify Authentication service is running
- Check token format (Bearer <token>)
- Ensure token is not expired

## Development

### Adding New Routes

1. Create route file in `routes/` directory
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

## Contributing

1. Follow existing code structure
2. Add error handling for all routes
3. Use asyncHandler for async routes
4. Update README with new endpoints
5. Test thoroughly before committing

## License

MIT
