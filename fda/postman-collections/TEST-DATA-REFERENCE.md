# Food Delivery App - Postman Test Data Reference

This document provides realistic test data scenarios used in the Postman collections for comprehensive food delivery app testing.

## User Personas

### 1. Sarah Martinez - Busy Professional
- **Email**: sarah.martinez@gmail.com
- **Password**: FoodLover2024!
- **Profile**: Marketing manager who frequently orders healthy meals
- **Ordering Behavior**: Health-conscious choices, vegetarian options, premium quality
- **Customer Type**: Premium

### 2. Emily Chen - Creative Professional
- **Email**: emily.chen@gmail.com
- **Phone**: +1-415-555-0187
- **Address**: 2847 Fillmore Street, Apt 4B, San Francisco, CA 94115
- **Profile**: UX Designer who works late and orders dinner frequently
- **Ordering Behavior**: Vegetarian meals, Asian cuisine, healthy options
- **Customer Type**: Premium
- **Special Notes**: Vegetarian with nut allergy, prefers eco-friendly restaurants

### 3. Alex Johnson - Busy Professional
- **Email**: alex.johnson@gmail.com
- **Password**: FoodieLife2024!
- **Profile**: Marketing executive with hectic schedule
- **Ordering Behavior**: Quick delivery, comfort food after long work days
- **Customer Type**: Regular

### 4. Michael Rodriguez - Team Leader
- **Email**: mike.rodriguez@gmail.com
- **Phone**: +1-512-555-0198
- **Address**: 1204 South Lamar Boulevard, Unit 15, Austin, TX 78704
- **Profile**: Software engineer who orders lunch for small team
- **Ordering Behavior**: Bulk orders, spicy food, office catering
- **Customer Type**: Business

### 5. Jessica Thompson - Graduate Student
- **Email**: jess.thompson@student.university.edu
- **Phone**: +1-617-555-0156
- **Address**: 45 Beacon Hill Street, Apt 3B, Boston, MA 02108
- **Profile**: MIT graduate student on tight budget
- **Ordering Behavior**: Budget-conscious, vegan options, late-night study meals
- **Customer Type**: Student

### 6. David Kim - Creative Director
- **Email**: david.kim@creativestudio.com
- **Phone**: +1-213-555-0167
- **Address**: 8765 Sunset Boulevard, Suite 200, West Hollywood, CA 90069
- **Profile**: Creative director who orders premium meals for client meetings
- **Ordering Behavior**: High-end restaurants, sushi, fine dining delivery
- **Customer Type**: Premium

## Restaurant Menu Catalog

### Pizza
1. **Margherita Pizza (Large)**
   - **SKU**: PIZZA-MARG-LG
   - **Price**: $18.99
   - **Restaurant**: Tony's Authentic Pizzeria
   - **Prep Time**: 15 minutes
   - **Dietary**: Vegetarian, Contains Gluten, Contains Dairy

### Indian Cuisine
1. **Chicken Tikka Masala**
   - **SKU**: CURRY-TIKKA-REG
   - **Price**: $16.95
   - **Restaurant**: Spice Garden Indian Cuisine
   - **Prep Time**: 20 minutes
   - **Dietary**: Contains Dairy, Mild Spice

### Sushi
1. **California Sushi Roll (8 pieces)**
   - **SKU**: SUSHI-CALIF-8PC
   - **Price**: $12.50
   - **Restaurant**: Sakura Sushi Bar
   - **Prep Time**: 12 minutes
   - **Dietary**: Contains Seafood, Gluten-Free

### American
1. **Classic Beef Burger & Fries**
   - **SKU**: BURGER-BEEF-COMBO
   - **Price**: $14.75
   - **Restaurant**: Burger Junction
   - **Prep Time**: 18 minutes
   - **Dietary**: Contains Gluten, Contains Dairy

### Thai
1. **Pad Thai with Shrimp**
   - **SKU**: THAI-PADTHAI-SHRIMP
   - **Price**: $15.25
   - **Restaurant**: Bangkok Street Kitchen
   - **Prep Time**: 16 minutes
   - **Dietary**: Contains Shellfish, Contains Peanuts, Medium Spice

### Healthy Options
1. **Mediterranean Quinoa Bowl**
   - **SKU**: BOWL-MED-QUINOA
   - **Price**: $13.95
   - **Restaurant**: Fresh & Green Cafe
   - **Prep Time**: 10 minutes
   - **Dietary**: Gluten-Free, Contains Dairy, High Protein

### Desserts
1. **Chocolate Lava Cake**
   - **SKU**: DESSERT-CHOC-LAVA
   - **Price**: $8.95
   - **Restaurant**: Sweet Endings Bakery
   - **Prep Time**: 8 minutes
   - **Dietary**: Contains Gluten, Contains Dairy, Contains Eggs

## Realistic Food Delivery Scenarios

### Scenario 1: Family Dinner Order
**Customer**: Emily Chen (Creative Professional)
**Items**:
- Margherita Pizza (Large) ($18.99)
- Chicken Tikka Masala x2 ($33.90)
- Chocolate Lava Cake ($8.95)
**Subtotal**: $61.84
**Delivery Fee**: $3.99
**Tip**: $12.36
**Total**: $86.26
**Use Case**: Multi-restaurant family dinner order

### Scenario 2: Office Lunch Catering
**Customer**: Michael Rodriguez (Team Leader)
**Items**:
- Pad Thai with Shrimp x3 ($45.75)
- California Sushi Rolls x2 ($25.00)
- Mediterranean Quinoa Bowls x2 ($27.90)
**Subtotal**: $98.65
**Business Delivery**: $5.99
**Total**: $115.42
**Use Case**: Team lunch order for small office

### Scenario 3: Student Budget Meal
**Customer**: Jessica Thompson (Graduate Student)
**Items**:
- Mediterranean Quinoa Bowl ($13.95)
- Student discount: -$2.00
**Subtotal**: $11.95
**Delivery Fee**: $2.49
**Tip**: $2.00
**Total**: $16.44
**Use Case**: Healthy budget meal for late-night studying

## Payment Test Data

### Valid Credit Cards (Test Environment)
1. **Visa**: 4532015112830366
   - **Expiry**: 08/2027
   - **CVV**: 847
   - **Name**: Emily Chen

2. **Mastercard**: 5555555555554444
   - **Expiry**: 12/2026
   - **CVV**: 123
   - **Name**: Michael Rodriguez

3. **American Express**: 378282246310005
   - **Expiry**: 04/2027
   - **CVV**: 1234
   - **Name**: David Kim

### Digital Payment Options
- **PayPal**: Integrated for quick checkout
- **Apple Pay**: Mobile app integration
- **Google Pay**: Android app support
- **Venmo**: Social payment option for splitting bills

### Delivery Addresses
All delivery addresses match the customer profiles above. Include apartment numbers, building codes, and specific delivery instructions for realistic testing scenarios.

## Delivery Scenarios

### Premium Family Order
- **Customer**: Premium customer (David Kim)
- **Items**: Fine dining selections from multiple restaurants
- **Delivery**: Expedited (under 30 minutes)
- **Special Instructions**: Coordinate timing, contact upon arrival
- **Total**: $75-$150

### Business Catering Order
- **Customer**: Business customer (Michael Rodriguez)
- **Items**: Lunch for team (5-10 people)
- **Delivery**: Scheduled delivery to office
- **Payment**: Corporate account billing
- **Total**: $100-$200

### Student Budget Order
- **Customer**: Student customer (Jessica Thompson)
- **Items**: Single healthy meal
- **Delivery**: Standard delivery
- **Discounts**: Student pricing, promo codes
- **Total**: $10-$25

### Late-Night Order
- **Customer**: Any customer type
- **Items**: Comfort food, desserts
- **Delivery**: After 10 PM
- **Special Notes**: Quiet delivery, contactless drop-off
- **Total**: $15-$40

## Environment-Specific Data

### Local Development
- **Base URLs**: localhost with NodePort (30001-30006)
- **MongoDB**: Shared instance
- **Test Users**: Full access to all test accounts
- **Payment Processing**: Sandbox mode
- **Delivery**: Simulated delivery times and tracking

### Production Testing
- **Base URLs**: Internal Kubernetes services
- **MongoDB**: Dedicated per service
- **Test Users**: Limited test accounts
- **Payment Processing**: Staging environment
- **Delivery**: Real-time integration with delivery partners

## Usage Notes

1. **Data Consistency**: All test data is interconnected and references real restaurant menus and food items
2. **Realistic Pricing**: Food prices reflect actual market values for authentic delivery app testing
3. **Geographic Diversity**: Delivery addresses span multiple US cities for delivery zone testing
4. **Customer Segmentation**: Different customer types enable various business logic testing (Premium, Student, Business)
5. **Payment Variety**: Multiple payment methods and scenarios including tips and delivery fees
6. **Dietary Restrictions**: Comprehensive allergy and dietary preference testing
7. **Restaurant Partners**: Multiple cuisine types and restaurant ratings for variety
8. **Delivery Logistics**: Real preparation times, delivery windows, and coordination scenarios

This test data enables realistic end-to-end testing of the entire food delivery platform while maintaining consistency across all microservices including restaurant management, order coordination, delivery tracking, and customer satisfaction metrics.