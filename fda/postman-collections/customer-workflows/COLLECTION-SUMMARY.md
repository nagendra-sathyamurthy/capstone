# Customer Workflows Collection - Summary

## What's New (December 4, 2025)

A comprehensive Postman collection has been added to test all customer-facing workflows in the Food Delivery Application. This collection covers the complete customer journey from profile setup to order tracking.

---

## Collection Details

### Location
```
fda/postman-collections/customer-workflows/
├── Customer-Workflows.postman_collection.json
├── README.md
└── TEST-DATA-REFERENCE.md
```

### Collection Structure

The collection is organized into 5 main folders:

#### 1. User Profile Management (4 requests)
- Get User Profile
- Update User Profile  
- Update Profile Image
- Update Food Preferences

#### 2. Address Management (4 requests)
- Get All Addresses
- Add New Address
- Update Address
- Delete Address

#### 3. Complete Order Flow (5 requests)
- Step 1: Browse Restaurants
- Step 2: View Restaurant Menu
- Step 3: Add Item to Cart
- Step 4: View Cart
- Step 5: Place Order

#### 4. Order History & Tracking (4 requests)
- View Order History
- Get Order Details
- Track Order Status
- Cancel Order

**Total: 17 Requests**

---

## Key Features

### 1. Complete Customer Journey
The collection covers the entire flow from registration to order delivery:
```
Register → Setup Profile → Add Addresses → Browse Restaurants → 
Add to Cart → Checkout → Place Order → Track Order → View History
```

### 2. Automated Variable Management
- Automatically saves IDs from responses
- Cascading variables through workflow
- No manual ID copying needed

### 3. Test Scripts
Every request includes:
- Status code validation
- Response structure validation
- Automatic environment variable updates
- Descriptive test assertions

### 4. Comprehensive Documentation
- Detailed README with usage instructions
- Complete test data reference
- Error handling guide
- Integration examples

---

## API Endpoints Covered

### CRM Service (Profile & Address)
```
GET    /api/crm/userprofile/by-user/{userId}
PUT    /api/crm/userprofile/by-user/{userId}
PUT    /api/crm/userprofile/by-user/{userId}/profile-image
PUT    /api/crm/userprofile/by-user/{userId}/food-preferences
GET    /api/crm/userprofile/by-user/{userId}/addresses
POST   /api/crm/userprofile/by-user/{userId}/addresses
PUT    /api/crm/userprofile/by-user/{userId}/addresses/{addressId}
DELETE /api/crm/userprofile/by-user/{userId}/addresses/{addressId}
```

### Catalog Service (Browse)
```
GET    /api/catalog/restaurants
GET    /api/catalog/menu/restaurant/{restaurantId}
```

### Cart Service
```
GET    /api/cart/{userId}
POST   /api/cart/{userId}/add
```

### Order Service
```
POST   /api/orders
GET    /api/orders/{orderId}
GET    /api/orders/customer/{customerId}
POST   /api/orders/{orderId}/cancel
```

---

## Test Data Provided

### Customer Profiles (3)
1. **Rajesh Kumar** - Regular vegetarian customer
2. **Priya Sharma** - Premium non-veg customer with allergies
3. **Amit Patel** - Corporate Jain food customer

### Sample Addresses (4)
1. Home - Central Bangalore (MG Road)
2. Work - Koramangala Office
3. Home - Suburban Whitefield
4. Other - Indiranagar Friend's Place

### Sample Restaurants (3)
1. **South Indian Paradise** - Dosas, Idlis (₹400 for two)
2. **North Spice Kitchen** - North Indian (₹600 for two)
3. **Chinese Wok** - Chinese cuisine (₹500 for two)

### Sample Orders (3)
1. Small vegetarian order (₹404.25)
2. Large mixed order for celebration (₹1380.75)
3. Quick lunch order (₹236.25)

---

## Usage Instructions

### Prerequisites
1. Import collection and environment
2. Obtain auth token from User Registration collection
3. Set environment variables:
   ```
   gateway_base_url: http://localhost:30500
   auth_token: <your_jwt_token>
   user_id: <your_user_id>
   ```

### Running the Collection

#### Option 1: Individual Requests
Run requests one by one in Postman to test specific features.

#### Option 2: Folder Run
Run entire folders in Collection Runner:
- Profile Management folder → Test profile updates
- Complete Order Flow folder → Test end-to-end ordering

#### Option 3: Newman CLI
```bash
newman run customer-workflows/Customer-Workflows.postman_collection.json \
  -e Capstone-Local-Environment.postman_environment.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results/customer-workflows.html
```

---

## Test Scenarios Supported

### Scenario 1: New User Setup
```
1. Register/Login
2. Update Profile (name, email, preferences)
3. Add Home Address
4. Add Work Address
5. Update Food Preferences
```

### Scenario 2: Place Order
```
1. Browse Restaurants
2. View Menu
3. Add Items to Cart
4. Place Order with Address
5. Track Order
```

### Scenario 3: Order History Review
```
1. View Order History
2. Get Details of Past Order
3. Track Active Order Status
```

### Scenario 4: Address Management
```
1. Get All Addresses
2. Add New Work Address
3. Update Home Address (set as default)
4. Delete Old Address
```

### Scenario 5: Reorder from History
```
1. View Order History
2. Find Previous Order
3. Browse Same Restaurant
4. Add Same Items to Cart
5. Place New Order
```

---

## Integration with Existing Collections

### Use with User Registration Collection
1. Run User Registration → Verify OTP
2. Receive `auth_token` and `user_id`
3. Set in environment variables
4. Run Customer Workflows

### Use with Restaurant Owner Collection
- Customer places order
- Restaurant receives order notification
- Restaurant accepts/declines order
- Complete business flow testing

### Use with Operator Service Collection
- Customer order → Restaurant accepts
- Operator prepares food
- Order ready for pickup
- Complete kitchen workflow

---

## RabbitMQ Events Integration

The collection triggers important RabbitMQ events:

### OrderCreatedEvent
**Triggered by:** Place Order (Step 5)
```json
{
  "orderId": "ORD1733368800000",
  "customerId": "67501234abcd1234567890ab",
  "restaurantId": "rest001",
  "totalAmount": 404.25,
  "items": [...]
}
```
**Routing Key:** `order.created`  
**Consumers:** Restaurant service, Notification service

---

## Testing Best Practices

### 1. Order of Execution
Always run in this sequence:
```
Authentication → Profile Setup → Address Setup → Order Flow
```

### 2. Environment Variables
Let the collection manage variables automatically:
- `restaurant_id` → Set by Browse Restaurants
- `menu_item_id` → Set by View Menu
- `order_id` → Set by Place Order

### 3. Data Validation
Check test results after each request:
- ✅ Green = Success
- ❌ Red = Failure (check console for details)

### 4. Cleanup
After testing, clean up test data:
```javascript
// Reset environment variables
pm.environment.unset('order_id');
pm.environment.unset('restaurant_id');
pm.environment.unset('address_id');
```

---

## Common Issues & Solutions

### Issue 1: 401 Unauthorized
**Cause:** Expired or missing auth token  
**Solution:** Re-login using User Registration collection

### Issue 2: Order Not Found
**Cause:** Invalid order_id or order belongs to different user  
**Solution:** Run "View Order History" to get valid order IDs

### Issue 3: Address Not Found
**Cause:** Address was deleted or never created  
**Solution:** Run "Get All Addresses" to verify address IDs

### Issue 4: Cart Empty
**Cause:** Cart was cleared or is empty  
**Solution:** Add items to cart again before ordering

---

## Performance Metrics

Expected response times (local environment):

| Request Type | Expected Time |
|-------------|---------------|
| Get Profile/Addresses | < 100ms |
| Browse Restaurants | < 200ms |
| Add to Cart | < 150ms |
| Place Order | < 300ms |

---

## Documentation Files

### README.md (Comprehensive Guide)
- Collection structure
- Prerequisites and setup
- Usage instructions
- API endpoints reference
- Common scenarios
- Error handling
- Testing tips
- Integration examples

### TEST-DATA-REFERENCE.md (Test Data)
- 3 Test customer profiles
- 4 Sample delivery addresses
- 3 Sample restaurants with menus
- Sample orders with breakdowns
- Food preferences examples
- Error scenarios
- Performance test data
- cURL command examples

---

## Success Metrics

After running the collection successfully, you should see:

### ✅ Profile Management
- User profile created/updated
- Profile image uploaded
- Food preferences saved
- All data persisted in CRM service

### ✅ Address Management
- Multiple addresses added
- Default address set
- Address updates successful
- Old addresses deleted

### ✅ Order Placement
- Restaurant browsed
- Menu items viewed
- Items added to cart
- Order placed successfully

### ✅ Order Tracking
- Order history visible
- Order details accessible
- Status updates tracked
- Cancellation working (when allowed)

### ✅ RabbitMQ Integration
- OrderCreatedEvent published
- Events logged in service consoles

---

## Next Steps

### For Developers
1. Import the collection into Postman
2. Set up environment variables
3. Run through Complete Order Flow
4. Check RabbitMQ management UI for events
5. Verify database changes in MongoDB

### For Testers
1. Review TEST-DATA-REFERENCE.md
2. Create test scenarios using provided data
3. Run automated tests with Newman
4. Generate HTML reports
5. Document any bugs found

### For Integration
1. Use with Restaurant Owner collection for end-to-end flow
2. Use with Operator collection for kitchen operations
3. Run all collections together for full system test
4. Monitor RabbitMQ event flow across services

---

## Support & Resources

### Documentation
- [Customer Workflows README](./customer-workflows/README.md)
- [Test Data Reference](./customer-workflows/TEST-DATA-REFERENCE.md)
- [Main Testing Guide](./TESTING-README.md)
- [API Endpoint Reference](../API-ENDPOINT-REFERENCE.md)

### Troubleshooting
- Check service logs: `docker logs capstone-<service>`
- Verify RabbitMQ: http://localhost:15672
- Check MongoDB: Use MongoDB Compass
- Review environment variables in Postman

### Related Collections
- User Registration Flow - Authentication
- Restaurant Owner Workflows - Restaurant management
- Operator Service Workflows - Kitchen operations

---

## Version History

### v1.0.0 (December 4, 2025)
- Initial release
- 21 requests across 5 workflow categories
- Complete customer journey coverage
- Comprehensive documentation
- Sample test data included
- RabbitMQ event integration
- Auto-variable management
- Test scripts for all requests

---

## Contributing

To add more test scenarios:

1. Add request to appropriate folder
2. Add test scripts for validation
3. Update TEST-DATA-REFERENCE.md with new data
4. Document in README.md
5. Add to Quick-Test.ps1 script (if applicable)
6. Test with Newman before committing

---

## License

Part of the Food Delivery Application (FDA) Capstone Project
