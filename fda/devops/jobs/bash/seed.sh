#!/bin/bash#!/bin/bash

# Sample Data Seeding Script for Food Delivery Application# Sample Data Seeding Script for Food Delivery Application

# This script seeds the system with sample restaurants, menu items, and user profiles via API calls# This script seeds the system with sample restaurants, menu items, and user profiles via API calls



set -e

GATEWAY_URL="${1:-http://localhost:30500}"

# Generate unique credentials for temporary admin user

TEMP_ADMIN_EMAIL="seed-admin-$(date +%Y%m%d%H%M%S)@temp.local"TEMP_ADMIN_EMAIL="seed-admin-$(date +%Y%m%d%H%M%S)@temp.local"

TEMP_ADMIN_PASSWORD="TempSeed$((RANDOM % 9000 + 1000))!"TEMP_ADMIN_PASSWORD="TempSeed$((RANDOM % 9000 + 1000))!"

TEMP_ADMIN_USER_ID=""TEMP_ADMIN_USER_ID=""

AUTH_TOKEN=""AUTH_TOKEN=""



# Function to create temporary admin user# Function to create temporary admin user

create_temporary_admin_user() {create_temporary_admin_user() {

    echo "👤 Creating temporary admin user: $TEMP_ADMIN_EMAIL"    echo "👤 Creating temporary admin user: $TEMP_ADMIN_EMAIL"

        

    REGISTER_BODY=$(cat <<EOF    REGISTER_BODY=$(cat <<EOF

{{

    "email": "$TEMP_ADMIN_EMAIL",    "email": "$TEMP_ADMIN_EMAIL",

    "password": "$TEMP_ADMIN_PASSWORD",    "password": "$TEMP_ADMIN_PASSWORD",

    "role": 2,    "role": 2,

    "organization": "SeedingScript"    "organization": "SeedingScript"

}}

EOFEOF

))

        

    RESPONSE=$(curl -s -X POST "$GATEWAY_URL/api/auth/register" \    RESPONSE=$(curl -s -X POST "$GATEWAY_URL/api/auth/register" \

        -H "Content-Type: application/json" \        -H "Content-Type: application/json" \

        -d "$REGISTER_BODY" || echo "")        -d "$REGISTER_BODY" || echo "")

        

    if [ -n "$RESPONSE" ]; then    if [ -n "$RESPONSE" ]; then

        TEMP_ADMIN_USER_ID=$(echo "$RESPONSE" | grep -o '"id":"[^"]*"' | cut -d'"' -f4)        TEMP_ADMIN_USER_ID=$(echo "$RESPONSE" | grep -o '"id":"[^"]*"' | cut -d'"' -f4)

        if [ -n "$TEMP_ADMIN_USER_ID" ]; then        if [ -n "$TEMP_ADMIN_USER_ID" ]; then

            echo "✓ Successfully created temporary admin user"            echo "✓ Successfully created temporary admin user"

            return 0            return 0

        fi        fi

    fi    fi

        

    echo "✗ Failed to create admin user"    echo "✗ Failed to create admin user"

    return 1    return 1

}}



# Function to get JWT token for admin user# Function to get JWT token for admin user

get_admin_auth_token() {get_admin_auth_token() {

    echo "🔐 Logging in as admin user..."    echo "🔐 Logging in as admin user..."

        

    LOGIN_BODY=$(cat <<EOF    LOGIN_BODY=$(cat <<EOF

{{

    "email": "$TEMP_ADMIN_EMAIL",    "email": "$TEMP_ADMIN_EMAIL",

    "password": "$TEMP_ADMIN_PASSWORD",    "password": "$TEMP_ADMIN_PASSWORD",

    "loginMethod": 0    "loginMethod": 0

}}

EOFEOF

))

        

    RESPONSE=$(curl -s -X POST "$GATEWAY_URL/api/auth/login" \    RESPONSE=$(curl -s -X POST "$GATEWAY_URL/api/auth/login" \

        -H "Content-Type: application/json" \        -H "Content-Type: application/json" \

        -d "$LOGIN_BODY" || echo "")        -d "$LOGIN_BODY" || echo "")

        

    if [ -n "$RESPONSE" ]; then    if [ -n "$RESPONSE" ]; then

        AUTH_TOKEN=$(echo "$RESPONSE" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)        AUTH_TOKEN=$(echo "$RESPONSE" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

        if [ -n "$AUTH_TOKEN" ]; then        if [ -n "$AUTH_TOKEN" ]; then

            echo "✓ Successfully obtained auth token"            echo "✓ Successfully obtained auth token"

            return 0            return 0

        fi        fi

    fi    fi

        

    echo "✗ Failed to get auth token"    echo "✗ Failed to get auth token"

    return 1    return 1

}}



# Function to delete temporary admin user# Function to delete temporary admin user

remove_temporary_admin_user() {remove_temporary_admin_user() {

    echo "🗑️  Deleting temporary admin user..."    echo "🗑️  Deleting temporary admin user..."

        

    curl -s -X DELETE "$GATEWAY_URL/api/auth/users/$TEMP_ADMIN_USER_ID" \    curl -s -X DELETE "$GATEWAY_URL/api/auth/users/$TEMP_ADMIN_USER_ID" \

        -H "Authorization: Bearer $AUTH_TOKEN" &>/dev/null || true        -H "Authorization: Bearer $AUTH_TOKEN" &>/dev/null || true

        

    echo "✓ Successfully deleted temporary admin user"    echo "✓ Successfully deleted temporary admin user"

}}



# Function to make API calls# Function to make API calls

invoke_api_call() {invoke_api_call() {

    local METHOD=$1    local METHOD=$1

    local ENDPOINT=$2    local ENDPOINT=$2

    local BODY=$3    local BODY=$3

        

    local URL="$GATEWAY_URL$ENDPOINT"    local URL="$GATEWAY_URL$ENDPOINT"

        

    if [ -n "$BODY" ]; then    if [ -n "$BODY" ]; then

        curl -s -X "$METHOD" "$URL" \        curl -s -X "$METHOD" "$URL" \

            -H "Content-Type: application/json" \            -H "Content-Type: application/json" \

            -H "Authorization: Bearer $AUTH_TOKEN" \            -H "Authorization: Bearer $AUTH_TOKEN" \

            -d "$BODY"            -d "$BODY"

    else    else

        curl -s -X "$METHOD" "$URL" \        curl -s -X "$METHOD" "$URL" \

            -H "Authorization: Bearer $AUTH_TOKEN"            -H "Authorization: Bearer $AUTH_TOKEN"

    fi    fi

}}



# Create temporary admin user# Create temporary admin user

echo ""echo ""

echo "========================================"echo "========================================"

echo "Setting up temporary admin account..."echo "Setting up temporary admin account..."

echo "========================================"echo "========================================"

echo ""echo ""



if ! create_temporary_admin_user; thenif ! create_temporary_admin_user; then

    echo "❌ Failed to create temporary admin user. Cannot proceed with seeding."    echo "❌ Failed to create temporary admin user. Cannot proceed with seeding."

    exit 1    exit 1

fifi



# Get auth token for admin user# Get auth token for admin user

if ! get_admin_auth_token; thenif ! get_admin_auth_token; then

    echo "❌ Failed to obtain auth token. Cannot proceed with seeding."    echo "❌ Failed to obtain auth token. Cannot proceed with seeding."

    remove_temporary_admin_user || true    remove_temporary_admin_user || true

    exit 1    exit 1

fifi



echo ""echo ""



echo "========================================"echo "========================================"

echo "Food Delivery App - Sample Data Seeder"echo "Food Delivery App - Sample Data Seeder"

echo "========================================"echo "========================================"

echo ""echo ""



echo "🔄 Starting data seeding process..."echo "🔄 Starting data seeding process..."

echo ""echo ""



# ============================================# ============================================

# STEP 1: Load Menu Items from JSON# STEP 1: Load Menu Items from JSON

# ============================================# ============================================

echo "📂 Loading seed data from JSON file..."echo "� Loading seed data from JSON file..."



SEED_DATA_PATH="$(dirname "$0")/../data/seed-menu-items.json"SEED_DATA_PATH="$(dirname "$0")/../data/seed-menu-items.json"

if [ ! -f "$SEED_DATA_PATH" ]; thenif [ ! -f "$SEED_DATA_PATH" ]; then

    echo "❌ Seed data file not found: $SEED_DATA_PATH"    echo "❌ Seed data file not found: $SEED_DATA_PATH"

    remove_temporary_admin_user || true    remove_temporary_admin_user || true

    exit 1    exit 1

fifi



# Check if jq is installed# Check if jq is installed

if ! command -v jq &> /dev/null; thenif ! command -v jq &> /dev/null; then

    echo "❌ Error: 'jq' command not found. Please install jq to parse JSON data."    echo "❌ Error: 'jq' command not found. Please install jq to parse JSON data."

    echo "   Install with: sudo apt-get install jq (Ubuntu/Debian) or brew install jq (macOS)"    echo "   Install with: sudo apt-get install jq (Ubuntu/Debian) or brew install jq (macOS)"

    remove_temporary_admin_user || true    remove_temporary_admin_user || true

    exit 1    exit 1

fifi



# Count menu items# Count menu items

MENU_ITEM_COUNT=$(jq '.menuItems | length' "$SEED_DATA_PATH")MENU_ITEM_COUNT=$(jq '.menuItems | length' "$SEED_DATA_PATH")

echo "✓ Loaded $MENU_ITEM_COUNT menu items from JSON"echo "✓ Loaded $MENU_ITEM_COUNT menu items from JSON"

echo ""echo ""



# ============================================# ============================================

# STEP 2: Seed Menu Items (Catalog Service)# STEP 2: Seed Menu Items (Catalog Service)

# ============================================# ============================================

echo "📋 Step 2: Seeding Menu Items..."echo "📋 Step 2: Seeding Menu Items..."



SUCCESS_COUNT=0SUCCESS_COUNT=0

FAIL_COUNT=0FAIL_COUNT=0



# Loop through each menu item in the JSON file# Loop through each menu item in the JSON file

for i in $(seq 0 $((MENU_ITEM_COUNT - 1))); dofor i in $(seq 0 $((MENU_ITEM_COUNT - 1))); do

    ITEM_NAME=$(jq -r ".menuItems[$i].Name" "$SEED_DATA_PATH")    ITEM_NAME=$(jq -r ".menuItems[$i].Name" "$SEED_DATA_PATH")

    MENU_ITEM=$(jq -c ".menuItems[$i]" "$SEED_DATA_PATH")    MENU_ITEM=$(jq -c ".menuItems[$i]" "$SEED_DATA_PATH")

        "PricePerUOM": 8.99,

    echo -n "  Adding: $ITEM_NAME..."    "Category": "Appetizer",

    if invoke_api_call "POST" "/api/catalog/menu" "$MENU_ITEM" &>/dev/null; then    "Cuisine": "Italian",

        echo " ✓"    "Ingredients": ["Pizza dough", "Mozzarella cheese", "Tomato sauce", "Fresh basil", "Olive oil"],

        SUCCESS_COUNT=$((SUCCESS_COUNT + 1))    "Allergens": ["Gluten", "Dairy"],

    else    "IsVegetarian": true,

        echo " ✗"    "IsVegan": false,

        FAIL_COUNT=$((FAIL_COUNT + 1))    "IsGlutenFree": false,

    fi    "SpiceLevel": 1,

done    "IsAvailable": true,

    "Calories": 320,

echo ""    "Protein": 12,

if [ $FAIL_COUNT -eq 0 ]; then    "Carbohydrates": 38,

    echo "  Summary: $SUCCESS_COUNT succeeded, $FAIL_COUNT failed"    "Fat": 14

else}

    echo "  Summary: $SUCCESS_COUNT succeeded, $FAIL_COUNT failed"EOF

fi)

echo ""if invoke_api_call "POST" "/api/catalog/menu" "$MENU_ITEM" &>/dev/null; then

    echo " ✓"

# ============================================    SUCCESS_COUNT=$((SUCCESS_COUNT + 1))

# STEP 3: Cleanupelse

# ============================================    echo " ✗"

echo "🧹 Step 3: Cleaning up..."    FAIL_COUNT=$((FAIL_COUNT + 1))

echo ""fi



# Delete temporary admin user# Menu item 2: Chicken Wings

remove_temporary_admin_userecho -n "  Adding: Chicken Wings..."

MENU_ITEM=$(cat <<'EOF'

echo ""{

    "RestaurantId": "rest-002",

# ============================================    "RestaurantName": "Burger Hub",

# STEP 4: Summary    "OwnerId": "owner-burgerhub",

# ============================================    "Name": "Chicken Wings",

echo "========================================"    "Description": "Crispy buffalo chicken wings served with ranch dipping sauce",

echo "✅ Data Seeding Complete!"    "PreparationTimeMinutes": 20,

echo "========================================"    "PackagingSize": "6 pieces",

echo ""    "UnitOfMeasure": "serving",

echo "Menu Items Created: $SUCCESS_COUNT"    "PricePerUOM": 12.99,

echo ""    "Category": "Appetizer",

echo "Next Steps:"    "Cuisine": "American",

echo "  1. Navigate to the customer app"    "Ingredients": ["Chicken wings", "Buffalo sauce", "Celery", "Ranch dressing"],

echo "  2. Browse available menu items"    "Allergens": ["Dairy"],

echo "  3. Place test orders"    "IsVegetarian": false,

echo ""    "IsVegan": false,

    "IsGlutenFree": true,
    "SpiceLevel": 3,
    "IsAvailable": true,
    "Calories": 450,
    "Protein": 28,
    "Carbohydrates": 2,
    "Fat": 36
}
EOF
)
if invoke_api_call "POST" "/api/catalog/menu" "$MENU_ITEM" &>/dev/null; then
    echo " ✓"
    SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
else
    echo " ✗"
    FAIL_COUNT=$((FAIL_COUNT + 1))
fi

# Menu item 3: Chicken Biryani
echo -n "  Adding: Chicken Biryani..."
MENU_ITEM=$(cat <<'EOF'
{
    "RestaurantId": "rest-003",
    "RestaurantName": "Indian Cuisine",
    "OwnerId": "owner-indiancuisine",
    "Name": "Chicken Biryani",
    "Description": "Aromatic basmati rice cooked with tender chicken pieces and traditional Indian spices",
    "PreparationTimeMinutes": 45,
    "PackagingSize": "Large",
    "UnitOfMeasure": "serving",
    "PricePerUOM": 18.99,
    "Category": "Main Course",
    "Cuisine": "Indian",
    "Ingredients": ["Basmati rice", "Chicken", "Onions", "Yogurt", "Biryani spices", "Saffron"],
    "Allergens": ["Dairy"],
    "IsVegetarian": false,
    "IsVegan": false,
    "IsGlutenFree": true,
    "SpiceLevel": 4,
    "IsAvailable": true,
    "Calories": 650,
    "Protein": 35,
    "Carbohydrates": 78,
    "Fat": 18
}
EOF
)
if invoke_api_call "POST" "/api/catalog/menu" "$MENU_ITEM" &>/dev/null; then
    echo " ✓"
    SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
else
    echo " ✗"
    FAIL_COUNT=$((FAIL_COUNT + 1))
fi

# Add more menu items with shortened descriptions...
# (Adding 7 more items for a total of 10)

declare -a MORE_ITEMS=(
    '{"RestaurantId":"rest-004","RestaurantName":"Thai Delight","OwnerId":"owner-thaidelight","Name":"Vegetable Pad Thai","Description":"Stir-fried rice noodles with tofu","PreparationTimeMinutes":25,"PackagingSize":"Regular","UnitOfMeasure":"serving","PricePerUOM":14.99,"Category":"Main Course","Cuisine":"Thai","Ingredients":["Rice noodles","Tofu"],"Allergens":["Nuts","Soy"],"IsVegetarian":true,"IsVegan":true,"IsGlutenFree":true,"SpiceLevel":2,"IsAvailable":true,"Calories":480,"Protein":18,"Carbohydrates":68,"Fat":16}'
    '{"RestaurantId":"rest-002","RestaurantName":"Burger Hub","OwnerId":"owner-burgerhub","Name":"Classic Beef Burger","Description":"Juicy beef patty with lettuce and tomato","PreparationTimeMinutes":18,"PackagingSize":"Single","UnitOfMeasure":"piece","PricePerUOM":16.99,"Category":"Main Course","Cuisine":"American","Ingredients":["Beef patty","Brioche bun"],"Allergens":["Gluten","Dairy"],"IsVegetarian":false,"IsVegan":false,"IsGlutenFree":false,"SpiceLevel":1,"IsAvailable":true,"Calories":580,"Protein":32,"Carbohydrates":45,"Fat":28}'
    '{"RestaurantId":"rest-005","RestaurantName":"Italian Bistro","OwnerId":"owner-italianbistro","Name":"Chocolate Lava Cake","Description":"Warm chocolate cake with molten center","PreparationTimeMinutes":12,"PackagingSize":"Individual","UnitOfMeasure":"piece","PricePerUOM":9.99,"Category":"Dessert","Cuisine":"French","Ingredients":["Dark chocolate","Butter"],"Allergens":["Gluten","Dairy","Eggs"],"IsVegetarian":true,"IsVegan":false,"IsGlutenFree":false,"SpiceLevel":1,"IsAvailable":true,"Calories":420,"Protein":8,"Carbohydrates":52,"Fat":22}'
    '{"RestaurantId":"rest-003","RestaurantName":"Indian Cuisine","OwnerId":"owner-indiancuisine","Name":"Fresh Mango Lassi","Description":"Traditional yogurt drink with mango","PreparationTimeMinutes":5,"PackagingSize":"16 oz","UnitOfMeasure":"glass","PricePerUOM":5.99,"Category":"Beverage","Cuisine":"Indian","Ingredients":["Fresh mango","Yogurt"],"Allergens":["Dairy"],"IsVegetarian":true,"IsVegan":false,"IsGlutenFree":true,"SpiceLevel":1,"IsAvailable":true,"Calories":180,"Protein":6,"Carbohydrates":32,"Fat":4}'
    '{"RestaurantId":"rest-006","RestaurantName":"Sushi Spot","OwnerId":"owner-sushispot","Name":"Green Smoothie","Description":"Healthy blend of spinach and banana","PreparationTimeMinutes":3,"PackagingSize":"12 oz","UnitOfMeasure":"glass","PricePerUOM":7.99,"Category":"Beverage","Cuisine":"Health Food","Ingredients":["Spinach","Banana"],"Allergens":[],"IsVegetarian":true,"IsVegan":true,"IsGlutenFree":true,"SpiceLevel":1,"IsAvailable":true,"Calories":140,"Protein":3,"Carbohydrates":35,"Fat":1}'
    '{"RestaurantId":"rest-005","RestaurantName":"Italian Bistro","OwnerId":"owner-italianbistro","Name":"Caesar Salad","Description":"Crisp romaine with Caesar dressing","PreparationTimeMinutes":8,"PackagingSize":"Regular","UnitOfMeasure":"bowl","PricePerUOM":11.99,"Category":"Salad","Cuisine":"Mediterranean","Ingredients":["Romaine lettuce","Caesar dressing"],"Allergens":["Gluten","Dairy","Eggs"],"IsVegetarian":true,"IsVegan":false,"IsGlutenFree":false,"SpiceLevel":1,"IsAvailable":true,"Calories":250,"Protein":8,"Carbohydrates":15,"Fat":18}'
    '{"RestaurantId":"rest-006","RestaurantName":"Sushi Spot","OwnerId":"owner-sushispot","Name":"Quinoa Buddha Bowl","Description":"Nutritious bowl with quinoa and vegetables","PreparationTimeMinutes":15,"PackagingSize":"Large","UnitOfMeasure":"bowl","PricePerUOM":13.99,"Category":"Salad","Cuisine":"Health Food","Ingredients":["Quinoa","Vegetables"],"Allergens":["Sesame"],"IsVegetarian":true,"IsVegan":true,"IsGlutenFree":true,"SpiceLevel":1,"IsAvailable":true,"Calories":380,"Protein":14,"Carbohydrates":48,"Fat":16}'
)

for item in "${MORE_ITEMS[@]}"; do
    ITEM_NAME=$(echo "$item" | grep -o '"Name":"[^"]*"' | cut -d'"' -f4)
    echo -n "  Adding: $ITEM_NAME..."
    if invoke_api_call "POST" "/api/catalog/menu" "$item" &>/dev/null; then
        echo " ✓"
        SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
    else
        echo " ✗"
        FAIL_COUNT=$((FAIL_COUNT + 1))
    fi
done

echo ""
if [ $FAIL_COUNT -eq 0 ]; then
    echo "  Summary: $SUCCESS_COUNT succeeded, $FAIL_COUNT failed"
else
    echo "  Summary: $SUCCESS_COUNT succeeded, $FAIL_COUNT failed"
fi
echo ""

# ============================================
# STEP 2: Cleanup
# ============================================
echo "🧹 Step 2: Cleaning up..."
echo ""

# Delete temporary admin user
remove_temporary_admin_user

echo ""

# ============================================
# STEP 3: Summary
# ============================================
echo "========================================"
echo "✅ Data Seeding Complete!"
echo "========================================"
echo ""
echo "Menu Items Created: $SUCCESS_COUNT"
echo ""
echo "Next Steps:"
echo "  1. Navigate to the customer app"
echo "  2. Browse available menu items"
echo "  3. Place test orders"
echo ""
