# PowerShell script to fix deprecated Chai assertion syntax in Postman collections
# Fixes: pm.response.to.have.status.oneOf -> pm.expect(pm.response.code).to.be.oneOf

Write-Host "Fixing Chai assertion syntax in Postman collections..." -ForegroundColor Yellow

# Define collections to fix
$collections = @(
    "postman\collections\Food-Delivery-API-Collection.postman_collection.json",
    "postman\collections\CRM-Service.postman_collection.json", 
    "postman\collections\Cart-Service.postman_collection.json"
)

$totalReplacements = 0

foreach ($collection in $collections) {
    if (Test-Path $collection) {
        Write-Host "Processing: $collection" -ForegroundColor Cyan
        
        # Read the file content
        $content = Get-Content $collection -Raw
        
        # Count existing occurrences
        $beforeCount = ([regex]::Matches($content, "pm\.response\.to\.have\.status\.oneOf")).Count
        
        if ($beforeCount -gt 0) {
            # Replace the deprecated syntax
            $content = $content -replace "pm\.response\.to\.have\.status\.oneOf\(\[([^\]]+)\]\);", "pm.expect(pm.response.code).to.be.oneOf([`$1]);"
            
            # Write back to file
            Set-Content $collection -Value $content -NoNewline
            
            # Count after replacement
            $afterCount = ([regex]::Matches($content, "pm\.response\.to\.have\.status\.oneOf")).Count
            $fixedCount = $beforeCount - $afterCount
            
            Write-Host "  Fixed $fixedCount deprecated assertions" -ForegroundColor Green
            $totalReplacements += $fixedCount
        } else {
            Write-Host "  No deprecated syntax found" -ForegroundColor Green
        }
    } else {
        Write-Host "  File not found: $collection" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Complete! Fixed $totalReplacements total deprecated assertions." -ForegroundColor Green
Write-Host "Collections are now ready for live testing with proper Chai syntax." -ForegroundColor White