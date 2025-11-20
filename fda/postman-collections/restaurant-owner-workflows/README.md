# Restaurant Owner Workflows - Postman Collection

Comprehensive workflow collection for restaurant owners to manage their restaurants and menu items in the food delivery system.

## Overview

This collection covers all essential restaurant owner operations:
1. **Authentication** - Login as restaurant owner
2. **Restaurant Registration** - Register new restaurant with complete details
3. **Menu Management** - Add, modify, view, and remove menu items
4. **Restaurant Management** - Update restaurant status, contact info, address, and business hours

## Prerequisites

- **Authentication Service** running on `http://localhost:30001`
- **Catalog Service** running on `http://localhost:30002`
- Valid restaurant owner account (role: 1)

## Test Account

Use one of the pre-registered restaurant owner accounts:
- **Email**: `pizzapalace@example.com`
- **Password**: `Password123!`

Other available accounts:
- sushispot@example.com
- burger.hub@example.com
- taco.town@example.com
- indian.cuisine@example.com
- italian.bistro@example.com
- chinese.dragon@example.com
- thai.delight@example.com

## Collection Structure

### 1. Authentication
- **Login Restaurant Owner** - Authenticates and stores JWT token

### 2. Restaurant Registration
- **Register New Restaurant** - Creates restaurant with:
  - Basic information (name, description, cuisine type)
  - Address (street, city, state, zipcode, landmark)
  - Contact information (phone, email, website)
  - Business hours (daily timings)
  - Status (active/inactive)
- **Get My Restaurants** - Retrieves all restaurants owned by logged-in user

### 3. Menu Management - Add Items
- **Add Single Menu Item** - Creates individual menu item with:
  - Restaurant association
  - Food details (name, description, category, cuisine)
  - Pricing and portions
  - Dietary information (vegetarian, vegan, gluten-free)
  - Availability timings (optional - for time-specific items)
  - Nutritional information
- **Add Multiple Menu Items (Bulk)** - Creates multiple items at once

**Time-Based Availability Example**:
```json
{
  "name": "Breakfast Pizza",
  "availableFromTime": "08:00",
  "availableToTime": "11:30"
}
```
This item will only be available between 8:00 AM and 11:30 AM.

### 4. Menu Management - Modify Items
- **Update Menu Item** - Modifies existing item (price, description, etc.)
- **Update Item Availability** - Toggle item availability on/off
  - Make unavailable (temporarily out of stock)
  - Make available (back in stock)

### 5. Menu Management - View & Remove
- **Get All My Menu Items** - Lists all items owned by restaurant owner
- **Get Restaurant Menu Items** - Lists items for specific restaurant
- **Delete Menu Item** - Removes item from menu

### 6. Restaurant Management - Update Details
- **Update Restaurant Status** - Activate/deactivate restaurant
  - Make inactive (temporarily closed)
  - Make active (open for business)
- **Update Contact Information** - Modify phone, email, website
- **Update Address** - Change restaurant location
- **Update Business Hours** - Modify daily opening/closing times
- **Get Restaurant Details** - View current restaurant information

## Collection Variables

The collection uses these variables (automatically set during workflow):
- `auth_token` - JWT authentication token
- `user_id` - Restaurant owner's user account ID
- `restaurant_id` - Created restaurant ID
- `menu_item_id` - Created menu item ID

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
   - Import `Restaurant-Owner-Workflows.postman_collection.json`
   - Import `Capstone-Local-Environment.postman_environment.json` (if not already imported)

3. **Run Collection**
   - Execute requests in order (1 → 6)
   - Each request uses variables from previous steps
   - Review test assertions to verify success

### Individual Workflow Examples

**Workflow 1: Register Restaurant and Add Menu**
1. Login Restaurant Owner
2. Register New Restaurant
3. Add Single Menu Item (or bulk)
4. Get Restaurant Menu Items (verify)

**Workflow 2: Update Menu Items**
1. Login Restaurant Owner
2. Get All My Menu Items
3. Update Menu Item (modify existing)
4. Update Item Availability (toggle on/off)

**Workflow 3: Manage Restaurant Status**
1. Login Restaurant Owner
2. Update Restaurant Status (make inactive)
3. Update Restaurant Status (make active)
4. Get Restaurant Details (verify)

**Workflow 4: Update Restaurant Information**
1. Login Restaurant Owner
2. Update Contact Information
3. Update Address
4. Update Business Hours
5. Get Restaurant Details (verify all updates)

## API Endpoints Reference

### Restaurant Endpoints
- `POST /api/restaurant/register` - Register new restaurant
- `GET /api/restaurant/owner/{ownerId}` - Get restaurants by owner
- `GET /api/restaurant/{id}` - Get restaurant details
- `PUT /api/restaurant/{id}` - Update restaurant
- `PATCH /api/restaurant/{id}/status` - Update active status
- `PATCH /api/restaurant/{id}/contact` - Update contact info
- `PATCH /api/restaurant/{id}/address` - Update address
- `PATCH /api/restaurant/{id}/hours` - Update business hours

### Menu Endpoints
- `POST /api/menu` - Create single menu item
- `POST /api/menu/bulk` - Create multiple items
- `GET /api/menu/owner/{ownerId}` - Get items by owner
- `GET /api/menu/restaurant/{restaurantId}` - Get items by restaurant
- `PUT /api/menu/{id}` - Update menu item
- `PATCH /api/menu/{id}/availability` - Update item availability
- `DELETE /api/menu/{id}` - Delete menu item

## Test Assertions

Each request includes test scripts to validate:
- ✅ Correct HTTP status codes
- ✅ Response structure and required fields
- ✅ Data relationships (restaurant → menu items)
- ✅ Variable extraction for subsequent requests

## Features Demonstrated

### Restaurant Registration
- Complete business profile creation
- Address with landmark
- Contact information validation
- Flexible business hours (different times per day)
- Cuisine type classification

### Menu Management
- Single and bulk item creation
- Time-based availability (e.g., breakfast-only items)
- Comprehensive food attributes (allergens, dietary flags)
- Nutritional information tracking
- Dynamic availability toggling

### Restaurant Updates
- Granular update endpoints (contact, address, hours)
- Status management (active/inactive)
- Real-time availability control

## Notes

- All requests require authentication (JWT bearer token)
- The collection automatically manages variables
- Restaurant owners can only modify their own restaurants
- Time-based availability is optional (leave null for all-day items)
- Business hours support different times for each day
- Status updates affect customer visibility (inactive restaurants hidden)

## Troubleshooting

**401 Unauthorized**
- Verify authentication token is set
- Re-run "Login Restaurant Owner" request

**404 Not Found**
- Check if services are running on correct ports
- Verify variable values (restaurant_id, menu_item_id)

**400 Bad Request**
- Review request body format
- Check required fields are included
- Validate data types (phone format, time format "HH:mm")

## Next Steps

After completing this workflow:
1. Test with different restaurant owner accounts
2. Create multiple restaurants per owner
3. Build comprehensive menus with varied items
4. Practice updating business hours for different schedules
5. Test time-based item availability with breakfast/lunch/dinner items
