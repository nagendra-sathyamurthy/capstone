# Operator Service Test Data Reference

Complete test data for operator workflow testing, including sample accounts, orders, and inventory configurations.

## Test Operator Accounts

### Operator 1 - Pizza Palace
```json
{
  "email": "operator.pizzapalace@example.com",
  "password": "Operator123!",
  "role": "Operator",
  "organization": "pizzapalace",
  "restaurantName": "Pizza Palace Downtown",
  "restaurantId": "6738a123456789abcdef0001",
  "employeeId": "OP-PP-001",
  "position": "Lead Operator",
  "department": "Operations"
}
```

### Operator 2 - Sushi Spot
```json
{
  "email": "operator.sushispot@example.com",
  "password": "Operator123!",
  "role": "Operator",
  "organization": "sushispot",
  "restaurantName": "Sushi Spot",
  "restaurantId": "6738a123456789abcdef0002",
  "employeeId": "OP-SS-001",
  "position": "Senior Operator",
  "department": "Customer Service"
}
```

### Operator 3 - Burger Hub
```json
{
  "email": "operator.burgerhub@example.com",
  "password": "Operator123!",
  "role": "Operator",
  "organization": "burgerhub",
  "restaurantName": "Burger Hub",
  "restaurantId": "6738a123456789abcdef0003",
  "employeeId": "OP-BH-001",
  "position": "Operator",
  "department": "Operations"
}
```

## Sample Orders for Testing

### Order 1 - Standard Lunch Order (Pending)
```json
{
  "id": "ORDER-001-20251120",
  "customerId": "CUST-12345",
  "restaurantId": "6738a123456789abcdef0001",
  "items": [
    {
      "menuItemId": "ITEM-PP-MARGHERITA",
      "name": "Pizza Margherita (Large)",
      "quantity": 2,
      "price": 12.99,
      "specialInstructions": "Extra cheese, well done"
    },
    {
      "menuItemId": "ITEM-PP-GARLIC-BREAD",
      "name": "Garlic Bread",
      "quantity": 1,
      "price": 5.99,
      "specialInstructions": null
    }
  ],
  "totalAmount": 31.97,
  "status": "Pending",
  "createdAt": "2025-11-20T11:15:00Z"
}
```

### Order 2 - Breakfast Order (Time-Sensitive)
```json
{
  "id": "ORDER-002-20251120",
  "customerId": "CUST-67890",
  "restaurantId": "6738a123456789abcdef0001",
  "items": [
    {
      "menuItemId": "ITEM-PP-BREAKFAST-PIZZA",
      "name": "Breakfast Pizza",
      "quantity": 1,
      "price": 9.99,
      "specialInstructions": "No onions"
    },
    {
      "menuItemId": "ITEM-PP-COFFEE",
      "name": "Cappuccino",
      "quantity": 2,
      "price": 3.50,
      "specialInstructions": null
    }
  ],
  "totalAmount": 16.99,
  "status": "Pending",
  "createdAt": "2025-11-20T08:45:00Z"
}
```

### Order 3 - Large Party Order (Accepted, Ready for Packaging)
```json
{
  "id": "ORDER-003-20251120",
  "customerId": "CUST-11223",
  "restaurantId": "6738a123456789abcdef0001",
  "items": [
    {
      "menuItemId": "ITEM-PP-PEPPERONI",
      "name": "Pepperoni Pizza (Family Size)",
      "quantity": 3,
      "price": 18.99,
      "specialInstructions": null
    },
    {
      "menuItemId": "ITEM-PP-VEGGIE",
      "name": "Veggie Supreme Pizza (Large)",
      "quantity": 2,
      "price": 15.99,
      "specialInstructions": "Extra veggies"
    },
    {
      "menuItemId": "ITEM-PP-WINGS",
      "name": "Buffalo Wings (24 pcs)",
      "quantity": 2,
      "price": 12.99,
      "specialInstructions": "Extra ranch dressing"
    }
  ],
  "totalAmount": 105.91,
  "status": "Accepted",
  "createdAt": "2025-11-20T10:30:00Z",
  "acceptedAt": "2025-11-20T10:32:00Z"
}
```

### Order 4 - Fragile Order (Requires Special Packaging)
```json
{
  "id": "ORDER-004-20251120",
  "customerId": "CUST-44556",
  "restaurantId": "6738a123456789abcdef0001",
  "items": [
    {
      "menuItemId": "ITEM-PP-SOUP",
      "name": "Tomato Basil Soup (Large)",
      "quantity": 2,
      "price": 6.99,
      "specialInstructions": "Very hot please"
    },
    {
      "menuItemId": "ITEM-PP-TIRAMISU",
      "name": "Tiramisu Dessert",
      "quantity": 1,
      "price": 7.99,
      "specialInstructions": "Handle with care"
    }
  ],
  "totalAmount": 21.97,
  "status": "Accepted",
  "isFragile": true,
  "createdAt": "2025-11-20T12:00:00Z"
}
```

### Order 5 - Eco-Friendly Order (No Cutlery)
```json
{
  "id": "ORDER-005-20251120",
  "customerId": "CUST-78901",
  "restaurantId": "6738a123456789abcdef0001",
  "items": [
    {
      "menuItemId": "ITEM-PP-MARGHERITA",
      "name": "Pizza Margherita (Medium)",
      "quantity": 1,
      "price": 10.99,
      "specialInstructions": "No cutlery needed, I have my own"
    }
  ],
  "totalAmount": 10.99,
  "status": "Accepted",
  "customerPreferences": {
    "noCutlery": true,
    "ecoFriendly": true
  },
  "createdAt": "2025-11-20T13:15:00Z"
}
```

## Sample Menu Items (Inventory)

### Item 1 - Pizza Margherita (All-Day Item)
```json
{
  "id": "ITEM-PP-MARGHERITA",
  "restaurantId": "6738a123456789abcdef0001",
  "name": "Pizza Margherita",
  "description": "Classic pizza with tomato sauce, mozzarella, and basil",
  "category": "Main Course",
  "cuisineType": "Italian",
  "price": 12.99,
  "isAvailable": true,
  "quantityAvailable": 45,
  "minimumQuantity": 10,
  "availableFromTime": null,
  "availableToTime": null,
  "preparationTimeMinutes": 15,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false
}
```

### Item 2 - Breakfast Pizza (Time-Limited)
```json
{
  "id": "ITEM-PP-BREAKFAST-PIZZA",
  "restaurantId": "6738a123456789abcdef0001",
  "name": "Breakfast Pizza",
  "description": "Eggs, bacon, cheese on pizza base",
  "category": "Breakfast",
  "cuisineType": "American",
  "price": 9.99,
  "isAvailable": true,
  "quantityAvailable": 12,
  "minimumQuantity": 5,
  "availableFromTime": "08:00",
  "availableToTime": "11:30",
  "preparationTimeMinutes": 12,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false
}
```

### Item 3 - Tomato Basil Soup (Low Stock)
```json
{
  "id": "ITEM-PP-SOUP",
  "restaurantId": "6738a123456789abcdef0001",
  "name": "Tomato Basil Soup",
  "description": "Creamy tomato soup with fresh basil",
  "category": "Appetizer",
  "cuisineType": "Italian",
  "price": 6.99,
  "isAvailable": true,
  "quantityAvailable": 4,
  "minimumQuantity": 5,
  "availableFromTime": null,
  "availableToTime": null,
  "preparationTimeMinutes": 5,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": true
}
```

### Item 4 - Pepperoni Pizza (Out of Stock)
```json
{
  "id": "ITEM-PP-PEPPERONI",
  "restaurantId": "6738a123456789abcdef0001",
  "name": "Pepperoni Pizza",
  "description": "Pizza with pepperoni, cheese, and tomato sauce",
  "category": "Main Course",
  "cuisineType": "Italian",
  "price": 14.99,
  "isAvailable": false,
  "quantityAvailable": 0,
  "minimumQuantity": 10,
  "availableFromTime": null,
  "availableToTime": null,
  "preparationTimeMinutes": 15,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false
}
```

### Item 5 - Garlic Bread (Critical Low Stock)
```json
{
  "id": "ITEM-PP-GARLIC-BREAD",
  "restaurantId": "6738a123456789abcdef0001",
  "name": "Garlic Bread",
  "description": "Fresh baked bread with garlic butter",
  "category": "Appetizer",
  "cuisineType": "Italian",
  "price": 5.99,
  "isAvailable": true,
  "quantityAvailable": 3,
  "minimumQuantity": 8,
  "availableFromTime": null,
  "availableToTime": null,
  "preparationTimeMinutes": 8,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false
}
```

## Sample Delivery Agents

### Agent 1 - John Smith (Bike)
```json
{
  "id": "674abc123def456789012345",
  "email": "john.delivery@example.com",
  "role": "DeliveryAgent",
  "organization": "fda_delivery_network",
  "employeeId": "DA-001",
  "vehicleType": "Bike",
  "licensePlate": "BIKE-123",
  "averageRating": 4.8,
  "isAvailable": true
}
```

### Agent 2 - Sarah Johnson (Scooter)
```json
{
  "id": "674abc123def456789012346",
  "email": "sarah.delivery@example.com",
  "role": "DeliveryAgent",
  "organization": "fda_delivery_network",
  "employeeId": "DA-002",
  "vehicleType": "Scooter",
  "licensePlate": "SC-456",
  "averageRating": 4.9,
  "isAvailable": true
}
```

## Test Scenarios with Data

### Scenario 1: Accept Order and Update Inventory
**Initial State**:
- Order: ORDER-001-20251120 (Pending, 2x Margherita Pizza)
- Inventory: ITEM-PP-MARGHERITA (45 available)

**Steps**:
1. Get Pending Orders → finds ORDER-001
2. Accept Order → status changes to Accepted
3. Update Inventory → decrease quantity to 43 (45 - 2)

**Expected Result**:
- Order status: Accepted
- Inventory: 43 available
- Low stock alert: No (still above minimum of 10)

### Scenario 2: Low Stock Alert and Out of Stock
**Initial State**:
- Inventory: ITEM-PP-SOUP (4 available, minimum 5)

**Steps**:
1. Get Low Stock Items → returns ITEM-PP-SOUP
2. Accept order with 2x soup → reduces to 2 available
3. Mark Item Out of Stock → prevent further orders

**Expected Result**:
- Low stock alert triggered
- Item marked unavailable
- Customers cannot order soup

### Scenario 3: Time-Based Availability (Breakfast)
**Initial State**:
- Current Time: 08:30 AM
- Item: ITEM-PP-BREAKFAST-PIZZA (available 08:00-11:30)

**Steps**:
1. View Kitchen Inventory → shows breakfast pizza available
2. Accept breakfast order → order processed successfully
3. Wait until 11:31 AM
4. Update Food Item Timings → disable breakfast items

**Expected Result**:
- Breakfast items available during window
- Automatically disabled after 11:30
- Lunch items enabled

### Scenario 4: Fragile Order Packaging
**Initial State**:
- Order: ORDER-004-20251120 (2x Soup, 1x Tiramisu)

**Steps**:
1. Get Order Details → note fragile items
2. Accept Order → confirm order
3. Package Fragile Order → isFragile: true, extra bags

**Expected Result**:
- Packaging marked as fragile
- Delivery agent receives special handling instructions
- Order packaged safely

### Scenario 5: OTP Handover
**Initial State**:
- Order: ORDER-003-20251120 (Accepted, packaged)
- Agent: John Smith (DA-001)

**Steps**:
1. Generate Handover OTP → OTP: 847593, expires in 5 min
2. Delivery agent arrives → operator shows OTP
3. Verify OTP and Complete Handover → validates OTP
4. Order Status → changes to OutForDelivery

**Expected Result**:
- OTP validated successfully
- Order handed over to John Smith
- Status: OutForDelivery
- Handover timestamp recorded

### Scenario 6: Invalid OTP (Security Test)
**Initial State**:
- Order: ORDER-003-20251120
- Valid OTP: 847593

**Steps**:
1. Attempt handover with OTP: 999999 → rejected
2. Attempt handover with expired OTP → rejected
3. Attempt handover with wrong agent ID → rejected

**Expected Result**:
- All invalid attempts rejected with 400 error
- Order remains in current status
- Security audit log recorded

## Postman Collection Variables Setup

### Required Variables
```json
{
  "auth_token": "",              // Auto-set on login
  "operator_id": "",             // Auto-set on login
  "restaurant_id": "6738a123456789abcdef0001",  // Set manually
  "order_id": "",                // Auto-set from pending orders
  "menu_item_id": "",            // Auto-set from inventory
  "delivery_agent_id": "674abc123def456789012345",  // Set manually
  "handover_otp": ""             // Auto-set from generate OTP
}
```

### Environment-Specific URLs
```json
{
  "auth_service_url": "http://localhost:30001",
  "catalog_service_url": "http://localhost:30002",
  "base_url": "{{catalog_service_url}}"
}
```

## Database Seeding (MongoDB)

### Seed Script for Test Data
```javascript
// MongoDB seed script
use CatalogDb;

// Insert test orders
db.Orders.insertMany([
  // Order 1 - Pending
  {
    _id: ObjectId("6738a123456789abcdef1001"),
    CustomerId: "CUST-12345",
    RestaurantId: "6738a123456789abcdef0001",
    Items: [
      { MenuItemId: "ITEM-PP-MARGHERITA", Name: "Pizza Margherita (Large)", Quantity: 2, Price: 12.99 },
      { MenuItemId: "ITEM-PP-GARLIC-BREAD", Name: "Garlic Bread", Quantity: 1, Price: 5.99 }
    ],
    TotalAmount: 31.97,
    Status: 0, // Pending
    CreatedAt: new Date("2025-11-20T11:15:00Z")
  },
  // Order 2 - Accepted
  {
    _id: ObjectId("6738a123456789abcdef1003"),
    CustomerId: "CUST-11223",
    RestaurantId: "6738a123456789abcdef0001",
    Items: [
      { MenuItemId: "ITEM-PP-PEPPERONI", Name: "Pepperoni Pizza (Family Size)", Quantity: 3, Price: 18.99 }
    ],
    TotalAmount: 56.97,
    Status: 1, // Accepted
    CreatedAt: new Date("2025-11-20T10:30:00Z"),
    UpdatedAt: new Date("2025-11-20T10:32:00Z")
  }
]);

// Insert test menu items
db.MenuItems.insertMany([
  // Margherita - Normal stock
  {
    _id: ObjectId("ITEM-PP-MARGHERITA"),
    RestaurantId: "6738a123456789abcdef0001",
    Name: "Pizza Margherita",
    Price: 12.99,
    IsAvailable: true,
    QuantityAvailable: 45,
    MinimumQuantity: 10
  },
  // Soup - Low stock
  {
    _id: ObjectId("ITEM-PP-SOUP"),
    RestaurantId: "6738a123456789abcdef0001",
    Name: "Tomato Basil Soup",
    Price: 6.99,
    IsAvailable: true,
    QuantityAvailable: 4,
    MinimumQuantity: 5
  },
  // Pepperoni - Out of stock
  {
    _id: ObjectId("ITEM-PP-PEPPERONI"),
    RestaurantId: "6738a123456789abcdef0001",
    Name: "Pepperoni Pizza",
    Price: 14.99,
    IsAvailable: false,
    QuantityAvailable: 0,
    MinimumQuantity: 10
  }
]);
```

## Testing Checklist

### Pre-Test Setup
- [ ] Authentication Service running (port 30001)
- [ ] Catalog Service running (port 30002)
- [ ] MongoDB running with test data
- [ ] Postman collection imported
- [ ] Collection variables configured
- [ ] Test operator account created

### Order Management Tests
- [ ] Login operator successfully
- [ ] View pending orders (at least 1 order)
- [ ] View orders ready for pickup
- [ ] Get specific order details
- [ ] Accept order (status change verified)
- [ ] Decline order (status change verified)

### Inventory Management Tests
- [ ] View complete kitchen inventory
- [ ] Get low stock items (verify threshold logic)
- [ ] Update food item quantity
- [ ] Update food item availability (toggle on/off)
- [ ] Update food item timings (time windows)
- [ ] Mark item out of stock
- [ ] Mark item back in stock

### Packaging Tests
- [ ] Package order with cutlery
- [ ] Package order without cutlery
- [ ] Package fragile order
- [ ] Verify packaging timestamp
- [ ] Verify operator ID recorded

### Handover Tests
- [ ] Generate handover OTP (6 digits)
- [ ] Verify OTP saved in variable
- [ ] Complete handover with valid OTP
- [ ] Attempt handover with invalid OTP (should fail)
- [ ] Verify order status changed to OutForDelivery
- [ ] Verify handover timestamp recorded

### Complete Workflow Test
- [ ] Run complete workflow (8 steps)
- [ ] Verify each step completes successfully
- [ ] Check final order status
- [ ] Verify inventory updated correctly

---

**Version**: 1.0.0  
**Last Updated**: November 20, 2025  
**Test Data Maintained By**: FDA QA Team
