# Restaurant Owner Workflows - Test Data Reference

Complete reference for test data used in Restaurant Owner workflow testing, including sample accounts, restaurant profiles, menu items, and MongoDB seed scripts.

---

## Table of Contents
1. [Test Accounts](#test-accounts)
2. [Restaurant Profiles](#restaurant-profiles)
3. [Menu Items](#menu-items)
4. [MongoDB Seed Data](#mongodb-seed-data)
5. [Testing Checklist](#testing-checklist)
6. [Data Validation Rules](#data-validation-rules)

---

## Test Accounts

### Restaurant Owner Accounts

#### Owner 1 - Pizza Palace
```json
{
  "email": "owner.pizzapalace@example.com",
  "password": "Owner123!",
  "firstName": "Maria",
  "lastName": "Rossi",
  "phone": "+1-555-0101",
  "role": 1,
  "organization": "Pizza Palace Inc"
}
```
- **Use Case**: Italian restaurant owner, multi-location
- **Restaurant**: Pizza Palace Downtown
- **Cuisine**: Italian (Pizza, Pasta)

#### Owner 2 - Sushi Spot
```json
{
  "email": "owner.sushispot@example.com",
  "password": "Owner123!",
  "firstName": "Takeshi",
  "lastName": "Yamamoto",
  "phone": "+1-555-0102",
  "role": 1,
  "organization": "Sushi Spot LLC"
}
```
- **Use Case**: Japanese restaurant owner
- **Restaurant**: Sushi Spot
- **Cuisine**: Japanese (Sushi, Ramen)

#### Owner 3 - Burger Hub
```json
{
  "email": "owner.burgerhub@example.com",
  "password": "Owner123!",
  "firstName": "John",
  "lastName": "Smith",
  "phone": "+1-555-0103",
  "role": 1,
  "organization": "Burger Hub Corp"
}
```
- **Use Case**: American restaurant owner, late-night service
- **Restaurant**: Burger Hub
- **Cuisine**: American (Burgers, Wings, Fries)

#### Owner 4 - Taco Town
```json
{
  "email": "owner.tacotown@example.com",
  "password": "Owner123!",
  "firstName": "Carlos",
  "lastName": "Rodriguez",
  "phone": "+1-555-0104",
  "role": 1,
  "organization": "Taco Town"
}
```
- **Use Case**: Mexican restaurant owner
- **Restaurant**: Taco Town
- **Cuisine**: Mexican (Tacos, Burritos, Quesadillas)

#### Owner 5 - Indian Cuisine Express
```json
{
  "email": "owner.indiancuisine@example.com",
  "password": "Owner123!",
  "firstName": "Raj",
  "lastName": "Patel",
  "phone": "+1-555-0105",
  "role": 1,
  "organization": "Indian Cuisine Express"
}
```
- **Use Case**: Indian restaurant owner, lunch buffet specialist
- **Restaurant**: Indian Cuisine Express
- **Cuisine**: Indian (Curry, Biryani, Naan)

---

## Restaurant Profiles

### Restaurant 1: Pizza Palace Downtown

```json
{
  "restaurant_id": "6738a123456789abcdef0001",
  "name": "Pizza Palace Downtown",
  "owner_id": "{{user_id}}",
  "description": "Authentic Italian pizzas and pasta with fresh ingredients daily",
  "cuisine_type": "Italian",
  "address": {
    "street": "123 Main Street",
    "city": "San Francisco",
    "state": "CA",
    "zip_code": "94102",
    "country": "USA",
    "coordinates": {
      "latitude": 37.7749,
      "longitude": -122.4194
    },
    "landmark": "Near Central Park"
  },
  "contact_info": {
    "phone": "+1-555-0101",
    "email": "contact@pizzapalace.com",
    "website": "https://www.pizzapalace.com"
  },
  "timings": {
    "monday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "tuesday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "wednesday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "thursday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "friday": { "is_open": true, "open_time": "11:00", "close_time": "23:00" },
    "saturday": { "is_open": true, "open_time": "11:00", "close_time": "23:00" },
    "sunday": { "is_open": true, "open_time": "10:00", "close_time": "22:00" }
  },
  "average_rating": 4.5,
  "total_reviews": 250,
  "isActive": true,
  "image_url": "https://images.example.com/pizzapalace.jpg"
}
```

**Business Hours Summary**:
- Mon-Thu: 11:00 AM - 10:00 PM
- Fri-Sat: 11:00 AM - 11:00 PM (extended weekend hours)
- Sunday: 10:00 AM - 10:00 PM (brunch available)

**Testing Scenarios**:
- ✅ Register with complete business information
- ✅ Update extended weekend hours
- ✅ Add Sunday brunch hours
- ✅ Update contact information
- ✅ Toggle active status

---

### Restaurant 2: Sushi Spot

```json
{
  "restaurant_id": "6738a123456789abcdef0002",
  "name": "Sushi Spot",
  "owner_id": "{{user_id}}",
  "description": "Fresh sushi and authentic Japanese cuisine",
  "cuisine_type": "Japanese",
  "address": {
    "street": "456 Ocean Avenue",
    "city": "San Francisco",
    "state": "CA",
    "zip_code": "94112",
    "country": "USA",
    "coordinates": {
      "latitude": 37.7249,
      "longitude": -122.4794
    },
    "landmark": "Waterfront District"
  },
  "contact_info": {
    "phone": "+1-555-0102",
    "email": "hello@sushispot.com",
    "website": "https://www.sushispot.com"
  },
  "timings": {
    "monday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
    "tuesday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
    "wednesday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
    "thursday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
    "friday": { "is_open": true, "open_time": "12:00", "close_time": "23:30" },
    "saturday": { "is_open": true, "open_time": "12:00", "close_time": "23:30" },
    "sunday": { "is_open": false, "open_time": null, "close_time": null }
  },
  "average_rating": 4.8,
  "total_reviews": 180,
  "isActive": true,
  "image_url": "https://images.example.com/sushispot.jpg"
}
```

**Business Hours Summary**:
- Mon-Thu: 12:00 PM - 10:00 PM
- Fri-Sat: 12:00 PM - 11:30 PM
- **Sunday: CLOSED** (weekly rest day)

**Testing Scenarios**:
- ✅ Register with closed Sunday
- ✅ Test closed day validation
- ✅ Update to open on Sunday (holiday season)
- ✅ Waterfront location handling

---

### Restaurant 3: Burger Hub

```json
{
  "restaurant_id": "6738a123456789abcdef0003",
  "name": "Burger Hub",
  "owner_id": "{{user_id}}",
  "description": "Gourmet burgers and craft beers",
  "cuisine_type": "American",
  "address": {
    "street": "789 Broadway",
    "city": "San Francisco",
    "state": "CA",
    "zip_code": "94133",
    "country": "USA",
    "coordinates": {
      "latitude": 37.7989,
      "longitude": -122.4094
    },
    "landmark": "Theater District"
  },
  "contact_info": {
    "phone": "+1-555-0103",
    "email": "info@burgerhub.com",
    "website": "https://www.burgerhub.com"
  },
  "timings": {
    "monday": { "is_open": true, "open_time": "10:30", "close_time": "23:00" },
    "tuesday": { "is_open": true, "open_time": "10:30", "close_time": "23:00" },
    "wednesday": { "is_open": true, "open_time": "10:30", "close_time": "23:00" },
    "thursday": { "is_open": true, "open_time": "10:30", "close_time": "23:00" },
    "friday": { "is_open": true, "open_time": "10:30", "close_time": "00:30" },
    "saturday": { "is_open": true, "open_time": "10:30", "close_time": "00:30" },
    "sunday": { "is_open": true, "open_time": "10:00", "close_time": "23:00" }
  },
  "average_rating": 4.3,
  "total_reviews": 320,
  "isActive": true,
  "image_url": "https://images.example.com/burgerhub.jpg"
}
```

**Business Hours Summary**:
- Mon-Thu: 10:30 AM - 11:00 PM
- **Fri-Sat: 10:30 AM - 12:30 AM** (late night service)
- Sunday: 10:00 AM - 11:00 PM

**Testing Scenarios**:
- ✅ Register with late-night hours (past midnight)
- ✅ Test midnight crossing times
- ✅ Theater district location
- ✅ High-volume weekend operations

---

### Restaurant 4: Taco Town

```json
{
  "restaurant_id": "6738a123456789abcdef0004",
  "name": "Taco Town",
  "owner_id": "{{user_id}}",
  "description": "Authentic Mexican street food and tacos",
  "cuisine_type": "Mexican",
  "address": {
    "street": "321 Mission Street",
    "city": "San Francisco",
    "state": "CA",
    "zip_code": "94110",
    "country": "USA",
    "coordinates": {
      "latitude": 37.7599,
      "longitude": -122.4148
    },
    "landmark": "Mission District"
  },
  "contact_info": {
    "phone": "+1-555-0104",
    "email": "orders@tacotown.com",
    "website": "https://www.tacotown.com"
  },
  "timings": {
    "monday": { "is_open": true, "open_time": "11:00", "close_time": "21:00" },
    "tuesday": { "is_open": true, "open_time": "11:00", "close_time": "21:00" },
    "wednesday": { "is_open": true, "open_time": "11:00", "close_time": "21:00" },
    "thursday": { "is_open": true, "open_time": "11:00", "close_time": "21:00" },
    "friday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "saturday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
    "sunday": { "is_open": true, "open_time": "10:00", "close_time": "21:00" }
  },
  "average_rating": 4.6,
  "total_reviews": 150,
  "isActive": true,
  "image_url": "https://images.example.com/tacotown.jpg"
}
```

**Business Hours Summary**:
- Mon-Thu: 11:00 AM - 9:00 PM
- Fri-Sat: 11:00 AM - 10:00 PM
- Sunday: 10:00 AM - 9:00 PM

**Testing Scenarios**:
- ✅ Register with moderate hours
- ✅ Mission District location
- ✅ Family-friendly hours

---

### Restaurant 5: Indian Cuisine Express

```json
{
  "restaurant_id": "6738a123456789abcdef0005",
  "name": "Indian Cuisine Express",
  "owner_id": "{{user_id}}",
  "description": "Traditional Indian curries and biryanis",
  "cuisine_type": "Indian",
  "address": {
    "street": "555 Market Street",
    "city": "San Francisco",
    "state": "CA",
    "zip_code": "94105",
    "country": "USA",
    "coordinates": {
      "latitude": 37.7899,
      "longitude": -122.4008
    },
    "landmark": "Financial District"
  },
  "contact_info": {
    "phone": "+1-555-0105",
    "email": "contact@indiancuisineexpress.com",
    "website": "https://www.indiancuisineexpress.com"
  },
  "timings": {
    "monday": { "is_open": true, "open_time": "11:30", "close_time": "22:00" },
    "tuesday": { "is_open": true, "open_time": "11:30", "close_time": "22:00" },
    "wednesday": { "is_open": true, "open_time": "11:30", "close_time": "22:00" },
    "thursday": { "is_open": true, "open_time": "11:30", "close_time": "22:00" },
    "friday": { "is_open": true, "open_time": "11:30", "close_time": "22:30" },
    "saturday": { "is_open": true, "open_time": "11:30", "close_time": "22:30" },
    "sunday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" }
  },
  "average_rating": 4.7,
  "total_reviews": 200,
  "isActive": true,
  "image_url": "https://images.example.com/indiancuisine.jpg"
}
```

**Business Hours Summary**:
- Mon-Thu: 11:30 AM - 10:00 PM (lunch buffet 11:30-2:30)
- Fri-Sat: 11:30 AM - 10:30 PM
- Sunday: 11:00 AM - 10:00 PM

**Testing Scenarios**:
- ✅ Register with lunch buffet hours
- ✅ Financial District location (business lunch focus)
- ✅ Extended Friday/Saturday hours

---

## Menu Items

### Pizza Items (5 items)

#### 1. Pizza Margherita (Vegetarian)
```json
{
  "menu_item_id": "menu001",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Pizza Margherita",
  "description": "Classic Italian pizza with tomato sauce, fresh mozzarella, and basil leaves",
  "category": "Main Course",
  "price": 12.99,
  "portions": "12 inch",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 50,
  "minimumQuantity": 10,
  "calories": 250,
  "image_url": "https://images.example.com/margherita.jpg"
}
```

#### 2. Pepperoni Pizza
```json
{
  "menu_item_id": "menu002",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Pepperoni Pizza",
  "description": "Classic pepperoni pizza with mozzarella cheese and tomato sauce",
  "category": "Main Course",
  "price": 14.99,
  "portions": "12 inch",
  "isAvailable": true,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 45,
  "minimumQuantity": 10,
  "calories": 290,
  "image_url": "https://images.example.com/pepperoni.jpg"
}
```

#### 3. Veggie Supreme Pizza (Vegetarian)
```json
{
  "menu_item_id": "menu003",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Veggie Supreme Pizza",
  "description": "Loaded with bell peppers, onions, mushrooms, olives, and tomatoes",
  "category": "Main Course",
  "price": 15.99,
  "portions": "12 inch",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 40,
  "minimumQuantity": 8,
  "calories": 270,
  "image_url": "https://images.example.com/veggiesupreme.jpg"
}
```

#### 4. Hawaiian Pizza
```json
{
  "menu_item_id": "menu004",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Hawaiian Pizza",
  "description": "Ham, pineapple, and mozzarella cheese on tomato sauce base",
  "category": "Main Course",
  "price": 15.99,
  "portions": "12 inch",
  "isAvailable": true,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 35,
  "minimumQuantity": 8,
  "calories": 280,
  "image_url": "https://images.example.com/hawaiian.jpg"
}
```

#### 5. Breakfast Pizza (Time-Limited)
```json
{
  "menu_item_id": "menu005",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Breakfast Pizza",
  "description": "Eggs, bacon, sausage, and cheese on a crispy crust",
  "category": "Breakfast",
  "price": 9.99,
  "portions": "10 inch",
  "isAvailable": true,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false,
  "availableFromTime": "08:00",
  "availableToTime": "11:30",
  "quantityAvailable": 20,
  "minimumQuantity": 5,
  "calories": 350,
  "image_url": "https://images.example.com/breakfast-pizza.jpg"
}
```
**Special Feature**: Time-based availability (08:00-11:30 only)

---

### Pasta Items (3 items)

#### 6. Spaghetti Carbonara
```json
{
  "menu_item_id": "menu006",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Spaghetti Carbonara",
  "description": "Traditional Italian pasta with eggs, cheese, pancetta, and black pepper",
  "category": "Main Course",
  "price": 13.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": false,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 30,
  "minimumQuantity": 8,
  "calories": 580,
  "image_url": "https://images.example.com/carbonara.jpg"
}
```

#### 7. Fettuccine Alfredo (Vegetarian)
```json
{
  "menu_item_id": "menu007",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Fettuccine Alfredo",
  "description": "Creamy Alfredo sauce with parmesan cheese over fettuccine pasta",
  "category": "Main Course",
  "price": 12.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 25,
  "minimumQuantity": 8,
  "calories": 520,
  "image_url": "https://images.example.com/alfredo.jpg"
}
```

#### 8. Penne Arrabiata (Vegan Option)
```json
{
  "menu_item_id": "menu008",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Penne Arrabiata",
  "description": "Spicy tomato sauce with garlic, red chili flakes, and fresh parsley",
  "category": "Main Course",
  "price": 11.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": true,
  "isGlutenFree": false,
  "quantityAvailable": 28,
  "minimumQuantity": 8,
  "calories": 420,
  "spice_level": "Medium",
  "image_url": "https://images.example.com/arrabiata.jpg"
}
```

---

### Appetizers (3 items)

#### 9. Garlic Bread (Vegetarian)
```json
{
  "menu_item_id": "menu009",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Garlic Bread",
  "description": "Toasted bread with garlic butter and parsley",
  "category": "Appetizer",
  "price": 5.99,
  "portions": "6 pieces",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 60,
  "minimumQuantity": 15,
  "calories": 150,
  "image_url": "https://images.example.com/garlicbread.jpg"
}
```

#### 10. Bruschetta (Vegetarian, Vegan)
```json
{
  "menu_item_id": "menu010",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Bruschetta",
  "description": "Grilled bread topped with fresh tomatoes, basil, and olive oil",
  "category": "Appetizer",
  "price": 7.99,
  "portions": "4 pieces",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": true,
  "isGlutenFree": false,
  "quantityAvailable": 40,
  "minimumQuantity": 10,
  "calories": 120,
  "image_url": "https://images.example.com/bruschetta.jpg"
}
```

#### 11. Mozzarella Sticks (Vegetarian)
```json
{
  "menu_item_id": "menu011",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Mozzarella Sticks",
  "description": "Breaded and fried mozzarella cheese sticks with marinara sauce",
  "category": "Appetizer",
  "price": 8.99,
  "portions": "6 pieces",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 50,
  "minimumQuantity": 12,
  "calories": 280,
  "image_url": "https://images.example.com/mozzarellasticks.jpg"
}
```

---

### Salads & Soups (3 items)

#### 12. Caesar Salad (Vegetarian)
```json
{
  "menu_item_id": "menu012",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Caesar Salad",
  "description": "Romaine lettuce, parmesan cheese, croutons, and Caesar dressing",
  "category": "Salad",
  "price": 8.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 35,
  "minimumQuantity": 10,
  "calories": 180,
  "image_url": "https://images.example.com/caesar.jpg"
}
```

#### 13. Greek Salad (Vegetarian)
```json
{
  "menu_item_id": "menu013",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Greek Salad",
  "description": "Fresh tomatoes, cucumbers, olives, feta cheese, and olive oil dressing",
  "category": "Salad",
  "price": 9.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": true,
  "quantityAvailable": 30,
  "minimumQuantity": 10,
  "calories": 160,
  "image_url": "https://images.example.com/greek-salad.jpg"
}
```

#### 14. Tomato Basil Soup (Vegetarian, Vegan, Gluten-Free)
```json
{
  "menu_item_id": "menu014",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Tomato Basil Soup",
  "description": "Homemade tomato soup with fresh basil and herbs",
  "category": "Soup",
  "price": 6.99,
  "portions": "Large bowl",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": true,
  "isGlutenFree": true,
  "quantityAvailable": 25,
  "minimumQuantity": 8,
  "calories": 140,
  "image_url": "https://images.example.com/tomato-soup.jpg"
}
```

---

### Desserts (1 item)

#### 15. Tiramisu (Vegetarian)
```json
{
  "menu_item_id": "menu015",
  "restaurant_id": "{{restaurant_id}}",
  "owner_id": "{{user_id}}",
  "name": "Tiramisu",
  "description": "Classic Italian dessert with coffee-soaked ladyfingers and mascarpone cream",
  "category": "Dessert",
  "price": 7.99,
  "portions": "Single serving",
  "isAvailable": true,
  "isVegetarian": true,
  "isVegan": false,
  "isGlutenFree": false,
  "quantityAvailable": 20,
  "minimumQuantity": 5,
  "calories": 450,
  "image_url": "https://images.example.com/tiramisu.jpg"
}
```

---

## MongoDB Seed Data

### Seed Script for Restaurant Owners

```javascript
// Insert Restaurant Owner Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6738a111111111111111001"),
    "email": "owner.pizzapalace@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "firstName": "Maria",
    "lastName": "Rossi",
    "phone": "+1-555-0101",
    "role": 1, // RestaurantOwner
    "organization": "Pizza Palace Inc",
    "createdAt": ISODate("2025-11-01T10:00:00Z"),
    "updatedAt": ISODate("2025-11-01T10:00:00Z")
  },
  {
    "_id": ObjectId("6738a111111111111111002"),
    "email": "owner.sushispot@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "firstName": "Takeshi",
    "lastName": "Yamamoto",
    "phone": "+1-555-0102",
    "role": 1,
    "organization": "Sushi Spot LLC",
    "createdAt": ISODate("2025-11-01T10:15:00Z"),
    "updatedAt": ISODate("2025-11-01T10:15:00Z")
  },
  {
    "_id": ObjectId("6738a111111111111111003"),
    "email": "owner.burgerhub@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "firstName": "John",
    "lastName": "Smith",
    "phone": "+1-555-0103",
    "role": 1,
    "organization": "Burger Hub Corp",
    "createdAt": ISODate("2025-11-01T10:30:00Z"),
    "updatedAt": ISODate("2025-11-01T10:30:00Z")
  }
]);
```

### Seed Script for Restaurants

```javascript
// Insert Restaurant Profiles
db.restaurants.insertMany([
  {
    "_id": ObjectId("6738a123456789abcdef0001"),
    "name": "Pizza Palace Downtown",
    "owner_id": ObjectId("6738a111111111111111001"),
    "description": "Authentic Italian pizzas and pasta with fresh ingredients daily",
    "cuisine_type": "Italian",
    "address": {
      "street": "123 Main Street",
      "city": "San Francisco",
      "state": "CA",
      "zip_code": "94102",
      "country": "USA",
      "coordinates": {
        "latitude": 37.7749,
        "longitude": -122.4194
      },
      "landmark": "Near Central Park"
    },
    "contact_info": {
      "phone": "+1-555-0101",
      "email": "contact@pizzapalace.com",
      "website": "https://www.pizzapalace.com"
    },
    "timings": {
      "monday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
      "tuesday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
      "wednesday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
      "thursday": { "is_open": true, "open_time": "11:00", "close_time": "22:00" },
      "friday": { "is_open": true, "open_time": "11:00", "close_time": "23:00" },
      "saturday": { "is_open": true, "open_time": "11:00", "close_time": "23:00" },
      "sunday": { "is_open": true, "open_time": "10:00", "close_time": "22:00" }
    },
    "average_rating": 4.5,
    "total_reviews": 250,
    "isActive": true,
    "image_url": "https://images.example.com/pizzapalace.jpg",
    "createdAt": ISODate("2025-11-01T11:00:00Z"),
    "updatedAt": ISODate("2025-11-01T11:00:00Z")
  },
  {
    "_id": ObjectId("6738a123456789abcdef0002"),
    "name": "Sushi Spot",
    "owner_id": ObjectId("6738a111111111111111002"),
    "description": "Fresh sushi and authentic Japanese cuisine",
    "cuisine_type": "Japanese",
    "address": {
      "street": "456 Ocean Avenue",
      "city": "San Francisco",
      "state": "CA",
      "zip_code": "94112",
      "country": "USA",
      "coordinates": {
        "latitude": 37.7249,
        "longitude": -122.4794
      },
      "landmark": "Waterfront District"
    },
    "contact_info": {
      "phone": "+1-555-0102",
      "email": "hello@sushispot.com",
      "website": "https://www.sushispot.com"
    },
    "timings": {
      "monday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
      "tuesday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
      "wednesday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
      "thursday": { "is_open": true, "open_time": "12:00", "close_time": "22:00" },
      "friday": { "is_open": true, "open_time": "12:00", "close_time": "23:30" },
      "saturday": { "is_open": true, "open_time": "12:00", "close_time": "23:30" },
      "sunday": { "is_open": false, "open_time": null, "close_time": null }
    },
    "average_rating": 4.8,
    "total_reviews": 180,
    "isActive": true,
    "image_url": "https://images.example.com/sushispot.jpg",
    "createdAt": ISODate("2025-11-01T11:15:00Z"),
    "updatedAt": ISODate("2025-11-01T11:15:00Z")
  }
]);
```

### Seed Script for Menu Items

```javascript
// Insert Menu Items (Pizza Palace)
db.menu_items.insertMany([
  {
    "_id": ObjectId("menu00000000000000000001"),
    "restaurant_id": ObjectId("6738a123456789abcdef0001"),
    "owner_id": ObjectId("6738a111111111111111001"),
    "name": "Pizza Margherita",
    "description": "Classic Italian pizza with tomato sauce, fresh mozzarella, and basil leaves",
    "category": "Main Course",
    "price": 12.99,
    "portions": "12 inch",
    "isAvailable": true,
    "isVegetarian": true,
    "isVegan": false,
    "isGlutenFree": false,
    "quantityAvailable": 50,
    "minimumQuantity": 10,
    "calories": 250,
    "image_url": "https://images.example.com/margherita.jpg",
    "createdAt": ISODate("2025-11-01T12:00:00Z"),
    "updatedAt": ISODate("2025-11-01T12:00:00Z")
  },
  {
    "_id": ObjectId("menu00000000000000000002"),
    "restaurant_id": ObjectId("6738a123456789abcdef0001"),
    "owner_id": ObjectId("6738a111111111111111001"),
    "name": "Pepperoni Pizza",
    "description": "Classic pepperoni pizza with mozzarella cheese and tomato sauce",
    "category": "Main Course",
    "price": 14.99,
    "portions": "12 inch",
    "isAvailable": true,
    "isVegetarian": false,
    "isVegan": false,
    "isGlutenFree": false,
    "quantityAvailable": 45,
    "minimumQuantity": 10,
    "calories": 290,
    "image_url": "https://images.example.com/pepperoni.jpg",
    "createdAt": ISODate("2025-11-01T12:05:00Z"),
    "updatedAt": ISODate("2025-11-01T12:05:00Z")
  },
  {
    "_id": ObjectId("menu00000000000000000005"),
    "restaurant_id": ObjectId("6738a123456789abcdef0001"),
    "owner_id": ObjectId("6738a111111111111111001"),
    "name": "Breakfast Pizza",
    "description": "Eggs, bacon, sausage, and cheese on a crispy crust",
    "category": "Breakfast",
    "price": 9.99,
    "portions": "10 inch",
    "isAvailable": true,
    "isVegetarian": false,
    "isVegan": false,
    "isGlutenFree": false,
    "availableFromTime": "08:00",
    "availableToTime": "11:30",
    "quantityAvailable": 20,
    "minimumQuantity": 5,
    "calories": 350,
    "image_url": "https://images.example.com/breakfast-pizza.jpg",
    "createdAt": ISODate("2025-11-01T12:20:00Z"),
    "updatedAt": ISODate("2025-11-01T12:20:00Z")
  }
]);
```

---

## Testing Checklist

### 1. Restaurant Registration Tests
- [ ] Register Pizza Palace with complete timings
- [ ] Register Sushi Spot with closed Sunday
- [ ] Register Burger Hub with late-night hours (past midnight)
- [ ] Register Taco Town with moderate hours
- [ ] Register Indian Cuisine Express with lunch focus
- [ ] Verify unique addresses for all restaurants
- [ ] Validate phone number formats
- [ ] Check email format validation
- [ ] Test coordinate validation (latitude/longitude)

### 2. Menu Management Tests
- [ ] Add single menu item
- [ ] Add multiple items (bulk operation)
- [ ] Add time-based item (Breakfast Pizza 08:00-11:30)
- [ ] Update item price
- [ ] Update item availability (toggle on/off)
- [ ] Update quantity available
- [ ] Delete menu item
- [ ] Get all items by restaurant
- [ ] Filter items by category
- [ ] Filter items by dietary preference (vegetarian/vegan/gluten-free)

### 3. Restaurant Status Management
- [ ] Update restaurant status to inactive
- [ ] Verify inactive restaurant not visible to customers
- [ ] Update status back to active
- [ ] Test search functionality with inactive restaurants

### 4. Business Hours Management
- [ ] Update single day hours (Monday)
- [ ] Update weekend hours (Friday-Sunday)
- [ ] Set closed day (isOpen: false)
- [ ] Extend hours (late night service)
- [ ] Update all weekdays at once
- [ ] Test midnight crossing times (Fri 10:30-00:30)
- [ ] Validate time format (HH:mm)

### 5. Contact Information Updates
- [ ] Update phone number
- [ ] Update email address
- [ ] Update website URL
- [ ] Update all contact info together
- [ ] Validate phone format (+1-XXX-XXXX)
- [ ] Validate email format

### 6. Address Updates
- [ ] Update street address
- [ ] Update city
- [ ] Update state/zip code
- [ ] Update coordinates
- [ ] Update landmark
- [ ] Validate zip code format

### 7. Time-Based Availability Tests
- [ ] Add Breakfast Pizza (08:00-11:30)
- [ ] Test at 09:00 AM → should be available
- [ ] Test at 12:00 PM → should be unavailable
- [ ] Update availableToTime to 12:00
- [ ] Verify updated availability
- [ ] Test null times → all-day availability

### 8. Dietary Filter Tests
- [ ] Filter vegetarian items (9 items expected)
- [ ] Filter vegan items (3 items expected)
- [ ] Filter gluten-free items (2 items expected)
- [ ] Filter non-vegetarian items
- [ ] Multiple filters (vegetarian + gluten-free)

### 9. Data-Driven Tests (Newman)
- [ ] Run with restaurants.json (5 registrations)
- [ ] Run with menu-items.json (15 item additions)
- [ ] Iterate through all restaurant profiles
- [ ] Iterate through all menu categories
- [ ] Generate HTML test report

### 10. Error Handling Tests
- [ ] Register with missing required fields
- [ ] Update with invalid restaurant_id
- [ ] Add menu item with negative price
- [ ] Set invalid time format
- [ ] Update non-existent restaurant
- [ ] Delete non-existent menu item
- [ ] Unauthorized access (wrong owner)

---

## Data Validation Rules

### Restaurant Fields
| Field | Type | Validation | Required |
|-------|------|------------|----------|
| name | string | 3-100 characters | Yes |
| owner_id | ObjectId | Valid MongoDB ObjectId | Yes |
| description | string | 10-500 characters | Yes |
| cuisine_type | string | Italian, Japanese, American, Mexican, Indian, etc. | Yes |
| address.street | string | 5-200 characters | Yes |
| address.city | string | 2-50 characters | Yes |
| address.state | string | 2 characters (US state code) | Yes |
| address.zip_code | string | 5 digits or 5+4 format (XXXXX or XXXXX-XXXX) | Yes |
| contact_info.phone | string | +1-XXX-XXXX or (XXX) XXX-XXXX | Yes |
| contact_info.email | string | Valid email format (user@domain.com) | Yes |
| timings.*.is_open | boolean | true/false | Yes |
| timings.*.open_time | string | HH:mm format (24-hour) | If is_open = true |
| timings.*.close_time | string | HH:mm format (24-hour) | If is_open = true |
| isActive | boolean | true/false | Yes |
| average_rating | decimal | 0.0 - 5.0 | No |

### Menu Item Fields
| Field | Type | Validation | Required |
|-------|------|------------|----------|
| name | string | 3-100 characters | Yes |
| restaurant_id | ObjectId | Valid MongoDB ObjectId | Yes |
| owner_id | ObjectId | Valid MongoDB ObjectId | Yes |
| description | string | 10-500 characters | Yes |
| category | string | Main Course, Appetizer, Dessert, Salad, Soup, Breakfast | Yes |
| price | decimal | > 0.00, 2 decimal places | Yes |
| portions | string | Descriptive (12 inch, 6 pieces, Large bowl) | Yes |
| isAvailable | boolean | true/false | Yes |
| isVegetarian | boolean | true/false | No (default: false) |
| isVegan | boolean | true/false | No (default: false) |
| isGlutenFree | boolean | true/false | No (default: false) |
| quantityAvailable | integer | >= 0 | No |
| minimumQuantity | integer | >= 0 | No |
| availableFromTime | string | HH:mm format (24-hour) | No |
| availableToTime | string | HH:mm format (24-hour) | No |
| calories | integer | >= 0 | No |

---

## Quick Reference

### Postman Collection Variables
```
{{base_url}}          → http://localhost:30002
{{auth_token}}        → JWT token (auto-set from login)
{{user_id}}           → Owner ID (auto-set from login)
{{restaurant_id}}     → Restaurant ID (auto-set from registration)
{{menu_item_id}}      → Menu item ID (auto-set from creation)
```

### Test Data Counts
- **Restaurant Owners**: 5 accounts
- **Restaurants**: 5 profiles (Italian, Japanese, American, Mexican, Indian)
- **Menu Items**: 15 items
  - Pizzas: 5 items
  - Pasta: 3 items
  - Appetizers: 3 items
  - Salads & Soups: 3 items
  - Desserts: 1 item
- **Dietary Options**:
  - Vegetarian: 9 items
  - Vegan: 3 items
  - Gluten-Free: 2 items

### Newman Command Examples
```powershell
# Full workflow test
newman run Restaurant-Owner-Workflows.postman_collection.json -e ../Capstone-Local-Environment.postman_environment.json

# Register all restaurants
newman run Restaurant-Owner-Workflows.postman_collection.json --folder "Restaurant Registration" -d data/restaurants.json -e ../Capstone-Local-Environment.postman_environment.json

# Add all menu items
newman run Restaurant-Owner-Workflows.postman_collection.json --folder "Menu Management - Add Items" -d data/menu-items.json -e ../Capstone-Local-Environment.postman_environment.json

# Generate HTML report
newman run Restaurant-Owner-Workflows.postman_collection.json -e ../Capstone-Local-Environment.postman_environment.json --reporters cli,htmlextra --reporter-htmlextra-export test-results/restaurant-owner-report.html
```

---

**Version**: 1.0.0  
**Last Updated**: November 20, 2025  
**Data Maintained By**: FDA QA Team  
**Related Documents**: 
- [Operator Service Workflows Test Data](../operator-service-workflows/TEST-DATA-REFERENCE.md)
- [User Registration Test Data](../user-registration/TEST-DATA-REFERENCE.md)
- [RBAC Documentation](../../docs/services/RBAC-COMPREHENSIVE.md)
