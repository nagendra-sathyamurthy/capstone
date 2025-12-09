# Quick Restaurant Seeding Script for Testing
# Creates sample restaurants with known data for Newman tests

param(
    [string]$GatewayUrl = "http://localhost:30500"
)

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║              Seeding Test Restaurants                          ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Create Restaurant Owner User
Write-Host "1. Creating Restaurant Owner user..." -ForegroundColor Yellow

$ownerRegister = @{
    email = "owner@pizzapalace.com"
    password = "Owner123!"
    role = 1  # RestaurantOwner
    name = "Pizza Palace Owner"
    phone = "+1234567890"
} | ConvertTo-Json

try {
    $ownerResponse = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/register" `
        -Method POST `
        -Body $ownerRegister `
        -ContentType "application/json" `
        -ErrorAction SilentlyContinue
    
    if ($ownerResponse.userId) {
        $ownerId = $ownerResponse.userId
        Write-Host "   ✓ Owner created: $ownerId" -ForegroundColor Green
    } else {
        # User might already exist, try to login
        $ownerLogin = @{
            email = "owner@pizzapalace.com"
            password = "Owner123!"
        } | ConvertTo-Json
        
        $loginResponse = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/login" `
            -Method POST `
            -Body $ownerLogin `
            -ContentType "application/json"
        
        $ownerId = $loginResponse.user.id
        $authToken = $loginResponse.token
        Write-Host "   ✓ Owner exists: $ownerId" -ForegroundColor Green
    }
} catch {
    # Try login if registration failed
    try {
        $ownerLogin = @{
            email = "owner@pizzapalace.com"
            password = "Owner123!"
        } | ConvertTo-Json
        
        $loginResponse = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/login" `
            -Method POST `
            -Body $ownerLogin `
            -ContentType "application/json"
        
        $ownerId = $loginResponse.user.id
        $authToken = $loginResponse.token
        Write-Host "   ✓ Owner exists: $ownerId" -ForegroundColor Green
    } catch {
        Write-Host "   ✗ Failed to create/login owner" -ForegroundColor Red
        exit 1
    }
}

# Get auth token
if (-not $authToken) {
    $ownerLogin = @{
        email = "owner@pizzapalace.com"
        password = "Owner123!"
    } | ConvertTo-Json
    
    $loginResponse = Invoke-RestMethod -Uri "$GatewayUrl/api/auth/login" `
        -Method POST `
        -Body $ownerLogin `
        -ContentType "application/json"
    
    $authToken = $loginResponse.token
}

# Debug: Decode JWT to check role claim
Write-Host "   📋 Checking auth token..." -ForegroundColor Cyan
$tokenParts = $authToken.Split('.')
if ($tokenParts.Length -eq 3) {
    $payload = $tokenParts[1]
    # Add padding if needed
    $padding = (4 - ($payload.Length % 4)) % 4
    $payload = $payload + ('=' * $padding)
    $payloadJson = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload))
    $payloadObj = $payloadJson | ConvertFrom-Json
    Write-Host "   📋 Token role claim: '$($payloadObj.role)'" -ForegroundColor Yellow
}

$headers = @{
    "Authorization" = "Bearer $authToken"
    "Content-Type" = "application/json"
}

# Create Sample Restaurants
Write-Host "`n2. Creating test restaurants..." -ForegroundColor Yellow

$restaurants = @(
    @{
        name = "Pizza Palace"
        description = "Best pizzas in town with authentic Italian recipes"
        ownerId = $ownerId
        address = @{
            street = "123 Main Street"
            city = "New York"
            state = "NY"
            zipCode = "10001"
            country = "USA"
        }
        contactInfo = @{
            phone = "+1-555-0100"
            email = "contact@pizzapalace.com"
            website = "www.pizzapalace.com"
        }
        timings = @{
            monday = @{ open = "10:00"; close = "22:00"; isOpen = $true }
            tuesday = @{ open = "10:00"; close = "22:00"; isOpen = $true }
            wednesday = @{ open = "10:00"; close = "22:00"; isOpen = $true }
            thursday = @{ open = "10:00"; close = "22:00"; isOpen = $true }
            friday = @{ open = "10:00"; close = "23:00"; isOpen = $true }
            saturday = @{ open = "10:00"; close = "23:00"; isOpen = $true }
            sunday = @{ open = "11:00"; close = "21:00"; isOpen = $true }
        }
        isActive = $true
        cuisineType = "Italian"
        rating = 4.5
    },
    @{
        name = "Burger Junction"
        description = "Gourmet burgers made with premium ingredients"
        ownerId = $ownerId
        address = @{
            street = "456 Oak Avenue"
            city = "New York"
            state = "NY"
            zipCode = "10002"
            country = "USA"
        }
        contactInfo = @{
            phone = "+1-555-0200"
            email = "info@burgerjunction.com"
            website = "www.burgerjunction.com"
        }
        timings = @{
            monday = @{ open = "11:00"; close = "21:00"; isOpen = $true }
            tuesday = @{ open = "11:00"; close = "21:00"; isOpen = $true }
            wednesday = @{ open = "11:00"; close = "21:00"; isOpen = $true }
            thursday = @{ open = "11:00"; close = "21:00"; isOpen = $true }
            friday = @{ open = "11:00"; close = "22:00"; isOpen = $true }
            saturday = @{ open = "11:00"; close = "22:00"; isOpen = $true }
            sunday = @{ open = "12:00"; close = "20:00"; isOpen = $true }
        }
        isActive = $true
        cuisineType = "American"
        rating = 4.3
    },
    @{
        name = "Sushi Express"
        description = "Fresh sushi and Japanese cuisine delivered fast"
        ownerId = $ownerId
        address = @{
            street = "789 Park Lane"
            city = "New York"
            state = "NY"
            zipCode = "10003"
            country = "USA"
        }
        contactInfo = @{
            phone = "+1-555-0300"
            email = "hello@sushiexpress.com"
            website = "www.sushiexpress.com"
        }
        timings = @{
            monday = @{ open = "12:00"; close = "22:00"; isOpen = $true }
            tuesday = @{ open = "12:00"; close = "22:00"; isOpen = $true }
            wednesday = @{ open = "12:00"; close = "22:00"; isOpen = $true }
            thursday = @{ open = "12:00"; close = "22:00"; isOpen = $true }
            friday = @{ open = "12:00"; close = "23:00"; isOpen = $true }
            saturday = @{ open = "12:00"; close = "23:00"; isOpen = $true }
            sunday = @{ open = "12:00"; close = "21:00"; isOpen = $true }
        }
        isActive = $true
        cuisineType = "Japanese"
        rating = 4.7
    }
)

$createdRestaurants = @()

foreach ($restaurant in $restaurants) {
    Write-Host "   Creating: $($restaurant.name)..." -NoNewline
    
    try {
        $body = $restaurant | ConvertTo-Json -Depth 10
        $result = Invoke-RestMethod -Uri "$GatewayUrl/api/catalog/restaurant/register" `
            -Method POST `
            -Headers $headers `
            -Body $body
        
        if ($result.id) {
            $createdRestaurants += $result
            Write-Host " ✓ ID: $($result.id)" -ForegroundColor Green
        } else {
            Write-Host " ✗ No ID returned" -ForegroundColor Red
        }
    } catch {
        $errorDetail = ""
        if ($_.ErrorDetails) {
            $errorDetail = $_.ErrorDetails.Message
        }
        Write-Host " ✗ $($_.Exception.Message)" -ForegroundColor Red
        if ($errorDetail) {
            Write-Host "     Details: $errorDetail" -ForegroundColor DarkRed
        }
    }
}

# Create Menu Items for the first restaurant
if ($createdRestaurants.Count -gt 0) {
    Write-Host "`n3. Creating menu items for $($createdRestaurants[0].name)..." -ForegroundColor Yellow
    
    $restaurantId = $createdRestaurants[0].id
    
    $menuItems = @(
        @{
            restaurantId = $restaurantId
            name = "Margherita Pizza"
            description = "Classic pizza with tomato sauce, mozzarella, and basil"
            preparationTimeMinutes = 20
            packagingSize = "Medium"
            unitOfMeasure = "piece"
            pricePerUOM = 12.99
            category = "Pizza"
            cuisine = "Italian"
            isAvailable = $true
            isVegetarian = $true
            ingredients = @("Tomato", "Mozzarella", "Basil", "Olive Oil")
            allergens = @("Dairy", "Gluten")
            spiceLevel = 1
        },
        @{
            restaurantId = $restaurantId
            name = "Pepperoni Pizza"
            description = "Traditional pizza with pepperoni and cheese"
            preparationTimeMinutes = 20
            packagingSize = "Medium"
            unitOfMeasure = "piece"
            pricePerUOM = 14.99
            category = "Pizza"
            cuisine = "Italian"
            isAvailable = $true
            isVegetarian = $false
            ingredients = @("Tomato", "Mozzarella", "Pepperoni", "Olive Oil")
            allergens = @("Dairy", "Gluten", "Pork")
            spiceLevel = 2
        },
        @{
            restaurantId = $restaurantId
            name = "Caesar Salad"
            description = "Fresh romaine lettuce with Caesar dressing and croutons"
            preparationTimeMinutes = 10
            packagingSize = "Regular"
            unitOfMeasure = "serving"
            pricePerUOM = 8.99
            category = "Salads"
            cuisine = "American"
            isAvailable = $true
            isVegetarian = $true
            ingredients = @("Romaine Lettuce", "Caesar Dressing", "Croutons", "Parmesan")
            allergens = @("Dairy", "Gluten", "Egg")
            spiceLevel = 1
        },
        @{
            restaurantId = $restaurantId
            name = "Garlic Bread"
            description = "Crispy bread with garlic butter and herbs"
            preparationTimeMinutes = 10
            packagingSize = "Small"
            unitOfMeasure = "piece"
            pricePerUOM = 5.99
            category = "Sides"
            cuisine = "Italian"
            isAvailable = $true
            isVegetarian = $true
            ingredients = @("Bread", "Garlic", "Butter", "Parsley")
            allergens = @("Dairy", "Gluten")
            spiceLevel = 1
        }
    )
    
    foreach ($item in $menuItems) {
        Write-Host "   Adding: $($item.name)..." -NoNewline
        
        try {
            $body = $item | ConvertTo-Json -Depth 10
            $result = Invoke-RestMethod -Uri "$GatewayUrl/api/catalog/menu" `
                -Method POST `
                -Headers $headers `
                -Body $body
            
            Write-Host " ✓" -ForegroundColor Green
        } catch {
            Write-Host " ✗" -ForegroundColor Red
        }
    }
}

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                  ✅ Seeding Complete!                           ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Green

Write-Host "Created Restaurants: $($createdRestaurants.Count)" -ForegroundColor Cyan

if ($createdRestaurants.Count -gt 0) {
    Write-Host "`nRestaurant IDs (for testing):" -ForegroundColor Yellow
    foreach ($r in $createdRestaurants) {
        Write-Host "  • $($r.name): $($r.id)" -ForegroundColor White
    }
}

Write-Host "`nYou can now run Newman tests with:" -ForegroundColor Cyan
Write-Host "  .\test.ps1" -ForegroundColor White
Write-Host ""
