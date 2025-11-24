# Order Microservice

Dedicated microservice for managing orders in the Food Delivery Application.

## Overview

The Order service handles all order-related operations including order creation, status management, packaging, and handover to delivery agents. This service follows proper microservices architecture principles with clear separation of concerns.

## Architecture

```
order/
├── API/                    # REST API endpoints
│   ├── Controllers/        # Order controllers
│   ├── OrderService.cs     # Business logic
│   ├── Program.cs          # Application entry point
│   └── appsettings.json    # Configuration
├── Models/                 # Domain models
│   └── Order.cs            # Order, OrderItem, PackagingDetails, etc.
├── DataAccess/             # Data persistence layer
│   └── OrderRepository.cs  # MongoDB repository
├── Order.sln               # Solution file
└── Dockerfile              # Container configuration
```

## Features

### Order Management
- **Create Order**: Customers can place new orders
- **View Orders**: Restaurant staff can view all orders
- **Get Pending Orders**: Operators can see orders awaiting acceptance
- **Get Ready Orders**: View orders ready for pickup
- **Order Status Management**: Track order lifecycle

### Order Status Flow
1. **Pending** → Order placed, awaiting acceptance
2. **Accepted** → Accepted by operator
3. **Declined** → Declined by operator
4. **Preparing** → Being prepared by kitchen workers
5. **ReadyForPickup** → Ready for delivery agent pickup
6. **OutForDelivery** → Handed over to delivery agent
7. **Delivered** → Delivered to customer
8. **Cancelled** → Order cancelled

### Packaging Management
- Add packaging details (cutlery, napkins, bags)
- Mark items as fragile
- Add special packaging notes
- Track who packaged the order and when

### Handover Management
- **Generate OTP**: Create secure 6-digit OTP for handover
- **Verify OTP**: Validate OTP during handover to delivery agent
- **Track Handover**: Record who handed over and when
- **OTP Expiry**: OTPs expire after 5 minutes for security

## API Endpoints

### Order Operations

#### Create Order
```http
POST /api/order
Authorization: Bearer <token>
Content-Type: application/json

{
  "customerId": "customer_id",
  "restaurantId": "restaurant_id",
  "items": [
    {
      "menuItemId": "item_id",
      "name": "Pizza Margherita",
      "quantity": 2,
      "price": 12.99
    }
  ],
  "totalAmount": 25.98
}
```

#### Get Pending Orders (Restaurant Staff)
```http
GET /api/order/restaurant/{restaurantId}/pending
Authorization: Bearer <token>
```

#### Accept Order (Operator)
```http
POST /api/order/{orderId}/accept
Authorization: Bearer <token>
```

#### Decline Order (Operator)
```http
POST /api/order/{orderId}/decline
Authorization: Bearer <token>
```

#### Update Order Status
```http
PATCH /api/order/{orderId}/status
Authorization: Bearer <token>
Content-Type: application/json

3  // OrderStatus enum value
```

### Packaging Operations

#### Package Order
```http
POST /api/order/{orderId}/package
Authorization: Bearer <token>
Content-Type: application/json

{
  "includeCutlery": true,
  "includeNapkins": true,
  "numberOfBags": 2,
  "isFragile": false,
  "specialPackagingNotes": "Keep upright"
}
```

### Handover Operations

#### Generate Handover OTP
```http
POST /api/order/{orderId}/generate-handover-otp
Authorization: Bearer <token>
Content-Type: application/json

"delivery_agent_id"
```

**Response:**
```json
{
  "message": "OTP generated successfully",
  "otp": "123456",
  "expiresIn": "5 minutes"
}
```

#### Verify and Complete Handover
```http
POST /api/order/handover
Authorization: Bearer <token>
Content-Type: application/json

{
  "orderId": "order_id",
  "deliveryAgentId": "agent_id",
  "otp": "123456",
  "operatorId": "operator_id"
}
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `MONGO_CONNECTION_STRING` | MongoDB connection string | `mongodb://localhost:27017` |
| `MONGO_DATABASE` | Database name | `OrderDb` |
| `ASPNETCORE_URLS` | Service URL | `http://+:80` |

### Database

The service uses MongoDB with the following collection:
- **Orders**: Stores all order documents

### MongoDB Schema

```javascript
{
  "_id": ObjectId,
  "customerId": String,
  "restaurantId": String,
  "restaurantName": String,
  "items": [
    {
      "menuItemId": String,
      "name": String,
      "quantity": Number,
      "price": Decimal,
      "specialInstructions": String
    }
  ],
  "totalAmount": Decimal,
  "status": Number,  // OrderStatus enum
  "packaging": {
    "includeCutlery": Boolean,
    "includeNapkins": Boolean,
    "numberOfBags": Number,
    "specialPackagingNotes": String,
    "isFragile": Boolean,
    "packagedAt": Date,
    "packagedBy": String
  },
  "deliveryAgentId": String,
  "handoverOTP": String,
  "handoverOTPGeneratedAt": Date,
  "isHandedOver": Boolean,
  "handoverTime": Date,
  "handoverBy": String,
  "notes": String,
  "createdAt": Date,
  "updatedAt": Date
}
```

## Authorization

The service uses JWT-based authentication with role-based access control:

- **RestaurantStaff**: Biller (Owner), Operator, Worker
- **Customer**: Can create and view own orders
- **DeliveryAgent**: Can view assigned orders

## Development

### Build the Project
```bash
dotnet build Order.sln
```

### Run the Service
```bash
dotnet run --project API/Order.API.csproj
```

### Run with Docker
```bash
# Build image
docker build -t order-service:latest .

# Run container
docker run -p 8005:80 \
  -e MONGO_CONNECTION_STRING="mongodb://localhost:27017" \
  -e MONGO_DATABASE="OrderDb" \
  order-service:latest
```

### Run in Kubernetes
```bash
kubectl apply -f ../../devops/kubernetes/local/order.yaml
```

## Port Configuration

- **Local Development**: `http://localhost:5005`
- **Docker**: Port `80` (mapped to host port)
- **Kubernetes**: Port `30005` (NodePort)

## Dependencies

- **.NET 8.0**: Web framework
- **MongoDB.Driver 3.5.0**: Database driver
- **JWT Bearer Authentication**: Security
- **Swashbuckle (Swagger)**: API documentation
- **Shared.DataAccess**: Common data access layer

## Testing

### Swagger UI
When running in development mode, access Swagger UI at:
```
http://localhost:5005
```

### Postman Collection
Use the Order Service Postman collection in:
```
fda/postman-collections/Order-Service.postman_collection.json
```

## Microservices Communication

The Order service may communicate with:
- **Catalog Service**: Verify menu items and availability
- **Cart Service**: Retrieve cart data when creating orders
- **Authentication Service**: Validate user tokens
- **Payment Service**: Process payments (future integration)

## Why Separate from Catalog?

Previously, OrderService was incorrectly placed in the Catalog service. Here's why it's now separate:

1. **Single Responsibility**: Orders and menu catalog are different domains
2. **Independent Scaling**: Order volume can scale independently from catalog
3. **Loose Coupling**: Changes to order logic don't affect catalog
4. **Team Ownership**: Different teams can own order and catalog services
5. **Database Separation**: Orders and catalog can have separate databases
6. **Deployment Independence**: Deploy order features without affecting catalog

## Future Enhancements

- [ ] Order tracking with real-time updates
- [ ] Order history and analytics
- [ ] Scheduled orders
- [ ] Recurring orders
- [ ] Order cancellation with refund logic
- [ ] Customer order ratings and reviews
- [ ] Integration with payment service
- [ ] Order notifications (SMS, Email, Push)

## Related Services

- **Catalog Service**: Menu and restaurant management
- **Cart Service**: Shopping cart management
- **Authentication Service**: User authentication and authorization
- **CRM Service**: Customer relationship management

---

**Version**: 1.0.0  
**Created**: November 24, 2025  
**Maintained By**: FDA Development Team
