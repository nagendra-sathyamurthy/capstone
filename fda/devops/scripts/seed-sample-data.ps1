# Sample Data Seeding Script for Food Delivery Application
# This script seeds the system with sample restaurants, menu items, and user profiles via API calls

param(
    [string]$GatewayUrl = "http://localhost:5000",
    [string]$AuthToken = ""
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Food Delivery App - Sample Data Seeder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Authentication token - update this with a valid token
if ([string]::IsNullOrEmpty($AuthToken)) {
    Write-Host "⚠️  No auth token provided. Some endpoints may fail." -ForegroundColor Yellow
    Write-Host "   To provide a token, use: .\seed-sample-data.ps1 -AuthToken 'your-token-here'" -ForegroundColor Yellow
    Write-Host ""
}

$headers = @{
    "Content-Type" = "Application/json"
}

if (![string]::IsNullOrEmpty($AuthToken)) {
    $headers["Authorization"] = "Bearer $AuthToken"
}

# Helper function to make API calls
function Invoke-ApiCall {
    param(
        [string]$Method,
        [string]$Endpoint,
        [object]$Body = $null
    )
    
    $url = "$GatewayUrl$Endpoint"
    
    try {
        if ($Body) {
            $jsonBody = $Body | ConvertTo-Json -Depth 10
            $response = Invoke-RestMethod -Uri $url -Method $Method -Headers $headers -Body $jsonBody
        } else {
            $response = Invoke-RestMethod -Uri $url -Method $Method -Headers $headers
        }
        return $response
    }
    catch {
        Write-Host "❌ Error calling $Method $url" -ForegroundColor Red
        Write-Host "   $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

Write-Host "🔄 Starting data seeding process..." -ForegroundColor Green
Write-Host ""

# ============================================
# STEP 1: Seed Menu Items (Catalog Service)
# ============================================
Write-Host "📋 Step 1: Seeding Menu Items..." -ForegroundColor Yellow

$menuItems = @(
    @{
        RestaurantId = "rest-001"
        RestaurantName = "Pizza Palace"
        OwnerId = "owner-pizzapalace"
        Name = "Margherita Pizza Slice"
        Description = "Classic pizza with fresh mozzarella, tomato sauce, and basil leaves"
        PreparationTimeMinutes = 15
        PackagingSize = "Single Slice"
        UnitOfMeasure = "slice"
        PricePerUOM = 8.99
        Category = "Appetizer"
        Cuisine = "Italian"
        Ingredients = @("Pizza dough", "Mozzarella cheese", "Tomato sauce", "Fresh basil", "Olive oil")
        Allergens = @("Gluten", "Dairy")
        IsVegetarian = $true
        IsVegan = $false
        IsGlutenFree = $false
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 320
        Protein = 12
        Carbohydrates = 38
        Fat = 14
    },
    @{
        RestaurantId = "rest-002"
        RestaurantName = "Burger Hub"
        OwnerId = "owner-burgerhub"
        Name = "Chicken Wings"
        Description = "Crispy buffalo chicken wings served with ranch dipping sauce"
        PreparationTimeMinutes = 20
        PackagingSize = "6 pieces"
        UnitOfMeasure = "serving"
        PricePerUOM = 12.99
        Category = "Appetizer"
        Cuisine = "American"
        Ingredients = @("Chicken wings", "Buffalo sauce", "Celery", "Ranch dressing")
        Allergens = @("Dairy")
        IsVegetarian = $false
        IsVegan = $false
        IsGlutenFree = $true
        SpiceLevel = 3
        IsAvailable = $true
        Calories = 450
        Protein = 28
        Carbohydrates = 2
        Fat = 36
    },
    @{
        RestaurantId = "rest-003"
        RestaurantName = "Indian Cuisine"
        OwnerId = "owner-indiancuisine"
        Name = "Chicken Biryani"
        Description = "Aromatic basmati rice cooked with tender chicken pieces and traditional Indian spices"
        PreparationTimeMinutes = 45
        PackagingSize = "Large"
        UnitOfMeasure = "serving"
        PricePerUOM = 18.99
        Category = "Main Course"
        Cuisine = "Indian"
        Ingredients = @("Basmati rice", "Chicken", "Onions", "Yogurt", "Biryani spices", "Saffron")
        Allergens = @("Dairy")
        IsVegetarian = $false
        IsVegan = $false
        IsGlutenFree = $true
        SpiceLevel = 4
        IsAvailable = $true
        Calories = 650
        Protein = 35
        Carbohydrates = 78
        Fat = 18
    },
    @{
        RestaurantId = "rest-004"
        RestaurantName = "Thai Delight"
        OwnerId = "owner-thaidelight"
        Name = "Vegetable Pad Thai"
        Description = "Stir-fried rice noodles with tofu, vegetables, and tamarind-based sauce"
        PreparationTimeMinutes = 25
        PackagingSize = "Regular"
        UnitOfMeasure = "serving"
        PricePerUOM = 14.99
        Category = "Main Course"
        Cuisine = "Thai"
        Ingredients = @("Rice noodles", "Tofu", "Bean sprouts", "Carrots", "Tamarind paste", "Peanuts")
        Allergens = @("Nuts", "Soy")
        IsVegetarian = $true
        IsVegan = $true
        IsGlutenFree = $true
        SpiceLevel = 2
        IsAvailable = $true
        Calories = 480
        Protein = 18
        Carbohydrates = 68
        Fat = 16
    },
    @{
        RestaurantId = "rest-002"
        RestaurantName = "Burger Hub"
        OwnerId = "owner-burgerhub"
        Name = "Classic Beef Burger"
        Description = "Juicy beef patty with lettuce, tomato, onion, and house sauce on a brioche bun"
        PreparationTimeMinutes = 18
        PackagingSize = "Single"
        UnitOfMeasure = "piece"
        PricePerUOM = 16.99
        Category = "Main Course"
        Cuisine = "American"
        Ingredients = @("Beef patty", "Brioche bun", "Lettuce", "Tomato", "Onion", "House sauce")
        Allergens = @("Gluten", "Dairy")
        IsVegetarian = $false
        IsVegan = $false
        IsGlutenFree = $false
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 580
        Protein = 32
        Carbohydrates = 45
        Fat = 28
    },
    @{
        RestaurantId = "rest-005"
        RestaurantName = "Italian Bistro"
        OwnerId = "owner-italianbistro"
        Name = "Chocolate Lava Cake"
        Description = "Warm chocolate cake with molten chocolate center, served with vanilla ice cream"
        PreparationTimeMinutes = 12
        PackagingSize = "Individual"
        UnitOfMeasure = "piece"
        PricePerUOM = 9.99
        Category = "Dessert"
        Cuisine = "French"
        Ingredients = @("Dark chocolate", "Butter", "Eggs", "Flour", "Sugar", "Vanilla ice cream")
        Allergens = @("Gluten", "Dairy", "Eggs")
        IsVegetarian = $true
        IsVegan = $false
        IsGlutenFree = $false
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 420
        Protein = 8
        Carbohydrates = 52
        Fat = 22
    },
    @{
        RestaurantId = "rest-003"
        RestaurantName = "Indian Cuisine"
        OwnerId = "owner-indiancuisine"
        Name = "Fresh Mango Lassi"
        Description = "Traditional Indian yogurt-based drink blended with fresh mango and cardamom"
        PreparationTimeMinutes = 5
        PackagingSize = "16 oz"
        UnitOfMeasure = "glass"
        PricePerUOM = 5.99
        Category = "Beverage"
        Cuisine = "Indian"
        Ingredients = @("Fresh mango", "Yogurt", "Milk", "Sugar", "Cardamom")
        Allergens = @("Dairy")
        IsVegetarian = $true
        IsVegan = $false
        IsGlutenFree = $true
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 180
        Protein = 6
        Carbohydrates = 32
        Fat = 4
    },
    @{
        RestaurantId = "rest-006"
        RestaurantName = "Sushi Spot"
        OwnerId = "owner-sushispot"
        Name = "Green Smoothie"
        Description = "Healthy blend of spinach, banana, apple, and coconut water"
        PreparationTimeMinutes = 3
        PackagingSize = "12 oz"
        UnitOfMeasure = "glass"
        PricePerUOM = 7.99
        Category = "Beverage"
        Cuisine = "Health Food"
        Ingredients = @("Fresh spinach", "Banana", "Apple", "Coconut water", "Honey")
        Allergens = @()
        IsVegetarian = $true
        IsVegan = $true
        IsGlutenFree = $true
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 140
        Protein = 3
        Carbohydrates = 35
        Fat = 1
    },
    @{
        RestaurantId = "rest-005"
        RestaurantName = "Italian Bistro"
        OwnerId = "owner-italianbistro"
        Name = "Caesar Salad"
        Description = "Crisp romaine lettuce with Caesar dressing, croutons, and parmesan cheese"
        PreparationTimeMinutes = 8
        PackagingSize = "Regular"
        UnitOfMeasure = "bowl"
        PricePerUOM = 11.99
        Category = "Salad"
        Cuisine = "Mediterranean"
        Ingredients = @("Romaine lettuce", "Caesar dressing", "Croutons", "Parmesan cheese")
        Allergens = @("Gluten", "Dairy", "Eggs")
        IsVegetarian = $true
        IsVegan = $false
        IsGlutenFree = $false
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 250
        Protein = 8
        Carbohydrates = 15
        Fat = 18
    },
    @{
        RestaurantId = "rest-006"
        RestaurantName = "Sushi Spot"
        OwnerId = "owner-sushispot"
        Name = "Quinoa Buddha Bowl"
        Description = "Nutritious bowl with quinoa, roasted vegetables, avocado, and tahini dressing"
        PreparationTimeMinutes = 15
        PackagingSize = "Large"
        UnitOfMeasure = "bowl"
        PricePerUOM = 13.99
        Category = "Salad"
        Cuisine = "Health Food"
        Ingredients = @("Quinoa", "Roasted vegetables", "Avocado", "Chickpeas", "Tahini dressing")
        Allergens = @("Sesame")
        IsVegetarian = $true
        IsVegan = $true
        IsGlutenFree = $true
        SpiceLevel = 1
        IsAvailable = $true
        Calories = 380
        Protein = 14
        Carbohydrates = 48
        Fat = 16
    }
)

$successCount = 0
$failCount = 0

foreach ($item in $menuItems) {
    Write-Host "  Adding: $($item.Name)..." -NoNewline
    $result = Invoke-ApiCall -Method POST -Endpoint "/api/menu" -Body $item
    
    if ($result) {
        Write-Host " ✓" -ForegroundColor Green
        $successCount++
    } else {
        Write-Host " ✗" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "  Summary: $successCount succeeded, $failCount failed" -ForegroundColor $(if ($failCount -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

# ============================================
# STEP 2: Summary
# ============================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Data Seeding Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Menu Items Created: $successCount" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Navigate to the customer app" -ForegroundColor White
Write-Host "  2. Browse available menu items" -ForegroundColor White
Write-Host "  3. Place test orders" -ForegroundColor White
Write-Host ""
