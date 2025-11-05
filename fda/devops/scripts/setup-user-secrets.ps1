# Setup User Secrets for Local Development
# This script configures .NET User Secrets for each microservice with MongoDB connection strings

Write-Host "Setting up .NET User Secrets for Capstone Microservices..." -ForegroundColor Green
Write-Host "This will configure MongoDB connection strings for local development." -ForegroundColor Yellow

# Check if dotnet CLI is available
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET CLI found: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Error "✗ .NET CLI not found. Please install .NET SDK."
    exit 1
}

# Function to set user secrets for a service
function Set-ServiceSecrets {
    param(
        [string]$ServicePath,
        [string]$ServiceName,
        [string]$Database
    )
    
    Write-Host "`nConfiguring secrets for $ServiceName..." -ForegroundColor Cyan
    
    if (Test-Path $ServicePath) {
        Push-Location $ServicePath
        
        try {
            # Initialize user secrets if not already done
            dotnet user-secrets init
            
            # Set connection strings
            dotnet user-secrets set "ConnectionStrings:DefaultConnection" "mongodb://Athena:Ath3n@Str0ngP@ssw0rd2024!@localhost:9000/${Database}?authSource=admin"
            dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://Athena:Ath3n@Str0ngP@ssw0rd2024!@localhost:9000/${Database}?authSource=admin"
            
            Write-Host "✓ Secrets configured for $ServiceName" -ForegroundColor Green
        }
        catch {
            Write-Warning "✗ Failed to configure secrets for $ServiceName : $($_.Exception.Message)"
        }
        finally {
            Pop-Location
        }
    }
    else {
        Write-Warning "✗ Service path not found: $ServicePath"
    }
}

# Base path to services
$servicesPath = "c:\dotnet\capstone\fda\src\services"

# Configure each service
Set-ServiceSecrets -ServicePath "$servicesPath\authentication\Services" -ServiceName "Authentication" -Database "authenticationdb"
Set-ServiceSecrets -ServicePath "$servicesPath\catalog\Services" -ServiceName "Catalog" -Database "catalogdb"
Set-ServiceSecrets -ServicePath "$servicesPath\crm\Services" -ServiceName "CRM" -Database "crmdb"
Set-ServiceSecrets -ServicePath "$servicesPath\cart\Services" -ServiceName "Cart" -Database "cartdb"

Write-Host "`n✓ User Secrets configuration complete!" -ForegroundColor Green
Write-Host "`nTo view configured secrets for a service, run:" -ForegroundColor Yellow
Write-Host "  cd [service-path]" -ForegroundColor Gray
Write-Host "  dotnet user-secrets list" -ForegroundColor Gray

Write-Host "`nTo manually set a secret, run:" -ForegroundColor Yellow
Write-Host "  cd [service-path]" -ForegroundColor Gray
Write-Host "  dotnet user-secrets set 'Key:Name' 'Value'" -ForegroundColor Gray

Write-Host "`nServices are now configured for local development with MongoDB on localhost:9000" -ForegroundColor Green