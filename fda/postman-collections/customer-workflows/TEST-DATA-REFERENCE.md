# Test Data Reference - Customer Workflows

## Overview
This document provides test data for all Customer Workflows Postman collection requests. Use this data to test various scenarios without needing to create new test data.

---

## Test Users

### Customer 1 - Regular User
```json
{
  "userId": "67501234abcd1234567890ab",
  "firstName": "Rajesh",
  "lastName": "Kumar",
  "email": "rajesh.kumar@example.com",
  "phoneNumber": "+919876543210",
  "role": "Customer",
  "dateOfBirth": "1990-05-15",
  "dietaryPreferences": ["Vegetarian"],
  "createdAt": "2025-01-01T10:00:00Z"
}
```

**Usage:**
- Standard customer journey
- Vegetarian orders
- Home delivery

**Environment Variables:**
```javascript
user_id: "67501234abcd1234567890ab"
user_phone: "+919876543210"
user_email: "rajesh.kumar@example.com"
```

---

### Customer 2 - Premium User
```json
{
  "userId": "67502345bcde2345678901bc",
  "firstName": "Priya",
  "lastName": "Sharma",
  "email": "priya.sharma@example.com",
  "phoneNumber": "+919123456789",
  "role": "Customer",
  "dateOfBirth": "1992-08-22",
  "dietaryPreferences": ["Non-Vegetarian", "Spicy Food"],
  "allergies": ["Peanuts"],
  "createdAt": "2025-01-02T11:00:00Z"
}
```

**Usage:**
- Multiple addresses
- Non-veg preferences
- Allergy testing

**Environment Variables:**
```javascript
user_id: "67502345bcde2345678901bc"
user_phone: "+919123456789"
user_email: "priya.sharma@example.com"
```

---

### Customer 3 - Corporate User
```json
{
  "userId": "67503456cdef3456789012cd",
  "firstName": "Amit",
  "lastName": "Patel",
  "email": "amit.patel@company.com",
  "phoneNumber": "+918765432109",
  "role": "Customer",
  "dateOfBirth": "1988-03-10",
  "dietaryPreferences": ["Jain"],
  "organization": "Tech Corp India",
  "createdAt": "2025-01-03T09:00:00Z"
}
```

**Usage:**
- Office address orders
- Bulk orders
- Corporate preferences

---

## Test Addresses

### Home Address 1 (Residential)
```json
{
  "type": "home",
  "line1": "123, MG Road",
  "line2": "Apartment 5B",
  "landmark": "Near City Mall",
  "city": "Bangalore",
  "state": "Karnataka",
  "pincode": "560001",
  "isDefault": true
}
```

**Delivery Time:** 30-40 minutes  
**Area:** Central Bangalore  
**Coordinates:** 12.9716, 77.5946

---

### Work Address 1 (Office)
```json
{
  "type": "work",
  "line1": "456, Koramangala 4th Block",
  "line2": "8th Main Road, Office Tower A",
  "landmark": "Near Sony Signal",
  "city": "Bangalore",
  "state": "Karnataka",
  "pincode": "560034",
  "isDefault": false
}
```

**Delivery Time:** 25-35 minutes  
**Area:** Koramangala  
**Coordinates:** 12.9352, 77.6245

---

### Home Address 2 (Suburban)
```json
{
  "type": "home",
  "line1": "789, Whitefield Main Road",
  "line2": "Green Valley Apartments, Tower B, Flat 301",
  "landmark": "Opposite Forum Mall",
  "city": "Bangalore",
  "state": "Karnataka",
  "pincode": "560066",
  "isDefault": false
}
```

**Delivery Time:** 45-55 minutes  
**Area:** Whitefield (Far)  
**Coordinates:** 12.9698, 77.7499

---

### Other Address (Friend's Place)
```json
{
  "type": "other",
  "line1": "321, Indiranagar 100 Feet Road",
  "line2": "Near 100ft Road Metro Station",
  "landmark": "Above Cafe Coffee Day",
  "city": "Bangalore",
  "state": "Karnataka",
  "pincode": "560038",
  "isDefault": false
}
```

**Delivery Time:** 30-40 minutes  
**Area:** Indiranagar  
**Coordinates:** 12.9784, 77.6408

---

## Test Restaurants

### Restaurant 1 - South Indian Paradise
```json
{
  "id": "rest001",
  "name": "South Indian Paradise",
  "cuisine": "South Indian",
  "rating": 4.5,
  "deliveryTime": "30-40 min",
  "costForTwo": 400,
  "isActive": true,
  "address": {
    "line1": "15, Church Street",
    "city": "Bangalore",
    "pincode": "560001"
  }
}
```

**Popular Items:**
- Masala Dosa - ₹120
- Idli Vada - ₹80
- Filter Coffee - ₹50

---

### Restaurant 2 - North Spice Kitchen
```json
{
  "id": "rest002",
  "name": "North Spice Kitchen",
  "cuisine": "North Indian",
  "rating": 4.3,
  "deliveryTime": "35-45 min",
  "costForTwo": 600,
  "isActive": true,
  "address": {
    "line1": "45, Residency Road",
    "city": "Bangalore",
    "pincode": "560025"
  }
}
```

**Popular Items:**
- Butter Chicken - ₹280
- Paneer Tikka - ₹220
- Garlic Naan - ₹60

---

### Restaurant 3 - Chinese Wok
```json
{
  "id": "rest003",
  "name": "Chinese Wok",
  "cuisine": "Chinese",
  "rating": 4.1,
  "deliveryTime": "25-35 min",
  "costForTwo": 500,
  "isActive": true,
  "address": {
    "line1": "78, Brigade Road",
    "city": "Bangalore",
    "pincode": "560001"
  }
}
```

**Popular Items:**
- Schezwan Fried Rice - ₹180
- Chilli Chicken - ₹240
- Spring Rolls - ₹120

---

## Test Menu Items

### Vegetarian Items
```json
[
  {
    "id": "menu001",
    "name": "Masala Dosa",
    "restaurantId": "rest001",
    "category": "Main Course",
    "price": 120,
    "isVeg": true,
    "isAvailable": true,
    "preparationTime": 15,
    "description": "Crispy rice crepe with spiced potato filling"
  },
  {
    "id": "menu002",
    "name": "Paneer Butter Masala",
    "restaurantId": "rest002",
    "category": "Main Course",
    "price": 250,
    "isVeg": true,
    "isAvailable": true,
    "preparationTime": 20,
    "description": "Cottage cheese in rich tomato gravy"
  },
  {
    "id": "menu003",
    "name": "Veg Fried Rice",
    "restaurantId": "rest003",
    "category": "Main Course",
    "price": 180,
    "isVeg": true,
    "isAvailable": true,
    "preparationTime": 18,
    "description": "Mixed vegetables with aromatic rice"
  }
]
```

---

### Non-Vegetarian Items
```json
[
  {
    "id": "menu004",
    "name": "Butter Chicken",
    "restaurantId": "rest002",
    "category": "Main Course",
    "price": 280,
    "isVeg": false,
    "isAvailable": true,
    "preparationTime": 25,
    "description": "Tender chicken in creamy tomato sauce"
  },
  {
    "id": "menu005",
    "name": "Chicken Biryani",
    "restaurantId": "rest002",
    "category": "Rice",
    "price": 220,
    "isVeg": false,
    "isAvailable": true,
    "preparationTime": 30,
    "description": "Aromatic basmati rice with spiced chicken"
  },
  {
    "id": "menu006",
    "name": "Chilli Chicken",
    "restaurantId": "rest003",
    "category": "Starters",
    "price": 240,
    "isVeg": false,
    "isAvailable": true,
    "preparationTime": 20,
    "description": "Spicy Indo-Chinese chicken preparation"
  }
]
```

---

### Beverages & Desserts
```json
[
  {
    "id": "menu007",
    "name": "Filter Coffee",
    "restaurantId": "rest001",
    "category": "Beverages",
    "price": 50,
    "isVeg": true,
    "isAvailable": true,
    "preparationTime": 5,
    "description": "Traditional South Indian filter coffee"
  },
  {
    "id": "menu008",
    "name": "Gulab Jamun",
    "restaurantId": "rest002",
    "category": "Desserts",
    "price": 80,
    "isVeg": true,
    "isAvailable": true,
    "preparationTime": 2,
    "description": "Soft milk dumplings in sugar syrup"
  }
]
```

---

## Test Orders

### Order 1 - Small Vegetarian Order
```json
{
  "customerId": "67501234abcd1234567890ab",
  "restaurantId": "rest001",
  "restaurantName": "South Indian Paradise",
  "items": [
    {
      "menuItemId": "menu001",
      "name": "Masala Dosa",
      "quantity": 2,
      "price": 120,
      "specialInstructions": "Extra spicy"
    },
    {
      "menuItemId": "menu007",
      "name": "Filter Coffee",
      "quantity": 2,
      "price": 50,
      "specialInstructions": ""
    }
  ],
  "totalAmount": 340,
  "status": 0,
  "notes": "Please call before delivery",
  "deliveryAddress": {
    "line1": "123, MG Road",
    "city": "Bangalore",
    "pincode": "560001"
  }
}
```

**Expected Total Breakdown:**
- Subtotal: ₹340
- Delivery Fee: ₹40
- Platform Fee: ₹5
- GST (5%): ₹19.25
- **Final Total: ₹404.25**

---

### Order 2 - Large Mixed Order
```json
{
  "customerId": "67502345bcde2345678901bc",
  "restaurantId": "rest002",
  "restaurantName": "North Spice Kitchen",
  "items": [
    {
      "menuItemId": "menu004",
      "name": "Butter Chicken",
      "quantity": 2,
      "price": 280,
      "specialInstructions": "Medium spicy"
    },
    {
      "menuItemId": "menu005",
      "name": "Chicken Biryani",
      "quantity": 1,
      "price": 220,
      "specialInstructions": ""
    },
    {
      "menuItemId": "menu002",
      "name": "Paneer Butter Masala",
      "quantity": 1,
      "price": 250,
      "specialInstructions": ""
    },
    {
      "menuItemId": "menu008",
      "name": "Gulab Jamun",
      "quantity": 3,
      "price": 80,
      "specialInstructions": ""
    }
  ],
  "totalAmount": 1270,
  "status": 0,
  "notes": "Celebrate birthday! Please add extra dessert",
  "deliveryAddress": {
    "line1": "456, Koramangala 4th Block",
    "city": "Bangalore",
    "pincode": "560034"
  }
}
```

**Expected Total Breakdown:**
- Subtotal: ₹1270
- Delivery Fee: ₹40
- Platform Fee: ₹5
- GST (5%): ₹65.75
- **Final Total: ₹1380.75**

---

### Order 3 - Quick Lunch Order
```json
{
  "customerId": "67503456cdef3456789012cd",
  "restaurantId": "rest003",
  "restaurantName": "Chinese Wok",
  "items": [
    {
      "menuItemId": "menu003",
      "name": "Veg Fried Rice",
      "quantity": 1,
      "price": 180,
      "specialInstructions": "No onions"
    }
  ],
  "totalAmount": 180,
  "status": 0,
  "notes": "Quick lunch break order",
  "deliveryAddress": {
    "line1": "456, Koramangala 4th Block",
    "city": "Bangalore",
    "pincode": "560034"
  }
}
```

**Expected Total Breakdown:**
- Subtotal: ₹180
- Delivery Fee: ₹40
- Platform Fee: ₹5
- GST (5%): ₹11.25
- **Final Total: ₹236.25**

---

## Food Preferences Test Data

### Vegetarian User
```json
{
  "dietaryRestrictions": ["Vegetarian", "Eggless"],
  "favoriteCategories": ["South Indian", "North Indian", "Italian"],
  "dislikedIngredients": ["Mushroom"],
  "spiceLevel": "Medium",
  "allergies": []
}
```

---

### Vegan User
```json
{
  "dietaryRestrictions": ["Vegan", "Dairy-Free"],
  "favoriteCategories": ["Continental", "Chinese"],
  "dislikedIngredients": ["Tofu", "Soy"],
  "spiceLevel": "Mild",
  "allergies": []
}
```

---

### Allergy-Sensitive User
```json
{
  "dietaryRestrictions": ["Gluten-Free"],
  "favoriteCategories": ["Mediterranean", "Thai"],
  "dislikedIngredients": [],
  "spiceLevel": "Hot",
  "allergies": ["Peanuts", "Tree Nuts", "Shellfish"]
}
```

---

### Jain User
```json
{
  "dietaryRestrictions": ["Jain", "No Onion", "No Garlic", "No Root Vegetables"],
  "favoriteCategories": ["Pure Veg", "Jain Special"],
  "dislikedIngredients": ["Potato", "Carrot"],
  "spiceLevel": "Mild",
  "allergies": []
}
```

---

## Order Status Test Scenarios

### Scenario 1: Successful Delivery
```
Timeline:
00:00 - Order Placed (Status: 0 - Pending)
00:02 - Restaurant Accepts (Status: 1 - Accepted)
00:05 - Food Preparation Starts (Status: 3 - Preparing)
00:25 - Food Ready (Status: 4 - Ready for Pickup)
00:30 - Delivery Agent Assigned (Status: 5 - Out for Delivery)
00:55 - Order Delivered (Status: 6 - Delivered)
```

---

### Scenario 2: Order Cancellation
```
Timeline:
00:00 - Order Placed (Status: 0 - Pending)
00:01 - Customer Cancels (Status: 7 - Cancelled)
Reason: "Changed mind"
```

---

### Scenario 3: Restaurant Decline
```
Timeline:
00:00 - Order Placed (Status: 0 - Pending)
00:03 - Restaurant Declines (Status: 2 - Declined)
Reason: "Item out of stock"
```

---

## Error Scenarios

### Invalid Order - Missing Required Fields
```json
{
  "customerId": "67501234abcd1234567890ab",
  "items": []
}
```
**Expected Error:** `400 Bad Request - Items array cannot be empty`

---

### Invalid Address - Missing Pincode
```json
{
  "type": "home",
  "line1": "123, MG Road",
  "city": "Bangalore",
  "state": "Karnataka"
}
```
**Expected Error:** `400 Bad Request - Pincode is required`

---

### Duplicate Address
```json
{
  "type": "home",
  "line1": "123, MG Road",
  "pincode": "560001"
}
```
**Expected Error:** `409 Conflict - Address already exists`

---

### Unauthorized Access
```
Request without auth_token
```
**Expected Error:** `401 Unauthorized - Authentication required`

---

### Order Not Found
```
GET /api/orders/INVALID_ORDER_ID
```
**Expected Error:** `404 Not Found - Order not found`

---

## Performance Test Data

### High-Volume Order (Stress Test)
```json
{
  "customerId": "67501234abcd1234567890ab",
  "restaurantId": "rest002",
  "items": [
    // 20 different items
    {"menuItemId": "menu001", "quantity": 5},
    {"menuItemId": "menu002", "quantity": 3},
    // ... 18 more items
  ],
  "totalAmount": 5000
}
```

---

### Concurrent Orders (Load Test)
- Place 100 orders simultaneously
- From 50 different customers
- To 10 different restaurants
- Monitor response times and success rate

---

## Sample cURL Commands

### Create Order
```bash
curl -X POST http://localhost:30500/api/orders \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "67501234abcd1234567890ab",
    "restaurantId": "rest001",
    "items": [
      {
        "menuItemId": "menu001",
        "name": "Masala Dosa",
        "quantity": 2,
        "price": 120
      }
    ],
    "totalAmount": 240
  }'
```

---

### Add Address
```bash
curl -X POST http://localhost:30500/api/crm/userprofile/by-user/67501234abcd1234567890ab/addresses \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "type": "home",
    "line1": "123, MG Road",
    "city": "Bangalore",
    "pincode": "560001",
    "isDefault": true
  }'
```

---

### View Order History
```bash
curl -X GET http://localhost:30500/api/orders/customer/67501234abcd1234567890ab \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Notes

1. **Dynamic IDs**: Replace placeholder IDs with actual IDs from your database
2. **Timestamps**: Use `{{$timestamp}}` in Postman for current timestamp
3. **Random Data**: Use Postman's `{{$randomInt}}`, `{{$randomEmail}}`, etc.
4. **Environment**: Always use environment variables for IDs and tokens
5. **Data Cleanup**: Delete test orders/addresses after testing

---

## Version History

- **v1.0.0** (Dec 4, 2025) - Initial test data reference
