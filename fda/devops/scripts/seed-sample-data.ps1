# Sample Data Seeding Script for Food Delivery Application
# This script seeds the system with sample restaurants, menu items, and user profiles via API calls

param(
    [string]$GatewayUrl = "http://localhost:5000"
)

# Generate unique credentials for temporary admin user
$TempAdminEmail = "seed-admin-$(Get-Date -Format 'yyyyMMddHHmmss')@temp.local"
$TempAdminPassword = "TempSeed$(Get-Random -Minimum 1000 -Maximum 9999)!"
$TempAdminUserId = $null

# Function to create temporary admin user
function New-TemporaryAdminUser {
    param(
        [string]$GatewayUrl,
        [string]$Email,
        [string]$Password
    )
    
    try {
        Write-Host "👤 Creating temporary admin user: $Email" -ForegroundColor Yellow
        
        $registerBody = @{
            email = $Email
            password = $Password
            role = 2  # Operator role (admin permissions)
            organization = "SeedingScript"
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/register" `
            -Method POST `
            -Body $registerBody `
            -ContentType "application/json" `
            -ErrorAction Stop
        
        if ($response.id) {
            Write-Host "✓ Successfully created temporary admin user" -ForegroundColor Green
            return $response.id
        } else {
            Write-Host "✗ Failed to create admin user" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "✗ Error creating admin user: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "   Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# Function to get JWT token for admin user
function Get-AdminAuthToken {
    param(
        [string]$GatewayUrl,
        [string]$Email,
        [string]$Password
    )
    
    try {
        Write-Host "� Logging in as admin user..." -ForegroundColor Yellow
        
        $loginBody = @{
            email = $Email
            password = $Password
            loginMethod = 0  # EmailPassword
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/login" `
            -Method POST `
            -Body $loginBody `
            -ContentType "application/json" `
            -ErrorAction Stop
        
        if ($response.token) {
            Write-Host "✓ Successfully obtained auth token" -ForegroundColor Green
            return $response.token
        } else {
            Write-Host "✗ Failed to get auth token from response" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "✗ Error getting auth token: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "   Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# Function to delete temporary admin user
function Remove-TemporaryAdminUser {
    param(
        [string]$GatewayUrl,
        [string]$UserId,
        [string]$AuthToken
    )
    
    try {
        Write-Host "�️  Deleting temporary admin user..." -ForegroundColor Yellow
        
        $headers = @{
            "Authorization" = "Bearer $AuthToken"
        }
        
        Invoke-RestMethod -Uri "$GatewayUrl/api/auth/users/$UserId" `
            -Method DELETE `
            -Headers $headers `
            -ErrorAction Stop | Out-Null
        
        Write-Host "✓ Successfully deleted temporary admin user" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "⚠️  Warning: Could not delete temporary admin user" -ForegroundColor Yellow
        Write-Host "   $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "   Manual cleanup may be required for user: $UserId" -ForegroundColor Yellow
        return $false
    }
}

# Create temporary admin user
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setting up temporary admin account..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$TempAdminUserId = New-TemporaryAdminUser -GatewayUrl $GatewayUrl -Email $TempAdminEmail -Password $TempAdminPassword

if ([string]::IsNullOrEmpty($TempAdminUserId)) {
    Write-Host "❌ Failed to create temporary admin user. Cannot proceed with seeding." -ForegroundColor Red
    exit 1
}

# Get auth token for admin user
$AuthToken = Get-AdminAuthToken -GatewayUrl $GatewayUrl -Email $TempAdminEmail -Password $TempAdminPassword

if ([string]::IsNullOrEmpty($AuthToken)) {
    Write-Host "❌ Failed to obtain auth token. Cannot proceed with seeding." -ForegroundColor Red
    # Try to cleanup
    if (![string]::IsNullOrEmpty($TempAdminUserId)) {
        Remove-TemporaryAdminUser -GatewayUrl $GatewayUrl -UserId $TempAdminUserId -AuthToken ""
    }
    exit 1
}

Write-Host ""

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Food Delivery App - Sample Data Seeder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

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
    $result = Invoke-ApiCall -Method POST -Endpoint "/api/catalog/menu" -Body $item
    
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
# STEP 2: Cleanup
# ============================================
Write-Host "🧹 Step 2: Cleaning up..." -ForegroundColor Yellow
Write-Host ""

# Delete temporary admin user
Remove-TemporaryAdminUser -GatewayUrl $GatewayUrl -UserId $TempAdminUserId -AuthToken $AuthToken | Out-Null

Write-Host ""

# ============================================
# STEP 3: Summary
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
