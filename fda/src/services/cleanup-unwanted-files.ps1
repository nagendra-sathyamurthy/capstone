# PowerShell Script to Clean Up Unwanted Files in Food Delivery Microservices
# Usage: .\cleanup-unwanted-files.ps1

param(
    [switch]$DryRun = $false,
    [switch]$Verbose = $false
)

Write-Host "Food Delivery API - Cleanup Unwanted Files" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Green

if ($DryRun) {
    Write-Host "DRY RUN MODE - No files will be deleted" -ForegroundColor Yellow
}

$RootPath = Get-Location

# Define patterns for unwanted files and directories
$UnwantedDirectories = @("bin", "obj", "logs", ".vs", "node_modules")
$UnwantedFilePatterns = @("*.tmp", "*.temp", "*.log", "*.cache", "*.bak", "*.old", "*.orig", "*~", "*.swp")

$DeletedDirs = 0
$DeletedFiles = 0
$TotalSize = 0

# Function to format file size
function Format-FileSize([long]$size) {
    if ($size -gt 1GB) { return "{0:N2} GB" -f ($size / 1GB) }
    elseif ($size -gt 1MB) { return "{0:N2} MB" -f ($size / 1MB) }
    elseif ($size -gt 1KB) { return "{0:N2} KB" -f ($size / 1KB) }
    else { return "$size bytes" }
}

# Clean up unwanted directories
Write-Host "`nCleaning unwanted directories..." -ForegroundColor Cyan
foreach ($dirName in $UnwantedDirectories) {
    $dirs = Get-ChildItem -Recurse -Directory | Where-Object { $_.Name -eq $dirName }
    foreach ($dir in $dirs) {
        $size = (Get-ChildItem -Path $dir.FullName -Recurse -File -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
        $TotalSize += $size
        $sizeStr = Format-FileSize $size
        
        if ($Verbose -or $DryRun) {
            Write-Host "  Found: $($dir.FullName) ($sizeStr)" -ForegroundColor Gray
        }
        
        if (-not $DryRun) {
            Remove-Item -Path $dir.FullName -Recurse -Force -ErrorAction SilentlyContinue
            Write-Host "  Deleted: $($dir.FullName) ($sizeStr)" -ForegroundColor Red
        }
        $DeletedDirs++
    }
}

# Clean up unwanted files
Write-Host "`nCleaning unwanted files..." -ForegroundColor Cyan
foreach ($pattern in $UnwantedFilePatterns) {
    $files = Get-ChildItem -Recurse -File | Where-Object { $_.Name -like $pattern }
    foreach ($file in $files) {
        $size = $file.Length
        $TotalSize += $size
        $sizeStr = Format-FileSize $size
        
        if ($Verbose -or $DryRun) {
            Write-Host "  Found: $($file.FullName) ($sizeStr)" -ForegroundColor Gray
        }
        
        if (-not $DryRun) {
            Remove-Item -Path $file.FullName -Force -ErrorAction SilentlyContinue
            Write-Host "  Deleted: $($file.FullName) ($sizeStr)" -ForegroundColor Red
        }
        $DeletedFiles++
    }
}

# Check for empty directories and remove them
Write-Host "`nCleaning empty directories..." -ForegroundColor Cyan
do {
    $EmptyDirs = Get-ChildItem -Recurse -Directory | Where-Object { (Get-ChildItem $_.FullName -Force | Measure-Object).Count -eq 0 }
    foreach ($emptyDir in $EmptyDirs) {
        if ($Verbose -or $DryRun) {
            Write-Host "  Found empty: $($emptyDir.FullName)" -ForegroundColor Gray
        }
        
        if (-not $DryRun) {
            Remove-Item -Path $emptyDir.FullName -Force -ErrorAction SilentlyContinue
            Write-Host "  Deleted empty: $($emptyDir.FullName)" -ForegroundColor Red
        }
    }
} while ($EmptyDirs.Count -gt 0 -and -not $DryRun)

# Summary
Write-Host "`nCleanup Summary:" -ForegroundColor Green
Write-Host "=================" -ForegroundColor Green
Write-Host "Directories cleaned: $DeletedDirs" -ForegroundColor White
Write-Host "Files cleaned: $DeletedFiles" -ForegroundColor White
Write-Host "Total space saved: $(Format-FileSize $TotalSize)" -ForegroundColor White

if ($DryRun) {
    Write-Host "`nTo actually delete these files, run without -DryRun parameter" -ForegroundColor Yellow
}

# Check git status
Write-Host "`nChecking git status..." -ForegroundColor Cyan
git status --porcelain

Write-Host "`nCleanup completed!" -ForegroundColor Green