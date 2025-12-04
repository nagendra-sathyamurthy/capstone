# Fix hardcoded URLs in Postman collections to use environment variables
# This script replaces hardcoded localhost URLs with environment variable placeholders

Write-Host "=== Fixing Postman Collection URLs ===" -ForegroundColor Green
Write-Host ""

# Define URL replacements
$urlReplacements = @{
    'http://localhost:30001/api/auth' = '{{auth_base_url}}'
    'http://localhost:30002/api/restaurant' = '{{crm_base_url}}/restaurant'
    'http://localhost:30002/api/menu' = '{{catalog_base_url}}/menu'
    'http://localhost:30002/api/operator' = '{{order_base_url}}/operator'
    'http://localhost:30003' = '{{crm_base_url}}'
    'http://localhost:30004' = '{{cart_base_url}}'
    'http://localhost:30005' = '{{order_base_url}}'
    'http://localhost:30006' = '{{payment_base_url}}'
    'http://localhost:30500' = '{{gateway_base_url}}'
}

# Get all Postman collection files
$collections = Get-ChildItem -Path "." -Filter "*.postman_collection.json" -Recurse

Write-Host "Found $($collections.Count) collection(s) to process:" -ForegroundColor Cyan
foreach ($collection in $collections) {
    Write-Host "  - $($collection.Name)" -ForegroundColor Gray
}
Write-Host ""

$totalReplacements = 0

foreach ($collection in $collections) {
    Write-Host "Processing: $($collection.Name)" -ForegroundColor Yellow
    
    # Read the collection file
    $content = Get-Content $collection.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $replacements = 0
    
    # Apply replacements
    foreach ($oldUrl in $urlReplacements.Keys) {
        $newUrl = $urlReplacements[$oldUrl]
        if ($content -match [regex]::Escape($oldUrl)) {
            $count = ([regex]::Matches($content, [regex]::Escape($oldUrl))).Count
            $content = $content -replace [regex]::Escape($oldUrl), $newUrl
            Write-Host "  ✓ Replaced $count occurrence(s) of $oldUrl → $newUrl" -ForegroundColor Green
            $replacements += $count
        }
    }
    
    # Save if changes were made
    if ($replacements -gt 0) {
        $content | Set-Content $collection.FullName -Encoding UTF8 -NoNewline
        Write-Host "  ✓ Saved changes: $replacements total replacement(s)" -ForegroundColor Green
        $totalReplacements += $replacements
    } else {
        Write-Host "  ℹ No changes needed" -ForegroundColor Gray
    }
    Write-Host ""
}

Write-Host "=== Summary ===" -ForegroundColor Green
Write-Host "Total replacements made: $totalReplacements" -ForegroundColor White
Write-Host ""

if ($totalReplacements -gt 0) {
    Write-Host "✓ Collections have been updated to use environment variables!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Review the changes: git diff" -ForegroundColor Gray
    Write-Host "2. Test the collections: .\test.ps1" -ForegroundColor Gray
    Write-Host "3. Commit if everything works: git commit -am 'fix: Update Postman collections to use environment variables'" -ForegroundColor Gray
} else {
    Write-Host "ℹ No changes were needed." -ForegroundColor Gray
}
