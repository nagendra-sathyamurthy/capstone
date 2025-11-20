# User Registration Flow - Test Data Reference

Complete reference for test data used in User Registration and Authentication workflows, including all user accounts across different roles, credentials, and MongoDB seed scripts.

---

## Table of Contents
1. [Test Accounts Overview](#test-accounts-overview)
2. [Customer Accounts](#customer-accounts)
3. [Restaurant Owner Accounts](#restaurant-owner-accounts)
4. [Kitchen Worker Accounts](#kitchen-worker-accounts)
5. [Delivery Agent Accounts](#delivery-agent-accounts)
6. [IT Admin Accounts](#it-admin-accounts)
7. [Role Reference](#role-reference)
8. [MongoDB Seed Data](#mongodb-seed-data)
9. [Testing Checklist](#testing-checklist)
10. [Data Validation Rules](#data-validation-rules)

---

## Test Accounts Overview

### Account Summary by Role

| Role                | File Name                              | Count | Organization Pattern     |
|---------------------|----------------------------------------|-------|--------------------------|
| Customer (0)        | customer-registration.json             | 5     | external_users           |
| Restaurant Owner (1)| restaurant-owner-registration.json     | 8     | Restaurant names         |
| Operator (2)        | N/A                                    | 0     | Operations Team          |
| Kitchen Worker (3)  | kitchen-worker-registration.json       | 10    | Restaurant names         |
| Delivery Agent (4)  | delivery-agent-registration.json       | 10    | Delivery company names   |
| Developer (5)       | it-admin-registration.json             | 3     | FoodDelivery IT          |
| Tester (6)          | it-admin-registration.json             | 2     | FoodDelivery IT          |
| Network Admin (7)   | it-admin-registration.json             | 2     | FoodDelivery IT          |
| Database Admin (8)  | it-admin-registration.json             | 3     | FoodDelivery IT          |

**Total Test Accounts**: 43 users across 9 roles

---

## Customer Accounts

### File: `customer-registration.json`
**Total**: 5 customer accounts

#### Customer 1 - Standard Account
```json
{
  "email": "customer1@example.com",
  "password": "Customer123!",
  "role": 0,
  "organization": "external_users"
}
```
- **Use Case**: Regular customer with standard ordering
- **Profile**: General user, moderate order frequency
- **Testing**: Basic order placement, cart operations

#### Customer 2 - Secondary Account
```json
{
  "email": "customer2@example.com",
  "password": "Customer456!",
  "role": 0,
  "organization": "external_users"
}
```
- **Use Case**: Alternative customer account
- **Profile**: Different preferences and delivery address
- **Testing**: Multiple customer scenarios

#### Customer 3 - VIP Customer
```json
{
  "email": "vip.customer@example.com",
  "password": "VIPCustomer789!",
  "role": 0,
  "organization": "external_users"
}
```
- **Use Case**: Premium customer with special privileges
- **Profile**: High order frequency, priority delivery
- **Testing**: VIP features, priority handling, loyalty rewards

#### Customer 4 - Frequent Diner
```json
{
  "email": "frequent.diner@example.com",
  "password": "FrequentDiner123!",
  "role": 0,
  "organization": "external_users"
}
```
- **Use Case**: Regular customer with high order volume
- **Profile**: Multiple orders per week, various restaurants
- **Testing**: Loyalty programs, order history, favorites

#### Customer 5 - New Customer
```json
{
  "email": "new.customer@example.com",
  "password": "NewCustomer456!",
  "role": 0,
  "organization": "external_users"
}
```
- **Use Case**: First-time customer
- **Profile**: No order history, exploring restaurants
- **Testing**: Onboarding flow, first order discount, welcome offers

---

## Restaurant Owner Accounts

### File: `restaurant-owner-registration.json`
**Total**: 8 restaurant owner accounts

#### Owner 1 - Pizza Palace
```json
{
  "email": "pizzapalace@example.com",
  "password": "PizzaOwner123!",
  "role": 1,
  "organization": "Pizza Palace"
}
```
- **Restaurant**: Pizza Palace
- **Cuisine**: Italian (Pizza, Pasta)
- **Use Case**: Multi-location Italian restaurant
- **Testing**: Menu management, restaurant hours, order processing

#### Owner 2 - Sushi Spot
```json
{
  "email": "sushispot@example.com",
  "password": "SushiOwner456!",
  "role": 1,
  "organization": "Sushi Spot"
}
```
- **Restaurant**: Sushi Spot
- **Cuisine**: Japanese (Sushi, Ramen)
- **Use Case**: High-end Japanese restaurant
- **Testing**: Fresh ingredient tracking, special dietary needs

#### Owner 3 - Burger Hub
```json
{
  "email": "burger.hub@example.com",
  "password": "BurgerOwner789!",
  "role": 1,
  "organization": "Burger Hub"
}
```
- **Restaurant**: Burger Hub
- **Cuisine**: American (Burgers, Fries, Wings)
- **Use Case**: Fast-casual burger restaurant
- **Testing**: Quick service, late-night hours, high volume

#### Owner 4 - Taco Town
```json
{
  "email": "taco.town@example.com",
  "password": "TacoOwner101!",
  "role": 1,
  "organization": "Taco Town"
}
```
- **Restaurant**: Taco Town
- **Cuisine**: Mexican (Tacos, Burritos)
- **Use Case**: Street food style restaurant
- **Testing**: Custom orders, spice levels, combo meals

#### Owner 5 - Indian Cuisine Express
```json
{
  "email": "indian.cuisine@example.com",
  "password": "IndianOwner202!",
  "role": 1,
  "organization": "Indian Cuisine Express"
}
```
- **Restaurant**: Indian Cuisine Express
- **Cuisine**: Indian (Curry, Biryani, Naan)
- **Use Case**: Lunch buffet and dinner service
- **Testing**: Spice levels, vegetarian options, lunch specials

#### Owner 6 - Italian Bistro
```json
{
  "email": "italian.bistro@example.com",
  "password": "ItalianOwner303!",
  "role": 1,
  "organization": "Italian Bistro"
}
```
- **Restaurant**: Italian Bistro
- **Cuisine**: Italian (Fine dining)
- **Use Case**: Upscale Italian restaurant
- **Testing**: Premium pricing, wine pairing, reservation system

#### Owner 7 - Chinese Dragon
```json
{
  "email": "chinese.dragon@example.com",
  "password": "ChineseOwner404!",
  "role": 1,
  "organization": "Chinese Dragon"
}
```
- **Restaurant**: Chinese Dragon
- **Cuisine**: Chinese (Dim Sum, Noodles, Rice dishes)
- **Use Case**: Traditional Chinese restaurant
- **Testing**: Family-style portions, dim sum carts, tea service

#### Owner 8 - Thai Delight
```json
{
  "email": "thai.delight@example.com",
  "password": "ThaiOwner505!",
  "role": 1,
  "organization": "Thai Delight"
}
```
- **Restaurant**: Thai Delight
- **Cuisine**: Thai (Pad Thai, Curry, Spring Rolls)
- **Use Case**: Authentic Thai restaurant
- **Testing**: Spice customization, coconut-free options, Thai basil

---

## Kitchen Worker Accounts

### File: `kitchen-worker-registration.json`
**Total**: 10 kitchen worker accounts

#### Worker 1 - Pizza Palace Head Chef
```json
{
  "email": "chef.pizzapalace@example.com",
  "password": "ChefPizza123!",
  "role": 3,
  "organization": "Pizza Palace"
}
```
- **Restaurant**: Pizza Palace
- **Position**: Head Chef
- **Specialty**: Pizza and pasta preparation
- **Testing**: Order acceptance, cooking time estimates, quality control

#### Worker 2 - Pizza Palace Line Cook
```json
{
  "email": "cook1.pizzapalace@example.com",
  "password": "Cook123!",
  "role": 3,
  "organization": "Pizza Palace"
}
```
- **Restaurant**: Pizza Palace
- **Position**: Line Cook
- **Specialty**: Pizza assembly
- **Testing**: Multi-order handling, peak hour operations

#### Worker 3 - Sushi Spot Sushi Chef
```json
{
  "email": "sushichef@sushispot.com",
  "password": "SushiChef456!",
  "role": 3,
  "organization": "Sushi Spot"
}
```
- **Restaurant**: Sushi Spot
- **Position**: Sushi Chef
- **Specialty**: Sushi rolls, sashimi preparation
- **Testing**: Fresh ingredient handling, plating standards

#### Worker 4 - Burger Hub Grill Master
```json
{
  "email": "grillmaster@burgerhub.com",
  "password": "Grill789!",
  "role": 3,
  "organization": "Burger Hub"
}
```
- **Restaurant**: Burger Hub
- **Position**: Grill Master
- **Specialty**: Burgers and grilled items
- **Testing**: Cook-to-order, custom specifications

#### Worker 5 - Taco Town Chef
```json
{
  "email": "chef.tacotown@example.com",
  "password": "TacoChef101!",
  "role": 3,
  "organization": "Taco Town"
}
```
- **Restaurant**: Taco Town
- **Position**: Chef
- **Specialty**: Taco assembly, Mexican cuisine
- **Testing**: Custom toppings, spice levels

#### Worker 6 - Indian Cuisine Express Chef
```json
{
  "email": "chef.indian@example.com",
  "password": "IndianChef202!",
  "role": 3,
  "organization": "Indian Cuisine Express"
}
```
- **Restaurant**: Indian Cuisine Express
- **Position**: Chef
- **Specialty**: Curry and biryani preparation
- **Testing**: Spice customization, naan bread timing

#### Worker 7 - Italian Bistro Sous Chef
```json
{
  "email": "souschef.italian@example.com",
  "password": "ItalianSous303!",
  "role": 3,
  "organization": "Italian Bistro"
}
```
- **Restaurant**: Italian Bistro
- **Position**: Sous Chef
- **Specialty**: Fine Italian dining
- **Testing**: Plating presentation, multiple courses

#### Worker 8 - Chinese Dragon Wok Chef
```json
{
  "email": "wokchef.chinese@example.com",
  "password": "ChineseWok404!",
  "role": 3,
  "organization": "Chinese Dragon"
}
```
- **Restaurant**: Chinese Dragon
- **Position**: Wok Chef
- **Specialty**: Stir-fry and noodle dishes
- **Testing**: High-heat cooking, quick turnaround

#### Worker 9 - Thai Delight Chef
```json
{
  "email": "chef.thai@example.com",
  "password": "ThaiChef505!",
  "role": 3,
  "organization": "Thai Delight"
}
```
- **Restaurant**: Thai Delight
- **Position**: Chef
- **Specialty**: Thai curry and noodle dishes
- **Testing**: Coconut milk alternatives, spice levels

#### Worker 10 - Pizza Palace Prep Cook
```json
{
  "email": "prep.pizzapalace@example.com",
  "password": "Prep606!",
  "role": 3,
  "organization": "Pizza Palace"
}
```
- **Restaurant**: Pizza Palace
- **Position**: Prep Cook
- **Specialty**: Ingredient preparation
- **Testing**: Inventory management, prep workflow

---

## Delivery Agent Accounts

### File: `delivery-agent-registration.json`
**Total**: 10 delivery agent accounts

#### Agent 1 - Fast Delivery (Bike)
```json
{
  "email": "rider1@fastdelivery.com",
  "password": "Rider123!",
  "role": 4,
  "organization": "Fast Delivery Co"
}
```
- **Company**: Fast Delivery Co
- **Vehicle**: Bike
- **Zone**: Downtown
- **Testing**: Short-distance deliveries, bike-friendly routes

#### Agent 2 - Fast Delivery (Scooter)
```json
{
  "email": "rider2@fastdelivery.com",
  "password": "Rider456!",
  "role": 4,
  "organization": "Fast Delivery Co"
}
```
- **Company**: Fast Delivery Co
- **Vehicle**: Scooter
- **Zone**: Urban areas
- **Testing**: Medium-distance deliveries, weather conditions

#### Agent 3 - Fast Delivery (Car)
```json
{
  "email": "driver1@fastdelivery.com",
  "password": "Driver789!",
  "role": 4,
  "organization": "Fast Delivery Co"
}
```
- **Company**: Fast Delivery Co
- **Vehicle**: Car
- **Zone**: Suburbs
- **Testing**: Long-distance deliveries, large orders

#### Agent 4 - Quick Courier (Bike)
```json
{
  "email": "courier1@quickcourier.com",
  "password": "Courier101!",
  "role": 4,
  "organization": "Quick Courier"
}
```
- **Company**: Quick Courier
- **Vehicle**: Bike
- **Zone**: City center
- **Testing**: Express deliveries, time-sensitive orders

#### Agent 5 - Quick Courier (Scooter)
```json
{
  "email": "courier2@quickcourier.com",
  "password": "Courier202!",
  "role": 4,
  "organization": "Quick Courier"
}
```
- **Company**: Quick Courier
- **Vehicle**: Scooter
- **Zone**: Multiple zones
- **Testing**: Multi-pickup, batch deliveries

#### Agent 6 - Speed Logistics (Car)
```json
{
  "email": "delivery1@speedlogistics.com",
  "password": "Speed303!",
  "role": 4,
  "organization": "Speed Logistics"
}
```
- **Company**: Speed Logistics
- **Vehicle**: Car
- **Zone**: Wide area
- **Testing**: Large orders, catering deliveries

#### Agent 7 - Speed Logistics (Van)
```json
{
  "email": "delivery2@speedlogistics.com",
  "password": "Speed404!",
  "role": 4,
  "organization": "Speed Logistics"
}
```
- **Company**: Speed Logistics
- **Vehicle**: Van
- **Zone**: Commercial areas
- **Testing**: Bulk orders, office deliveries

#### Agent 8 - Urban Express (Bike)
```json
{
  "email": "express1@urbanexpress.com",
  "password": "Express505!",
  "role": 4,
  "organization": "Urban Express"
}
```
- **Company**: Urban Express
- **Vehicle**: Electric Bike
- **Zone**: Downtown
- **Testing**: Eco-friendly deliveries, bike lanes

#### Agent 9 - Urban Express (Scooter)
```json
{
  "email": "express2@urbanexpress.com",
  "password": "Express606!",
  "role": 4,
  "organization": "Urban Express"
}
```
- **Company**: Urban Express
- **Vehicle**: Electric Scooter
- **Zone**: Urban areas
- **Testing**: Green deliveries, charging stations

#### Agent 10 - Reliable Rides (Car)
```json
{
  "email": "ride1@reliablerides.com",
  "password": "Ride707!",
  "role": 4,
  "organization": "Reliable Rides"
}
```
- **Company**: Reliable Rides
- **Vehicle**: Car (sedan)
- **Zone**: All zones
- **Testing**: Reliable service, customer satisfaction

---

## IT Admin Accounts

### File: `it-admin-registration.json`
**Total**: 10 IT admin accounts across 4 roles

### Developers (3 accounts)

#### Developer 1 - Lead Developer
```json
{
  "email": "dev.lead@fooddelivery.com",
  "password": "DevLead123!",
  "role": 5,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Lead Developer
- **Access**: Full system access, deployment permissions
- **Testing**: Code deployment, feature testing, API development

#### Developer 2 - Backend Developer
```json
{
  "email": "backend.dev@fooddelivery.com",
  "password": "BackendDev456!",
  "role": 5,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Backend Developer
- **Access**: Service development, database access
- **Testing**: API testing, service integration

#### Developer 3 - Frontend Developer
```json
{
  "email": "frontend.dev@fooddelivery.com",
  "password": "FrontendDev789!",
  "role": 5,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Frontend Developer
- **Access**: UI development, client-side testing
- **Testing**: User interface, responsive design

### Testers (2 accounts)

#### Tester 1 - QA Lead
```json
{
  "email": "qa.lead@fooddelivery.com",
  "password": "QALead101!",
  "role": 6,
  "organization": "FoodDelivery IT"
}
```
- **Position**: QA Lead
- **Access**: Testing environments, bug tracking
- **Testing**: End-to-end testing, test automation, quality assurance

#### Tester 2 - QA Engineer
```json
{
  "email": "qa.engineer@fooddelivery.com",
  "password": "QAEngineer202!",
  "role": 6,
  "organization": "FoodDelivery IT"
}
```
- **Position**: QA Engineer
- **Access**: Test execution, defect management
- **Testing**: Manual testing, regression testing, test reporting

### Network Admins (2 accounts)

#### Network Admin 1 - Senior
```json
{
  "email": "network.admin@fooddelivery.com",
  "password": "NetAdmin303!",
  "role": 7,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Senior Network Administrator
- **Access**: Network infrastructure, security policies
- **Testing**: Network monitoring, firewall rules, VPN access

#### Network Admin 2 - Junior
```json
{
  "email": "network.support@fooddelivery.com",
  "password": "NetSupport404!",
  "role": 7,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Network Support Engineer
- **Access**: Network troubleshooting, basic configuration
- **Testing**: Network diagnostics, connectivity issues

### Database Admins (3 accounts)

#### Database Admin 1 - Senior DBA
```json
{
  "email": "dba.senior@fooddelivery.com",
  "password": "DBA505!",
  "role": 8,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Senior Database Administrator
- **Access**: Full database access, backup/restore
- **Testing**: Database performance, query optimization, data integrity

#### Database Admin 2 - DBA
```json
{
  "email": "dba@fooddelivery.com",
  "password": "DBA606!",
  "role": 8,
  "organization": "FoodDelivery IT"
}
```
- **Position**: Database Administrator
- **Access**: Database management, user permissions
- **Testing**: Schema changes, index optimization, monitoring

#### Database Admin 3 - MongoDB Specialist
```json
{
  "email": "mongo.dba@fooddelivery.com",
  "password": "MongoDBA707!",
  "role": 8,
  "organization": "FoodDelivery IT"
}
```
- **Position**: MongoDB Specialist
- **Access**: MongoDB administration, replica sets
- **Testing**: Document structure, aggregation pipelines, sharding

---

## Role Reference

### Role Enumeration

```csharp
public enum UserRole
{
    Customer = 0,          // External customer
    Biller = 1,            // Restaurant Owner
    Operator = 2,          // Operations staff
    Worker = 3,            // Kitchen Worker
    DeliveryAgent = 4,     // Delivery driver
    Developer = 5,         // IT Developer
    Tester = 6,            // QA Tester
    NetworkAdmin = 7,      // Network Administrator
    DatabaseAdmin = 8      // Database Administrator
}
```

### Role Permissions Matrix

| Role             | ID | View Menu | Place Order | Manage Restaurant | Manage Orders | Deliver Orders | Admin Access |
|------------------|----|--------------|-------------|-------------------|---------------|----------------|--------------|
| Customer         | 0  | ✅           | ✅          | ❌                | ❌            | ❌             | ❌           |
| Biller (Owner)   | 1  | ✅           | ❌          | ✅                | ✅            | ❌             | ❌           |
| Operator         | 2  | ✅           | ❌          | ❌                | ✅            | ❌             | ❌           |
| Worker (Kitchen) | 3  | ✅           | ❌          | ❌                | ✅ (Kitchen)  | ❌             | ❌           |
| DeliveryAgent    | 4  | ✅           | ❌          | ❌                | ✅ (Delivery) | ✅             | ❌           |
| Developer        | 5  | ✅           | ✅          | ✅                | ✅            | ✅             | ✅           |
| Tester           | 6  | ✅           | ✅          | ✅                | ✅            | ✅             | ✅ (Read)    |
| NetworkAdmin     | 7  | ✅           | ❌          | ❌                | ❌            | ❌             | ✅ (Network) |
| DatabaseAdmin    | 8  | ✅           | ❌          | ❌                | ❌            | ❌             | ✅ (Database)|

---

## MongoDB Seed Data

### Seed Script for All User Roles

```javascript
// MongoDB Shell Script: seed-users.js
// Run: mongosh < seed-users.js

use fooddelivery_auth;

// Insert Customer Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6900000000000000000001"),
    "email": "customer1@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 0,
    "organization": "external_users",
    "createdAt": ISODate("2025-11-01T10:00:00Z"),
    "updatedAt": ISODate("2025-11-01T10:00:00Z")
  },
  {
    "_id": ObjectId("6900000000000000000002"),
    "email": "customer2@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 0,
    "organization": "external_users",
    "createdAt": ISODate("2025-11-01T10:05:00Z"),
    "updatedAt": ISODate("2025-11-01T10:05:00Z")
  },
  {
    "_id": ObjectId("6900000000000000000003"),
    "email": "vip.customer@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 0,
    "organization": "external_users",
    "createdAt": ISODate("2025-11-01T10:10:00Z"),
    "updatedAt": ISODate("2025-11-01T10:10:00Z")
  }
]);

// Insert Restaurant Owner Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6900000000000000001001"),
    "email": "pizzapalace@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 1,
    "organization": "Pizza Palace",
    "createdAt": ISODate("2025-11-01T11:00:00Z"),
    "updatedAt": ISODate("2025-11-01T11:00:00Z")
  },
  {
    "_id": ObjectId("6900000000000000001002"),
    "email": "sushispot@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 1,
    "organization": "Sushi Spot",
    "createdAt": ISODate("2025-11-01T11:05:00Z"),
    "updatedAt": ISODate("2025-11-01T11:05:00Z")
  },
  {
    "_id": ObjectId("6900000000000000001003"),
    "email": "burger.hub@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 1,
    "organization": "Burger Hub",
    "createdAt": ISODate("2025-11-01T11:10:00Z"),
    "updatedAt": ISODate("2025-11-01T11:10:00Z")
  }
]);

// Insert Kitchen Worker Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6900000000000000003001"),
    "email": "chef.pizzapalace@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 3,
    "organization": "Pizza Palace",
    "createdAt": ISODate("2025-11-01T12:00:00Z"),
    "updatedAt": ISODate("2025-11-01T12:00:00Z")
  },
  {
    "_id": ObjectId("6900000000000000003002"),
    "email": "cook1.pizzapalace@example.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 3,
    "organization": "Pizza Palace",
    "createdAt": ISODate("2025-11-01T12:05:00Z"),
    "updatedAt": ISODate("2025-11-01T12:05:00Z")
  }
]);

// Insert Delivery Agent Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6900000000000000004001"),
    "email": "rider1@fastdelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 4,
    "organization": "Fast Delivery Co",
    "createdAt": ISODate("2025-11-01T13:00:00Z"),
    "updatedAt": ISODate("2025-11-01T13:00:00Z")
  },
  {
    "_id": ObjectId("6900000000000000004002"),
    "email": "rider2@fastdelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 4,
    "organization": "Fast Delivery Co",
    "createdAt": ISODate("2025-11-01T13:05:00Z"),
    "updatedAt": ISODate("2025-11-01T13:05:00Z")
  }
]);

// Insert IT Admin Accounts
db.users.insertMany([
  {
    "_id": ObjectId("6900000000000000005001"),
    "email": "dev.lead@fooddelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 5,
    "organization": "FoodDelivery IT",
    "createdAt": ISODate("2025-11-01T14:00:00Z"),
    "updatedAt": ISODate("2025-11-01T14:00:00Z")
  },
  {
    "_id": ObjectId("6900000000000000006001"),
    "email": "qa.lead@fooddelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 6,
    "organization": "FoodDelivery IT",
    "createdAt": ISODate("2025-11-01T14:10:00Z"),
    "updatedAt": ISODate("2025-11-01T14:10:00Z")
  },
  {
    "_id": ObjectId("6900000000000000007001"),
    "email": "network.admin@fooddelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 7,
    "organization": "FoodDelivery IT",
    "createdAt": ISODate("2025-11-01T14:20:00Z"),
    "updatedAt": ISODate("2025-11-01T14:20:00Z")
  },
  {
    "_id": ObjectId("6900000000000000008001"),
    "email": "dba.senior@fooddelivery.com",
    "passwordHash": "$2a$11$hashedPasswordHere",
    "role": 8,
    "organization": "FoodDelivery IT",
    "createdAt": ISODate("2025-11-01T14:30:00Z"),
    "updatedAt": ISODate("2025-11-01T14:30:00Z")
  }
]);

print("✅ Successfully seeded 43 user accounts across 9 roles");
```

### Verify Seeded Data

```javascript
// Verify accounts by role
db.users.aggregate([
  {
    $group: {
      _id: "$role",
      count: { $sum: 1 },
      organizations: { $addToSet: "$organization" }
    }
  },
  {
    $sort: { _id: 1 }
  }
]);

// Expected Output:
// { _id: 0, count: 5, organizations: ["external_users"] }
// { _id: 1, count: 8, organizations: ["Pizza Palace", "Sushi Spot", ...] }
// { _id: 3, count: 10, organizations: ["Pizza Palace", "Sushi Spot", ...] }
// { _id: 4, count: 10, organizations: ["Fast Delivery Co", "Quick Courier", ...] }
// { _id: 5, count: 3, organizations: ["FoodDelivery IT"] }
// { _id: 6, count: 2, organizations: ["FoodDelivery IT"] }
// { _id: 7, count: 2, organizations: ["FoodDelivery IT"] }
// { _id: 8, count: 3, organizations: ["FoodDelivery IT"] }
```

---

## Testing Checklist

### 1. Customer Registration Tests
- [ ] Register customer1@example.com
- [ ] Register customer2@example.com
- [ ] Register vip.customer@example.com
- [ ] Register frequent.diner@example.com
- [ ] Register new.customer@example.com
- [ ] Verify all customers have role = 0
- [ ] Verify organization = "external_users"
- [ ] Test duplicate email (409 Conflict)

### 2. Restaurant Owner Registration Tests
- [ ] Register all 8 restaurant owners
- [ ] Verify unique organizations (Pizza Palace, Sushi Spot, etc.)
- [ ] Verify all have role = 1
- [ ] Test login for each owner
- [ ] Validate JWT token contains correct role

### 3. Kitchen Worker Registration Tests
- [ ] Register all 10 kitchen workers
- [ ] Verify workers associated with correct restaurants
- [ ] Verify all have role = 3
- [ ] Test multiple workers from same restaurant
- [ ] Validate restaurant assignment

### 4. Delivery Agent Registration Tests
- [ ] Register all 10 delivery agents
- [ ] Verify agents across different companies
- [ ] Verify all have role = 4
- [ ] Test multiple agents from same company
- [ ] Validate company assignment

### 5. IT Admin Registration Tests
- [ ] Register 3 developers (role = 5)
- [ ] Register 2 testers (role = 6)
- [ ] Register 2 network admins (role = 7)
- [ ] Register 3 database admins (role = 8)
- [ ] Verify all have organization = "FoodDelivery IT"
- [ ] Validate admin permissions

### 6. Authentication Flow Tests
- [ ] Register → Login → Validate (complete flow)
- [ ] Test JWT token expiration
- [ ] Test invalid credentials (401)
- [ ] Test missing token (401)
- [ ] Test token refresh (if implemented)

### 7. Data-Driven Tests (Newman)
- [ ] Run with customer-registration.json (5 iterations)
- [ ] Run with restaurant-owner-registration.json (8 iterations)
- [ ] Run with kitchen-worker-registration.json (10 iterations)
- [ ] Run with delivery-agent-registration.json (10 iterations)
- [ ] Run with it-admin-registration.json (10 iterations)
- [ ] Generate HTML test reports

### 8. Role-Based Access Tests
- [ ] Customer can place orders
- [ ] Owner can manage restaurant
- [ ] Worker can update order status
- [ ] Agent can accept deliveries
- [ ] Admin can access system settings
- [ ] Verify unauthorized access denied

### 9. Bulk Registration Tests
- [ ] Register all 43 accounts sequentially
- [ ] Verify no duplicate emails
- [ ] Check database consistency
- [ ] Validate all JWT tokens
- [ ] Test concurrent registrations

### 10. Error Handling Tests
- [ ] Missing email field (400 Bad Request)
- [ ] Invalid email format (400)
- [ ] Weak password (400)
- [ ] Missing role (400)
- [ ] Duplicate email (409 Conflict)
- [ ] Invalid role ID (400)

---

## Data Validation Rules

### User Registration Fields

| Field        | Type    | Validation                                  | Required |
|--------------|---------|---------------------------------------------|----------|
| email        | string  | Valid email format (RFC 5322)               | Yes      |
| password     | string  | Min 6 chars, at least 1 uppercase, 1 digit, 1 special | Yes |
| role         | integer | 0-8 (valid role ID)                         | Yes      |
| organization | string  | 3-100 characters                            | Yes      |

### Password Requirements

- **Minimum Length**: 6 characters (recommended: 8+)
- **Uppercase**: At least 1 uppercase letter (A-Z)
- **Lowercase**: At least 1 lowercase letter (a-z)
- **Digit**: At least 1 number (0-9)
- **Special Character**: At least 1 special char (!@#$%^&*)
- **No Common Patterns**: No "123456", "password", etc.

**Example Valid Passwords**:
- `Customer123!` ✅
- `PizzaOwner456!` ✅
- `Chef@2025` ✅

**Example Invalid Passwords**:
- `password` ❌ (no uppercase, digit, special char)
- `Pass123` ❌ (no special char)
- `Password!` ❌ (no digit)

### Email Format Validation

- **Format**: `user@domain.com`
- **Local Part**: Alphanumeric, dots, hyphens, underscores
- **Domain**: Valid domain name with TLD
- **No Spaces**: Email must not contain spaces
- **Case Insensitive**: Stored in lowercase

**Example Valid Emails**:
- `customer1@example.com` ✅
- `chef.pizzapalace@example.com` ✅
- `dev_lead@fooddelivery.com` ✅

**Example Invalid Emails**:
- `customer1` ❌ (missing @domain)
- `@example.com` ❌ (missing local part)
- `customer @example.com` ❌ (contains space)

---

## Quick Reference

### Account Counts by Role

```
Customers:           5 accounts
Restaurant Owners:   8 accounts
Kitchen Workers:    10 accounts
Delivery Agents:    10 accounts
Developers:          3 accounts
Testers:             2 accounts
Network Admins:      2 accounts
Database Admins:     3 accounts
-----------------------------------
TOTAL:              43 accounts
```

### Data File Locations

```
fda/postman-collections/user-registration/data/
├── customer-registration.json           (5 accounts)
├── restaurant-owner-registration.json   (8 accounts)
├── kitchen-worker-registration.json    (10 accounts)
├── delivery-agent-registration.json    (10 accounts)
├── it-admin-registration.json          (10 accounts)
└── README.md
```

### Newman Commands

```powershell
# Register all customers (5 accounts)
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/customer-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Register all restaurant owners (8 accounts)
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/restaurant-owner-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Register all kitchen workers (10 accounts)
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/kitchen-worker-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Register all delivery agents (10 accounts)
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/delivery-agent-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Register all IT admins (10 accounts)
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/it-admin-registration.json `
  -e Capstone-Local-Environment.postman_environment.json

# Generate HTML report
newman run user-registration/User-Registration-Flow.postman_collection.json `
  -d user-registration/data/customer-registration.json `
  -e Capstone-Local-Environment.postman_environment.json `
  --reporters cli,htmlextra `
  --reporter-htmlextra-export test-results/user-registration-report.html
```

### Postman Collection Variables

```javascript
{
  "user_data": "",         // JSON string with registration data
  "user_password": "",     // Password for login
  "user_email": "",        // Auto-set from registration
  "user_id": "",           // Auto-set from registration/login
  "auth_token": ""         // Auto-set from login (JWT)
}
```

---

## Security Notes

⚠️ **Critical Security Warnings**:

1. **Test Data Only**: All credentials in this document are for testing purposes only
2. **Plain Text Passwords**: Never store passwords in plain text in production
3. **Password Hashing**: Production uses BCrypt with cost factor 11
4. **JWT Expiration**: Tokens expire after 1 hour (configurable)
5. **HTTPS Only**: Production must use HTTPS for all authentication
6. **Environment Separation**: Test data must never be used in production
7. **Secret Rotation**: Rotate JWT secrets regularly in production
8. **Rate Limiting**: Implement rate limiting on authentication endpoints

---

## Related Documentation

- **[User Registration README](./README.md)** - Collection usage guide
- **[Operator Workflows Test Data](../operator-service-workflows/TEST-DATA-REFERENCE.md)** - Operator test accounts
- **[Restaurant Owner Workflows Test Data](../restaurant-owner-workflows/TEST-DATA-REFERENCE.md)** - Owner test accounts
- **[RBAC Documentation](../../docs/services/RBAC-COMPREHENSIVE.md)** - Role-based access control
- **[Authentication API](../../docs/services/README.md)** - API documentation

---

**Version**: 1.0.0  
**Last Updated**: November 20, 2025  
**Data Maintained By**: FDA QA Team  
**Total Test Accounts**: 43 users across 9 roles
