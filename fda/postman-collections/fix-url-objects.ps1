# Final URL fix - converts complex URL objects to simple strings with environment variables
# This fixes the port/host fields that override the raw URL

Write-Host "=== Final Postman URL Fix ===" -ForegroundColor Green
Write-Host ""

# Get all collection files
$collections = Get-ChildItem -Path "." -Filter "*.postman_collection.json" -Recurse

$totalChanges = 0

foreach ($collection in $collections) {
    Write-Host "Processing: $($collection.Name)" -ForegroundColor Yellow
    
    # Read as JSON
    $json = Get-Content $collection.FullName -Raw | ConvertFrom-Json
    
    $changes = 0
    
    # Function to recursively fix URL objects
    function Fix-UrlObjects {
        param($obj)
        
        if ($null -eq $obj) { return }
        
        if ($obj -is [System.Management.Automation.PSCustomObject]) {
            # Check for 'url' property that's an object with 'raw' field
            if ($obj.PSObject.Properties['url'] -and 
                $obj.url -is [System.Management.Automation.PSCustomObject] -and
                $obj.url.PSObject.Properties['raw']) {
                
                # Replace the complex URL object with just the raw string
                $rawUrl = $obj.url.raw
                $obj.url = $rawUrl
                $script:changes++
                Write-Host "  ✓ Simplified URL: $rawUrl" -ForegroundColor Green
            }
            
            # Recurse into all properties
            foreach ($prop in $obj.PSObject.Properties) {
                Fix-UrlObjects $prop.Value
            }
        }
        elseif ($obj -is [Array]) {
            foreach ($item in $obj) {
                Fix-UrlObjects $item
            }
        }
    }
    
    Fix-UrlObjects $json
    
    if ($changes -gt 0) {
        # Save back to file
        $json | ConvertTo-Json -Depth 100 | Set-Content $collection.FullName -Encoding UTF8
        Write-Host "  ✓ Made $changes changes" -ForegroundColor Green
        $totalChanges += $changes
    } else {
        Write-Host "  ℹ No changes needed" -ForegroundColor Gray
    }
    
    Write-Host ""
}

Write-Host "=== Summary ===" -ForegroundColor Green
Write-Host "Total URL objects simplified: $totalChanges" -ForegroundColor White
Write-Host ""

if ($totalChanges -gt 0) {
    Write-Host "✓ Collections updated! URLs now use environment variables." -ForegroundColor Green
    Write-Host ""
    Write-Host "Next: Run .\test.ps1 to verify fixes" -ForegroundColor Cyan
} else {
    Write-Host "ℹ No changes needed" -ForegroundColor Gray
}
