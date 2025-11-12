# PowerShell script to update namespaces from Services to API

param(
    [string]$ServicePath,
    [string]$OldNamespace,
    [string]$NewNamespace
)

if (-not $ServicePath -or -not $OldNamespace -or -not $NewNamespace) {
    Write-Host "Usage: update-namespaces.ps1 -ServicePath <path> -OldNamespace <old> -NewNamespace <new>"
    exit 1
}

# Get all C# files in the service directory
$files = Get-ChildItem -Path $ServicePath -Filter "*.cs" -Recurse

foreach ($file in $files) {
    Write-Host "Processing: $($file.FullName)"
    
    # Read file content
    $content = Get-Content -Path $file.FullName -Raw
    
    # Replace namespace declarations
    $content = $content -replace "namespace $OldNamespace", "namespace $NewNamespace"
    
    # Replace using statements
    $content = $content -replace "using $OldNamespace", "using $NewNamespace"
    
    # Write back to file
    Set-Content -Path $file.FullName -Value $content -NoNewline
}

Write-Host "Namespace update completed for $ServicePath"