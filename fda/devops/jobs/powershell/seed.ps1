# Sample Data Seeding Script for Food Delivery Application
# This script seeds the system with sample restaurants, menu items, and user profiles via API calls

param(
    [string]$GatewayUrl = "http://localhost:30500"
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
        
        if ($response.user.id) {
            Write-Host "✓ Successfully created temporary admin user" -ForegroundColor Green
            return $response.user.id
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
# STEP 1: Load Menu Items from JSON
# ============================================
Write-Host "📂 Loading seed data from JSON file..." -ForegroundColor Yellow

$seedDataPath = Join-Path $PSScriptRoot "..\data\seed-menu-items.json"
if (-not (Test-Path $seedDataPath)) {
    Write-Host "❌ Seed data file not found: $seedDataPath" -ForegroundColor Red
    Remove-TemporaryAdminUser -GatewayUrl $GatewayUrl -UserId $TempAdminUserId -AuthToken $AuthToken | Out-Null
    exit 1
}

$seedData = Get-Content $seedDataPath -Raw | ConvertFrom-Json
$menuItems = $seedData.menuItems

Write-Host "✓ Loaded $($menuItems.Count) menu items from JSON" -ForegroundColor Green
Write-Host ""

# ============================================
# STEP 2: Seed Menu Items (Catalog Service)
# ============================================
Write-Host "📋 Step 2: Seeding Menu Items..." -ForegroundColor Yellow

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
# STEP 3: Cleanup
# ============================================
Write-Host "🧹 Step 3: Cleaning up..." -ForegroundColor Yellow
Write-Host ""

# Delete temporary admin user
Remove-TemporaryAdminUser -GatewayUrl $GatewayUrl -UserId $TempAdminUserId -AuthToken $AuthToken | Out-Null

Write-Host ""

# ============================================
# STEP 4: Summary
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
