# Setup User Secrets for Local Development
# This script configures MongoDB connection strings in user secrets for each service
# Run this script after cloning the repository to set up your local development environment

Write-Host "Setting up user secrets for local development..." -ForegroundColor Green
Write-Host ""

# Default MongoDB connection string for local development
# Developers can modify this if their local MongoDB has different credentials
$mongoHost = "localhost:27017"
$mongoUser = "admin"
$mongoPass = Read-Host "Enter MongoDB password (default: admin123)" -AsSecureString
$mongoPassPlainText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($mongoPass))
if ([string]::IsNullOrWhiteSpace($mongoPassPlainText)) {
    $mongoPassPlainText = "admin123"
}

Write-Host ""
Write-Host "Configuring user secrets for each service..." -ForegroundColor Cyan

# Authentication Service
Write-Host "  - Authentication Service" -ForegroundColor Yellow
$authConnStr = "mongodb://${mongoUser}:${mongoPassPlainText}@${mongoHost}/authenticationdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" $authConnStr --project "$PSScriptRoot\..\..\src\services\authentication\API"

# Cart Service
Write-Host "  - Cart Service" -ForegroundColor Yellow
$cartConnStr = "mongodb://${mongoUser}:${mongoPassPlainText}@${mongoHost}/cartdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" $cartConnStr --project "$PSScriptRoot\..\..\src\services\cart\API"

# Catalog Service
Write-Host "  - Catalog Service" -ForegroundColor Yellow
$catalogConnStr = "mongodb://${mongoUser}:${mongoPassPlainText}@${mongoHost}/catalogdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" $catalogConnStr --project "$PSScriptRoot\..\..\src\services\catalog\API"

# CRM Service
Write-Host "  - CRM Service" -ForegroundColor Yellow
$crmConnStr = "mongodb://${mongoUser}:${mongoPassPlainText}@${mongoHost}/crmdb?authSource=admin"
dotnet user-secrets set "MONGO_CONNECTION_STRING" $crmConnStr --project "$PSScriptRoot\..\..\src\services\crm\API"

Write-Host ""
Write-Host "User secrets configured successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "You can now run and debug services from VS Code (F5)" -ForegroundColor Cyan
Write-Host ""
Write-Host "To view secrets for a service, run:" -ForegroundColor Gray
Write-Host "  dotnet user-secrets list --project fda/src/services/<service-name>/API" -ForegroundColor Gray
Write-Host ""
Write-Host "To update a secret, run:" -ForegroundColor Gray
Write-Host "  dotnet user-secrets set ""MONGO_CONNECTION_STRING"" ""<new-value>"" --project fda/src/services/<service-name>/API" -ForegroundColor Gray
Write-Host ""
