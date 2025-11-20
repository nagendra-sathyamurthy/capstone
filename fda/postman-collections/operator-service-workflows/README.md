# Operator Service Workflows - Postman Collection

Comprehensive workflow collection for restaurant operators to manage orders, inventory, packaging, and handover to delivery agents in the food delivery system.

## Overview

This collection covers all essential operator operations across 4 main workflows:
1. **Order Management** - View, accept, and decline orders
2. **Inventory Management** - Monitor stock levels and update food item availability
3. **Order Packaging** - Handle packaging with/without cutlery
4. **Delivery Handover** - Secure OTP-based handover to delivery agents

## Prerequisites

- **Authentication Service** running on `http://localhost:30001`
- **Catalog Service** running on `http://localhost:30002`
- Valid operator account (role: Operator)
- At least one restaurant registered with menu items
- At least one test order in the system

## Test Accounts

### Operator Account
- **Email**: `operator.pizzapalace@example.com`
- **Password**: `Operator123!`
- **Restaurant**: Pizza Palace Downtown
- **Role**: Operator (Single Point of Contact for order management)

### Additional Test Operators
- `operator.sushispot@example.com` - Sushi Spot
- `operator.burgerhub@example.com` - Burger Hub
- `operator.tacotown@example.com` - Taco Town

## Collection Structure

### 1. Authentication
- **Login Operator** - Authenticates operator and stores JWT token
- **Set Restaurant Context** - Retrieves restaurant details for context

### 2. Order Management - View Orders
- **Get Pending Orders** - Lists all orders awaiting acceptance
  - Filters by restaurant ID
  - Returns orders with status: Pending
  - Automatically saves first order ID for subsequent requests
- **Get Orders Ready for Pickup** - Lists orders packaged and ready
  - Returns orders with status: ReadyForPickup
  - Used by operators to notify delivery agents
- **Get Order Details** - Retrieves complete order information
  - Customer details
  - Order items with quantities
  - Current status and timestamps
  - Packaging information (if packaged)

### 3. Order Management - Accept/Decline
- **Accept Order** - Confirms order and starts preparation
  - Changes status from Pending → Accepted
  - Triggers kitchen notification
  - Records acceptance timestamp
- **Decline Order** - Rejects order (out of stock, closed, etc.)
  - Changes status from Pending → Declined
  - Notifies customer of rejection
  - Records decline timestamp

### 4. Inventory Management - View
- **View Kitchen Inventory** - Lists all menu items with quantities
  - Shows current stock levels (QuantityAvailable)
  - Displays minimum quantity thresholds
  - Shows availability status and timings
  - Automatically saves first menu item ID
- **Get Low Stock Items** - Alert system for items needing restock
  - Returns items where: `QuantityAvailable ≤ MinimumQuantity`
  - Helps operators proactively manage inventory
  - Prevents order acceptance when ingredients unavailable

### 5. Inventory Management - Update
- **Update Food Item Availability** - Modify quantity and availability
  - Update `quantityAvailable` (current stock count)
  - Toggle `isAvailable` (true/false)
  - Supports partial updates (quantity only, status only, or both)
  
- **Update Food Item Timings** - Modify availability time windows
  - Set `availableFromTime` (e.g., "09:00" for breakfast items)
  - Set `availableToTime` (e.g., "11:30" for breakfast cutoff)
  - Useful for time-specific menu items
  
- **Mark Item Out of Stock** - Temporarily disable item
  - Sets `quantityAvailable` to 0
  - Sets `isAvailable` to false
  - Hides item from customer ordering
  
- **Mark Item Back in Stock** - Re-enable item after restock
  - Updates `quantityAvailable` to new stock level
  - Sets `isAvailable` to true
  - Makes item visible to customers again

### 6. Order Packaging
- **Package Order with Cutlery** - Standard packaging
  - Include cutlery, napkins, bags
  - Records operator ID who packaged
  - Sets `PackagedAt` timestamp
  - Example use case: Dine-in style orders
  
- **Package Order without Cutlery** - Minimal packaging
  - Customer has own utensils
  - Reduces waste and cost
  - Example use case: Regular customers, eco-friendly requests
  
- **Package Fragile Order** - Special handling
  - Mark as `isFragile: true`
  - Extra bags for protection
  - Delivery agent gets special handling instructions
  - Example use case: Soups, beverages, delicate desserts

**PackagingDetails Schema**:
```json
{
  "includeCutlery": true/false,
  "includeNapkins": true/false,
  "numberOfBags": 1-5,
  "isFragile": true/false,
  "packagedBy": "operator_user_id",
  "packagedAt": "2025-11-20T10:30:00Z" // Auto-set by system
}
```

### 7. Handover to Delivery Agent
- **Generate Handover OTP** - Create secure 6-digit code
  - Links OTP to specific order and delivery agent
  - 5-minute expiry for security
  - Returns OTP in response (for testing)
  - Automatically saves OTP in collection variable
  - **Production note**: OTP should be sent via SMS/app notification
  
- **Verify OTP and Complete Handover** - Confirm delivery agent pickup
  - Multi-factor validation:
    - OTP must match
    - Delivery agent ID must match
    - OTP must not be expired (< 5 minutes)
  - On success:
    - Sets `isHandedOver: true`
    - Records handover timestamp
    - Changes order status to `OutForDelivery`
    - Records operator who completed handover
  
- **Verify with Invalid OTP** - Test security validation
  - Demonstrates OTP rejection
  - Returns 400 Bad Request with error message
  - Used for testing security controls

**Handover Security Features**:
- 6-digit numeric OTP (e.g., "847593")
- 5-minute expiry window
- One-time use (invalidated after successful verification)
- Delivery agent binding (OTP only valid for assigned agent)
- Failed attempt logging (for security audit)

### 8. Complete Operator Workflow
End-to-end demonstration of all operator activities in proper sequence:

1. **View Pending Orders** - Check new orders
2. **Get Order Details** - Review order requirements
3. **Check Low Stock Items** - Verify ingredient availability
4. **Accept Order** - Confirm order to customer and kitchen
5. **Update Food Item Availability** - Adjust inventory after order
6. **Package Order with Cutlery** - Prepare order for delivery
7. **Generate Handover OTP** - Create secure code for delivery agent
8. **Verify OTP and Complete Handover** - Transfer order to agent

## Collection Variables

The collection automatically manages these variables:
- `auth_token` - JWT authentication token (set on login)
- `operator_id` - Operator's user account ID (set on login)
- `restaurant_id` - Restaurant ID context (set manually or from API)
- `order_id` - Current order being processed (set from pending orders)
- `menu_item_id` - Current menu item being updated (set from inventory)
- `delivery_agent_id` - Delivery agent ID for handover (set manually)
- `handover_otp` - Generated OTP code (set from generate-otp response)

## Usage Instructions

### Complete Workflow Execution

1. **Start Services**
   ```bash
   # Authentication Service
   cd fda/src/services/authentication/API
   dotnet run

   # Catalog Service (in new terminal)
   cd fda/src/services/catalog/API
   dotnet run
   ```

2. **Import Collection**
   - Open Postman
   - Import `Operator-Service-Workflows.postman_collection.json`
   - Import `Capstone-Local-Environment.postman_environment.json` (if not already imported)

3. **Set Test Data** (Before running)
   - Update `restaurant_id` in "Set Restaurant Context" pre-request script
   - Update `delivery_agent_id` in "Generate Handover OTP" pre-request script
   - Ensure test orders exist in the system

4. **Run Collection**
   - Execute sections in order (1 → 8)
   - Each request uses variables from previous steps
   - Review test assertions to verify success

### Individual Workflow Examples

**Workflow 1: Order Processing**
```
1. Login Operator
2. Get Pending Orders → saves order_id
3. Get Order Details → review requirements
4. Accept Order → confirm to kitchen
```

**Workflow 2: Inventory Management**
```
1. Login Operator
2. View Kitchen Inventory → saves menu_item_id
3. Get Low Stock Items → check alerts
4. Update Food Item Availability → adjust stock
5. View Kitchen Inventory → verify changes
```

**Workflow 3: Order Fulfillment**
```
1. Login Operator
2. Get Orders Ready for Pickup → find packaged orders
3. Package Order with Cutlery → prepare order
4. Generate Handover OTP → create secure code
5. Verify OTP and Complete Handover → transfer to agent
```

**Workflow 4: Emergency Out of Stock**
```
1. Login Operator
2. Get Low Stock Items → identify critical items
3. Mark Item Out of Stock → disable ordering
4. Accept/Decline pending orders → manage existing orders
5. Mark Item Back in Stock → re-enable after restock
```

## API Endpoints Reference

### Order Management Endpoints
- `GET /api/operator/orders/pending/{restaurantId}` - Get pending orders
- `GET /api/operator/orders/ready/{restaurantId}` - Get orders ready for pickup
- `GET /api/operator/orders/{orderId}` - Get order details
- `POST /api/operator/orders/{orderId}/accept` - Accept order
- `POST /api/operator/orders/{orderId}/decline` - Decline order

### Inventory Management Endpoints
- `GET /api/operator/inventory/{restaurantId}` - View kitchen inventory
- `GET /api/operator/inventory/{restaurantId}/low-stock` - Get low stock alerts
- `PATCH /api/operator/inventory/{menuItemId}/availability` - Update availability

### Packaging Endpoints
- `POST /api/operator/orders/{orderId}/package` - Package order

### Handover Endpoints
- `POST /api/operator/orders/{orderId}/generate-handover-otp` - Generate OTP
- `POST /api/operator/orders/handover` - Verify OTP and complete handover

## Test Assertions

Each request includes comprehensive test scripts:
- ✅ Correct HTTP status codes (200, 400, 404)
- ✅ Response structure validation
- ✅ Required fields presence checks
- ✅ Data relationships verification
- ✅ Variable extraction for subsequent requests
- ✅ Business logic validation (low stock thresholds, OTP expiry)

## Features Demonstrated

### Order Management
- Real-time pending order queue
- Order acceptance/rejection workflow
- Order status transitions (Pending → Accepted/Declined)
- Complete order details with customer info

### Inventory Control
- Live kitchen inventory visibility
- Stock quantity tracking with thresholds
- Low stock alert system (automated warnings)
- Time-based availability windows (breakfast, lunch, dinner items)
- Dynamic availability toggling (in-stock/out-of-stock)

### Packaging Workflow
- Flexible packaging options (cutlery, napkins, bags)
- Fragile item special handling
- Operator accountability (PackagedBy tracking)
- Timestamp tracking (PackagedAt)
- Custom packaging based on order requirements

### Secure Handover System
- OTP-based authentication (6-digit codes)
- Time-limited OTPs (5-minute expiry)
- Delivery agent binding (OTP tied to specific agent)
- Multi-factor validation (OTP + AgentId + Expiry)
- Handover audit trail (who, when, to whom)
- Order status automation (auto-transition to OutForDelivery)

## Data Model Reference

### Order Model
```json
{
  "id": "order_id",
  "customerId": "customer_user_id",
  "restaurantId": "restaurant_id",
  "items": [
    {
      "menuItemId": "item_id",
      "name": "Pizza Margherita",
      "quantity": 2,
      "price": 12.99,
      "specialInstructions": "Extra cheese"
    }
  ],
  "totalAmount": 25.98,
  "status": "Pending|Accepted|Declined|Preparing|ReadyForPickup|OutForDelivery|Delivered|Cancelled",
  "packaging": {
    "includeCutlery": true,
    "includeNapkins": true,
    "numberOfBags": 2,
    "isFragile": false,
    "packagedBy": "operator_id",
    "packagedAt": "2025-11-20T10:30:00Z"
  },
  "handoverOTP": "847593",
  "handoverOTPGeneratedAt": "2025-11-20T10:35:00Z",
  "deliveryAgentId": "agent_id",
  "isHandedOver": false,
  "handoverTime": null,
  "handoverBy": null,
  "createdAt": "2025-11-20T10:15:00Z",
  "updatedAt": "2025-11-20T10:30:00Z"
}
```

### MenuItem Model (Inventory)
```json
{
  "id": "menu_item_id",
  "restaurantId": "restaurant_id",
  "name": "Chicken Tikka Masala",
  "description": "Tender chicken in creamy tomato sauce",
  "category": "Main Course",
  "price": 14.99,
  "isAvailable": true,
  "quantityAvailable": 25,
  "minimumQuantity": 5,
  "availableFromTime": "11:00",
  "availableToTime": "22:00",
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false,
  "createdAt": "2025-11-15T08:00:00Z",
  "updatedAt": "2025-11-20T09:45:00Z"
}
```

### InventoryUpdateRequest
```json
{
  "quantityAvailable": 50,
  "isAvailable": true,
  "availableFromTime": "09:00",
  "availableToTime": "22:00"
}
```
*Note: All fields are optional - supports partial updates*

### HandoverRequest
```json
{
  "orderId": "order_id",
  "deliveryAgentId": "agent_id",
  "otp": "847593",
  "operatorId": "operator_id"
}
```

## Role-Based Access Control (RBAC)

### Operator Permissions
According to [RBAC-COMPREHENSIVE.md](../../docs/services/RBAC-COMPREHENSIVE.md):

**Core Features**:
- ✅ Single Point of Contact (SPOC) for order management
- ✅ Customer service coordination
- ✅ Order workflow management (accept/decline/modify)
- ✅ Food item availability and timing management
- ✅ Kitchen inventory visibility and stock alerts
- ✅ Order packaging and preparation
- ✅ Delivery handover with OTP verification

**JWT Claims Required**:
- `role`: "Operator"
- `organization`: Restaurant-specific (e.g., "pizzapalace")
- `restaurantName`: "Pizza Palace Downtown"
- `employeeId`: Operator employee ID
- `position`: "Operator" or "SPOC"
- `department`: "Operations" or "Customer Service"

**Authorization Policy**: `RestaurantStaff` (includes Biller, Operator, Worker roles)

### Security Features
- JWT Bearer authentication required for all endpoints
- Restaurant-scoped access (operators can only access their restaurant's data)
- Role validation at controller level (`[Authorize(Policy = "RestaurantStaff")]`)
- OTP expiry enforcement (5-minute window)
- Delivery agent verification (OTP bound to specific agent)
- Audit trail for all operations (who, what, when)

## Environment Variables

Required environment configuration:
```json
{
  "MONGO_CONNECTION_STRING": "mongodb://localhost:27017",
  "JWT_SECRET": "your-256-bit-secret-key",
  "CATALOG_SERVICE_URL": "http://localhost:30002",
  "AUTH_SERVICE_URL": "http://localhost:30001"
}
```

## Troubleshooting

### Common Issues

**401 Unauthorized**
- **Cause**: JWT token expired or invalid
- **Solution**: Re-run "Login Operator" request to refresh token

**403 Forbidden**
- **Cause**: User not in Operator role or wrong restaurant
- **Solution**: Verify account has Operator role and correct restaurant association

**404 Not Found - Orders**
- **Cause**: No pending orders exist in system
- **Solution**: Create test orders using customer account or seed data

**404 Not Found - Menu Items**
- **Cause**: Restaurant has no menu items configured
- **Solution**: Use Restaurant Owner collection to add menu items first

**400 Bad Request - OTP Expired**
- **Cause**: More than 5 minutes elapsed since OTP generation
- **Solution**: Re-generate OTP using "Generate Handover OTP" request

**400 Bad Request - Invalid OTP**
- **Cause**: OTP mismatch or delivery agent ID mismatch
- **Solution**: 
  - Verify OTP value in collection variable
  - Check delivery agent ID matches original generation request
  - Ensure OTP not already used (one-time use only)

**400 Bad Request - Low Stock**
- **Cause**: Attempting to accept order when ingredients unavailable
- **Solution**: 
  - Check "Get Low Stock Items" endpoint
  - Update inventory using "Update Food Item Availability"
  - Or decline the order if ingredients truly unavailable

### Debugging Tips

1. **Check Collection Variables**: View current values in Postman Variables tab
2. **Review Test Results**: Failed assertions indicate specific validation issues
3. **Check Console**: Pre-request and test script logs appear in Postman Console
4. **Verify Service Status**: Ensure both Authentication and Catalog services running
5. **Check Database**: Use MongoDB Compass to verify data exists (orders, menu items)

## Testing Scenarios

### Scenario 1: Rush Hour Order Management
```
Objective: Handle multiple orders efficiently during peak hours

Steps:
1. Login Operator
2. Get Pending Orders → should show 5+ orders
3. Check Low Stock Items → verify ingredients available
4. Accept Order #1 → confirm to kitchen
5. Accept Order #2 → confirm to kitchen
6. Check Low Stock Items → see reduced quantities
7. Mark critical item out of stock → prevent overselling
8. Decline remaining orders → notify customers
```

### Scenario 2: Inventory Restock
```
Objective: Manage inventory after supplier delivery

Steps:
1. Login Operator
2. Get Low Stock Items → identify items needing restock
3. Mark Item Out of Stock → disable chicken tikka (temporarily)
4. Update Food Item Availability → increase quantity for restocked items
5. Mark Item Back in Stock → re-enable chicken tikka with new quantity
6. View Kitchen Inventory → verify all items properly stocked
```

### Scenario 3: Special Packaging Requirements
```
Objective: Handle customer with specific packaging needs

Steps:
1. Login Operator
2. Get Order Details → read special instructions "no cutlery, eco-friendly"
3. Accept Order → confirm order
4. Package Order without Cutlery → respect customer preference
5. Generate Handover OTP → prepare for delivery
6. Verify OTP and Complete Handover → transfer to agent
```

### Scenario 4: Breakfast-Only Items
```
Objective: Manage time-based menu availability

Steps:
1. Login Operator (at 7:00 AM)
2. View Kitchen Inventory → see breakfast items
3. Update Food Item Timings → set breakfast pizza (08:00-11:30)
4. Accept breakfast orders → process until 11:30
5. Update Food Item Availability → disable breakfast items after 11:30
6. Update Food Item Timings → enable lunch items (11:30-15:00)
```

## Performance Considerations

- **Response Times**: Most endpoints < 200ms
- **Order Queue**: Can handle 100+ pending orders
- **Inventory Updates**: Real-time (immediate visibility)
- **OTP Generation**: < 50ms
- **Concurrent Operators**: Multiple operators can work simultaneously

## Next Steps

After completing this workflow:
1. ✅ Test all 8 workflow sections individually
2. ✅ Run complete end-to-end workflow (Section 8)
3. ✅ Test error scenarios (invalid OTP, expired OTP, out of stock)
4. ✅ Test with multiple operators simultaneously
5. ✅ Integrate with Worker workflows (kitchen preparation)
6. ✅ Integrate with Delivery Agent workflows (pickup and delivery)
7. ✅ Test time-based availability (breakfast/lunch/dinner items)
8. ✅ Monitor low stock alerts and inventory management

## Related Collections

- **Restaurant Owner Workflows** - Register restaurant, add menu items
- **Customer Workflows** - Place orders, track delivery (coming soon)
- **Worker Workflows** - Kitchen preparation (coming soon)
- **Delivery Agent Workflows** - Pickup and delivery (coming soon)

## Support & Documentation

- **RBAC Documentation**: [RBAC-COMPREHENSIVE.md](../../docs/services/RBAC-COMPREHENSIVE.md)
- **API Documentation**: Available at `http://localhost:30002/swagger` (Catalog Service)
- **Authentication API**: Available at `http://localhost:30001/swagger` (Auth Service)

---

**Version**: 1.0.0  
**Last Updated**: November 20, 2025  
**Maintained By**: FDA Development Team
