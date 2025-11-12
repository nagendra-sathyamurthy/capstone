# PowerShell script to update solution files with new project references

param(
    [string]$SolutionPath,
    [string]$OldProjectName, 
    [string]$NewProjectName,
    [string]$OldFolderName,
    [string]$NewFolderName
)

if (-not $SolutionPath -or -not $OldProjectName -or -not $NewProjectName) {
    Write-Host "Usage: update-solution.ps1 -SolutionPath <path> -OldProjectName <old> -NewProjectName <new> -OldFolderName <oldFolder> -NewFolderName <newFolder>"
    exit 1
}

Write-Host "Processing solution: $SolutionPath"

# Read solution file content
$content = Get-Content -Path $SolutionPath -Raw

# Replace project references
$content = $content -replace "$OldProjectName", "$NewProjectName"
$content = $content -replace "$OldFolderName\\$OldProjectName", "$NewFolderName\\$NewProjectName"

# Write back to file
Set-Content -Path $SolutionPath -Value $content -NoNewline

Write-Host "Solution file updated: $SolutionPath"