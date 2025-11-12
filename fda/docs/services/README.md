# Food Delivery Menu Catalog Service - Documentation

## Overview

The Catalog Service has been transformed into a comprehensive **Food Delivery Menu System** designed specifically for food delivery applications. This service manages restaurant menus with detailed food item information, dietary preferences, preparation times, and advanced filtering capabilities.

## Key Features

### 🍕 Menu Item Properties
- **Name & Description**: Detailed food item information
- **Preparation Time**: Cooking/preparation time in minutes
- **Packaging Size**: Portion sizes (Small, Medium, Large, Family Size)
- **Unit of Measure (UOM)**: Flexible units (piece, serving, kg, liter, portion)
- **Price per UOM**: Accurate pricing per unit
- **Category**: Food categories (Appetizer, Main Course, Dessert, Beverage, Salad)
- **Cuisine**: Cuisine types (Italian, Indian, Chinese, American, Thai, etc.)
- **Ingredients**: Complete ingredient lists
- **Allergens**: Allergen information (Nuts, Dairy, Gluten, Eggs, etc.)

### 🥗 Dietary Preferences
- **Vegetarian**: Meat-free options
- **Vegan**: Plant-based only
- **Gluten-Free**: Celiac-friendly options
- **Spice Level**: 1-5 scale (1=Mild, 5=Very Spicy)

### 📊 Nutritional Information (Optional)
- Calories per serving
- Protein content (grams)
- Carbohydrates (grams)
- Fat content (grams)

### ⚡ Advanced Features
- **Availability Management**: Real-time item availability
- **Search Functionality**: Name-based search with regex support
- **Multi-Filter Support**: Combine multiple filters simultaneously
- **Quick Options**: Fast preparation items (≤30 minutes)
- **Price Range Filtering**: Budget-friendly options
- **Category Organization**: Well-structured menu presentation

## API Endpoints

### 🏥 Health & Status
```
GET /api/health                    - Basic health check
GET /api/health/detailed           - Detailed health with database status
GET /api/seed/status               - Menu statistics and seeding status
```

### 🍽️ Menu Management (Admin - Requires Authentication)
```
POST /api/menu                     - Create new menu item
POST /api/menu/bulk               - Create multiple menu items
PUT /api/menu/{id}                - Update menu item
DELETE /api/menu/{id}             - Delete menu item
PATCH /api/menu/{id}/availability - Update item availability
GET /api/menu/all                 - Get all items (including unavailable)
```

### 📋 Public Menu Access
```
GET /api/menu                     - Get all available menu items
GET /api/menu/{id}                - Get specific menu item by ID
```

### 🏷️ Category & Cuisine
```
GET /api/menu/categories          - Get all available categories
GET /api/menu/cuisines            - Get all available cuisines
GET /api/menu/category/{category} - Get items by category
GET /api/menu/cuisine/{cuisine}   - Get items by cuisine
GET /api/menu/menu/categories     - Get menu organized by categories
GET /api/menu/menu/cuisines       - Get menu organized by cuisines
```

### 🥬 Dietary Preferences
```
GET /api/menu/vegetarian          - Get vegetarian items
GET /api/menu/vegan              - Get vegan items
GET /api/menu/gluten-free        - Get gluten-free items
GET /api/menu/spice-level/{level} - Get items by spice level (1-5)
```

### 🔍 Search & Filter
```
GET /api/menu/search?searchTerm={term}              - Search by name
GET /api/menu/price-range?minPrice={min}&maxPrice={max} - Filter by price
GET /api/menu/quick?maxMinutes={minutes}            - Quick preparation items
GET /api/menu/filter?[multiple_parameters]          - Advanced filtering
```

### 🛠️ Data Seeding (Admin)
```
POST /api/seed/sample-menu        - Populate with sample menu data
```

## Advanced Filtering Parameters

The `/api/menu/filter` endpoint supports multiple simultaneous filters:

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `category` | string | Filter by food category | `category=Main Course` |
| `cuisine` | string | Filter by cuisine type | `cuisine=Italian` |
| `isVegetarian` | boolean | Vegetarian items only | `isVegetarian=true` |
| `isVegan` | boolean | Vegan items only | `isVegan=true` |
| `isGlutenFree` | boolean | Gluten-free items only | `isGlutenFree=true` |
| `maxSpiceLevel` | integer | Maximum spice level (1-5) | `maxSpiceLevel=3` |
| `minPrice` | decimal | Minimum price filter | `minPrice=10.00` |
| `maxPrice` | decimal | Maximum price filter | `maxPrice=25.00` |
| `maxPreparationTime` | integer | Max preparation time (minutes) | `maxPreparationTime=30` |

### Example Filter Queries
```
# Vegetarian Italian dishes under $20 with max 30min prep time
GET /api/menu/filter?cuisine=Italian&isVegetarian=true&maxPrice=20&maxPreparationTime=30

# Quick vegan options with mild spice level
GET /api/menu/filter?isVegan=true&maxSpiceLevel=2&maxPreparationTime=15

# Main course dishes between $15-$25
GET /api/menu/filter?category=Main Course&minPrice=15&maxPrice=25
```

## Sample Menu Data

The service includes a comprehensive sample dataset with 10 diverse food items:

### Appetizers
- **Margherita Pizza Slice** - Classic Italian (15 min, $8.99)
- **Chicken Wings** - American Buffalo style (20 min, $12.99)

### Main Courses
- **Chicken Biryani** - Indian aromatic rice dish (45 min, $18.99)
- **Vegetable Pad Thai** - Thai stir-fried noodles (25 min, $14.99)
- **Classic Beef Burger** - American classic (18 min, $16.99)

### Desserts
- **Chocolate Lava Cake** - French dessert with ice cream (12 min, $9.99)

### Beverages
- **Fresh Mango Lassi** - Indian yogurt drink (5 min, $5.99)
- **Green Smoothie** - Healthy blend (3 min, $7.99)

### Salads
- **Caesar Salad** - Mediterranean classic (8 min, $11.99)
- **Quinoa Buddha Bowl** - Healthy power bowl (15 min, $13.99)

## Database Schema

### MenuItem Collection Structure
```json
{
  "id": "ObjectId",
  "name": "string (required, max 100)",
  "description": "string (required, max 1000)",
  "preparationTimeMinutes": "int (1-300)",
  "packagingSize": "string (max 50)",
  "unitOfMeasure": "string (max 20)",
  "pricePerUOM": "decimal (min 0.01)",
  "category": "string (max 50)",
  "cuisine": "string (max 50)",
  "ingredients": ["string array"],
  "allergens": ["string array"],
  "isVegetarian": "boolean",
  "isVegan": "boolean",
  "isGlutenFree": "boolean",
  "spiceLevel": "int (1-5)",
  "isAvailable": "boolean",
  "imageUrl": "string (max 500, optional)",
  "createdAt": "DateTime",
  "updatedAt": "DateTime",
  "calories": "int? (optional)",
  "protein": "decimal? (optional)",
  "carbohydrates": "decimal? (optional)",
  "fat": "decimal? (optional)"
}
```

## Getting Started

### 1. Build and Run the Service
```bash
cd /path/to/services
dotnet build ./catalog/Services/catalog.Services.csproj
dotnet run --project ./catalog/Services/catalog.Services.csproj
```

### 2. Access Swagger Documentation
```
http://localhost:5002/swagger
```

### 3. Populate Sample Data (Optional)
```bash
# Requires authentication token
POST http://localhost:5002/api/seed/sample-menu
Authorization: Bearer <your-jwt-token>
```

### 4. Test Public Endpoints
```bash
# Get all available menu items
GET http://localhost:5002/api/menu

# Get menu by category
GET http://localhost:5002/api/menu/category/Main%20Course

# Search for chicken dishes
GET http://localhost:5002/api/menu/search?searchTerm=chicken

# Get vegetarian options
GET http://localhost:5002/api/menu/vegetarian
```

## Environment Configuration

### Required Environment Variables
```bash
MONGO_CONNECTION_STRING=mongodb://localhost:27017
```

### Database
- **Database Name**: `CatalogDb`
- **Collection Name**: `MenuItems`
- **Backward Compatibility**: `Items` collection maintained for migration

## Authentication

Protected endpoints require JWT Bearer token authentication:
```bash
Authorization: Bearer <your-jwt-token>
```

### Protected Endpoints
- All POST, PUT, DELETE operations
- Admin endpoints (`/api/menu/all`, `/api/seed/*`)
- Availability management

## Error Handling

The API returns standard HTTP status codes:
- `200 OK` - Success
- `201 Created` - Resource created
- `204 No Content` - Success with no response body
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Performance Considerations

### Indexing Recommendations
```javascript
// MongoDB indexes for optimal performance
db.MenuItems.createIndex({ "isAvailable": 1 })
db.MenuItems.createIndex({ "category": 1, "isAvailable": 1 })
db.MenuItems.createIndex({ "cuisine": 1, "isAvailable": 1 })
db.MenuItems.createIndex({ "pricePerUOM": 1, "isAvailable": 1 })
db.MenuItems.createIndex({ "preparationTimeMinutes": 1, "isAvailable": 1 })
db.MenuItems.createIndex({ "name": "text" }) // For text search
```

### Caching Strategies
- Consider caching frequently accessed endpoints
- Category and cuisine lists are good candidates for caching
- Menu statistics can be cached with TTL

## Testing

Use the provided test script to validate functionality:
```bash
dotnet run --project ./catalog/Testing/MenuApiTestScript.cs
```

The test script covers:
1. Health checks
2. Category and cuisine retrieval
3. Menu filtering by various criteria
4. Search functionality
5. Dietary preference filtering
6. Price range filtering
7. Statistics and status endpoints

## Migration Notes

### Backward Compatibility
- Original `Item` model maintained alongside new `MenuItem`
- Original `ItemController` and `ItemService` preserved
- Existing data structure supported during transition period

### Upgrading from Legacy System
1. Deploy new service version
2. Run data migration scripts (if needed)
3. Update client applications to use new endpoints
4. Phase out legacy endpoints after client migration

## Future Enhancements

### Planned Features
- **Reviews & Ratings**: Customer feedback system
- **Inventory Management**: Stock level tracking
- **Seasonal Menus**: Time-based menu availability
- **Combo Deals**: Package offerings
- **Customization Options**: Ingredient modifications
- **Image Management**: Multiple images per item
- **Recommendation Engine**: Personalized suggestions
- **Loyalty Integration**: Point-based rewards

### API Versioning
- Current version: `v1`
- Future versions will maintain backward compatibility
- Deprecated endpoints will have 6-month sunset period

---

*This documentation is for the Food Delivery Menu Catalog Service v2.0.0*
*Last updated: November 2025*