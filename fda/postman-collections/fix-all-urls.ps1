# Comprehensive URL fixer for Postman collections
# Replaces ALL hardcoded localhost:PORT references with environment variables

Write-Host "=== Comprehensive Postman URL Fixer ===" -ForegroundColor Green
Write-Host ""

# Get all collection files
$collections = Get-ChildItem -Path "." -Filter "*.postman_collection.json" -Recurse

$totalReplacements = 0

foreach ($collection in $collections) {
    Write-Host "Processing: $($collection.Name)" -ForegroundColor Yellow
    
    # Read as JSON
    $json = Get-Content $collection.FullName -Raw | ConvertFrom-Json
    
    $replacements = 0
    
    # Function to recursively search and replace URLs
    function Fix-Urls {
        param($obj)
        
        if ($null -eq $obj) { return }
        
        if ($obj -is [System.Management.Automation.PSCustomObject]) {
            # Check for 'url' property
            if ($obj.PSObject.Properties['url']) {
                $urlObj = $obj.url
                
                # Handle string URLs
                if ($urlObj -is [string]) {
                    $original = $urlObj
                    
                    # Replace authentication URLs
                    if ($urlObj -match 'localhost:30001/api/auth') {
                        $obj.url = $urlObj -replace 'http://localhost:30001/api/auth', '{{auth_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed auth URL" -ForegroundColor Green
                    }
                    # Replace catalog/menu URLs (port 30002)
                    elseif ($urlObj -match 'localhost:30002/api/menu') {
                        $obj.url = $urlObj -replace 'http://localhost:30002/api/menu', '{{catalog_base_url}}/menu'
                        $script:replacements++
                        Write-Host "  ✓ Fixed catalog/menu URL" -ForegroundColor Green
                    }
                    # Replace crm/restaurant URLs (port 30002)
                    elseif ($urlObj -match 'localhost:30002/api/restaurant') {
                        $obj.url = $urlObj -replace 'http://localhost:30002/api/restaurant', '{{crm_base_url}}/restaurant'
                        $script:replacements++
                        Write-Host "  ✓ Fixed crm/restaurant URL" -ForegroundColor Green
                    }
                    # Replace operator URLs (port 30002)
                    elseif ($urlObj -match 'localhost:30002/api/operator') {
                        $obj.url = $urlObj -replace 'http://localhost:30002/api/operator', '{{order_base_url}}/operator'
                        $script:replacements++
                        Write-Host "  ✓ Fixed operator URL" -ForegroundColor Green
                    }
                    # Replace generic port 30002
                    elseif ($urlObj -match 'localhost:30002') {
                        $obj.url = $urlObj -replace 'http://localhost:30002', '{{gateway_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed generic port 30002" -ForegroundColor Green
                    }
                    # Replace port 30001
                    elseif ($urlObj -match 'localhost:30001') {
                        $obj.url = $urlObj -replace 'http://localhost:30001', '{{auth_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed generic port 30001" -ForegroundColor Green
                    }
                    # Replace gateway port 30500
                    elseif ($urlObj -match 'localhost:30500') {
                        $obj.url = $urlObj -replace 'http://localhost:30500', '{{gateway_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed gateway port 30500" -ForegroundColor Green
                    }
                }
                # Handle object URLs with 'raw' property
                elseif ($urlObj -is [System.Management.Automation.PSCustomObject] -and $urlObj.PSObject.Properties['raw']) {
                    $original = $urlObj.raw
                    
                    # Apply same replacements to raw URL
                    if ($urlObj.raw -match 'localhost:30001/api/auth') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30001/api/auth', '{{auth_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed auth URL (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30002/api/menu') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30002/api/menu', '{{catalog_base_url}}/menu'
                        $script:replacements++
                        Write-Host "  ✓ Fixed catalog/menu URL (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30002/api/restaurant') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30002/api/restaurant', '{{crm_base_url}}/restaurant'
                        $script:replacements++
                        Write-Host "  ✓ Fixed crm/restaurant URL (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30002/api/operator') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30002/api/operator', '{{order_base_url}}/operator'
                        $script:replacements++
                        Write-Host "  ✓ Fixed operator URL (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30002') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30002', '{{gateway_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed generic port 30002 (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30001') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30001', '{{gateway_base_url}}/api/auth'
                        $script:replacements++
                        Write-Host "  ✓ Fixed generic port 30001 (raw)" -ForegroundColor Green
                    }
                    elseif ($urlObj.raw -match 'localhost:30500') {
                        $urlObj.raw = $urlObj.raw -replace 'http://localhost:30500', '{{gateway_base_url}}'
                        $script:replacements++
                        Write-Host "  ✓ Fixed gateway port 30500 (raw)" -ForegroundColor Green
                    }
                }
            }
            
            # Recurse into all properties
            foreach ($prop in $obj.PSObject.Properties) {
                Fix-Urls $prop.Value
            }
        }
        elseif ($obj -is [Array]) {
            foreach ($item in $obj) {
                Fix-Urls $item
            }
        }
    }
    
    Fix-Urls $json
    
    if ($replacements -gt 0) {
        # Save back to file
        $json | ConvertTo-Json -Depth 100 | Set-Content $collection.FullName -Encoding UTF8
        Write-Host "  ✓ Saved $replacements replacements" -ForegroundColor Green
        $totalReplacements += $replacements
    } else {
        Write-Host "  ℹ No changes needed" -ForegroundColor Gray
    }
    
    Write-Host ""
}

Write-Host "=== Summary ===" -ForegroundColor Green
Write-Host "Total replacements: $totalReplacements" -ForegroundColor White
Write-Host ""

if ($totalReplacements -gt 0) {
    Write-Host "✓ Collections updated!" -ForegroundColor Green
} else {
    Write-Host "ℹ No changes needed" -ForegroundColor Gray
}
