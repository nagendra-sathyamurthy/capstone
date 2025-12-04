# Remove duplicate /api/xxx paths from collection URLs
# Since environment variables now include the /api/xxx part, 
# we need to remove it from the collection URLs

Write-Host "=== Remove Duplicate API Paths ===" -ForegroundColor Green
Write-Host ""

$collections = Get-ChildItem -Path "." -Filter "*.postman_collection.json" -Recurse
$totalFixes = 0

foreach ($collection in $collections) {
    Write-Host "Processing: $($collection.Name)" -ForegroundColor Yellow
    
    # Read as text for simple replacement
    $content = Get-Content $collection.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Replace duplicated API paths
    $replacements = @{
        '/api/auth/api/auth/' = '/api/auth/'
        '/api/catalog/api/catalog/' = '/api/catalog/'
        '/api/catalog/menu' = '/api/catalog/menu'  # This is correct
        '/api/crm/api/crm/' = '/api/crm/'
        '/api/crm/restaurant' = '/api/crm/restaurant'  # This is correct
        '/api/order/api/order/' = '/api/order/'
        '/api/order/api/order/restaurant' = '/api/order/restaurant'
        '/api/order/api/order/inventory' = '/api/order/inventory'
        '/api/cart/api/cart/' = '/api/cart/'
        '/api/payment/api/payment/' = '/api/payment/'
    }
    
    $fixes = 0
    foreach ($key in $replacements.Keys) {
        $value = $replacements[$key]
        if ($content -ne $originalContent) {
            $originalContent = $content
        }
        $beforeCount = ([regex]::Matches($content, [regex]::Escape($key))).Count
        $content = $content -replace [regex]::Escape($key), $value
        $afterCount = ([regex]::Matches($content, [regex]::Escape($key))).Count
        $changed = $beforeCount - $afterCount
        if ($changed -gt 0) {
            Write-Host "  ✓ Fixed $changed occurrence(s) of $key" -ForegroundColor Green
            $fixes += $changed
        }
    }
    
    if ($fixes -gt 0) {
        $content | Set-Content $collection.FullName -Encoding UTF8 -NoNewline
        Write-Host "  ✓ Total fixes: $fixes" -ForegroundColor Green
        $totalFixes += $fixes
    } else {
        Write-Host "  ℹ No duplicates found" -ForegroundColor Gray
    }
    Write-Host ""
}

Write-Host "=== Summary ===" -ForegroundColor Green
Write-Host "Total duplicates removed: $totalFixes" -ForegroundColor White
Write-Host ""

if ($totalFixes -gt 0) {
    Write-Host "✓ Collections cleaned!" -ForegroundColor Green
} else {
    Write-Host "ℹ No duplicates found" -ForegroundColor Gray
}
