# Setup Docker Secrets for Local Development
# This script creates secret files needed for docker-compose
# These files are gitignored and contain sensitive configuration

Write-Host "🔐 Setting up Docker Secrets for Local Development" -ForegroundColor Cyan
Write-Host ""

$secretsDir = "$PSScriptRoot\secrets"

# Create secrets directory if it doesn't exist
if (-not (Test-Path $secretsDir)) {
    New-Item -ItemType Directory -Path $secretsDir -Force | Out-Null
    Write-Host "✓ Created secrets directory" -ForegroundColor Green
}

# Prompt for MongoDB credentials
Write-Host "MongoDB Configuration:" -ForegroundColor Yellow
$mongoUser = Read-Host "Enter MongoDB root username (default: admin)"
if ([string]::IsNullOrWhiteSpace($mongoUser)) {
    $mongoUser = "admin"
}

$mongoPass = Read-Host "Enter MongoDB root password (default: AdminPass2024)" -AsSecureString
$mongoPassPlainText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($mongoPass))
if ([string]::IsNullOrWhiteSpace($mongoPassPlainText)) {
    $mongoPassPlainText = "AdminPass2024"
}

Write-Host ""
Write-Host "JWT Configuration:" -ForegroundColor Yellow
Write-Host "Enter JWT Secret Key (default: auto-generate)"
$jwtKey = Read-Host -AsSecureString
$jwtKeyPlainText = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($jwtKey))
if ([string]::IsNullOrWhiteSpace($jwtKeyPlainText)) {
    # Generate a random 64-character key
    $jwtKeyPlainText = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
    Write-Host "Generated random JWT key" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Creating secret files..." -ForegroundColor Cyan

# MongoDB secrets
Set-Content -Path "$secretsDir\mongo_root_username.txt" -Value $mongoUser -NoNewline
Write-Host "  ✓ mongo_root_username.txt" -ForegroundColor Green

Set-Content -Path "$secretsDir\mongo_root_password.txt" -Value $mongoPassPlainText -NoNewline
Write-Host "  ✓ mongo_root_password.txt" -ForegroundColor Green

# JWT secret
Set-Content -Path "$secretsDir\jwt_secret_key.txt" -Value $jwtKeyPlainText -NoNewline
Write-Host "  ✓ jwt_secret_key.txt" -ForegroundColor Green

# MongoDB connection strings
$mongoConnStr = "mongodb://${mongoUser}:${mongoPassPlainText}@mongodb:27017"
Set-Content -Path "$secretsDir\mongo_connection_string.txt" -Value $mongoConnStr -NoNewline
Write-Host "  ✓ mongo_connection_string.txt" -ForegroundColor Green

$mongoConnStrCrm = "mongodb://${mongoUser}:${mongoPassPlainText}@mongodb:27017/crmdb?authSource=admin"
Set-Content -Path "$secretsDir\mongo_connection_string_crm.txt" -Value $mongoConnStrCrm -NoNewline
Write-Host "  ✓ mongo_connection_string_crm.txt" -ForegroundColor Green

Write-Host ""
Write-Host "✅ Docker secrets configured successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review the generated secrets in: $secretsDir" -ForegroundColor White
Write-Host "  2. Copy .env.example to .env and customize if needed" -ForegroundColor White
Write-Host "  3. Run: docker compose -f docker-compose-working.yml up -d" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  IMPORTANT: Never commit files in the secrets/ directory!" -ForegroundColor Yellow
Write-Host ""

