#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Stop and cleanup Capstone Food Delivery Application deployment
    
.DESCRIPTION
    This script provides various cleanup options:
    - Stop services
    - Remove containers
    - Remove volumes (data)
    - Remove images
    - Complete cleanup
    
.PARAMETER Stop
    Stop services but keep containers
    
.PARAMETER Down
    Stop and remove containers (keeps volumes)
    
.PARAMETER Clean
    Stop, remove containers and volumes (removes all data)
    
.PARAMETER Prune
    Complete cleanup including images
    
.PARAMETER Services
    Specific services to stop (comma-separated)
    
.EXAMPLE
    .\cleanup.ps1 -Stop
    Stop all services
    
.EXAMPLE
    .\cleanup.ps1 -Down
    Stop and remove containers (keeps data)
    
.EXAMPLE
    .\cleanup.ps1 -Clean
    Stop, remove containers and volumes (removes all data)
    
.EXAMPLE
    .\cleanup.ps1 -Prune
    Complete cleanup including images
    
.EXAMPLE
    .\cleanup.ps1 -Services "catalog,gateway" -Down
    Stop and remove only catalog and gateway services
#>

param(
    [Parameter(Mandatory=$false)]
    [switch]$Stop = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Down = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Prune = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$Services = ""
)

# Script configuration
$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "docker-compose-working.yml"

# Color output functions
function Write-Step {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Blue
}

# Banner
Clear-Host
Write-Host @"
╔════════════════════════════════════════════════════════════╗
║                                                            ║
║   🧹 Capstone Application Cleanup 🧹                      ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
"@ -ForegroundColor Yellow

Write-Host ""

# Validate parameters
$actionCount = ($Stop -eq $true) + ($Down -eq $true) + ($Clean -eq $true) + ($Prune -eq $true)
if ($actionCount -eq 0) {
    Write-Info "Usage:"
    Write-Host "  .\cleanup.ps1 -Stop          # Stop services (keep containers)" -ForegroundColor Gray
    Write-Host "  .\cleanup.ps1 -Down          # Stop and remove containers" -ForegroundColor Gray
    Write-Host "  .\cleanup.ps1 -Clean         # Remove containers and volumes (data loss!)" -ForegroundColor Gray
    Write-Host "  .\cleanup.ps1 -Prune         # Complete cleanup including images" -ForegroundColor Gray
    Write-Host ""
    exit 0
}

if ($actionCount -gt 1) {
    Write-Error "Please specify only one action: -Stop, -Down, -Clean, or -Prune"
    exit 1
}

# Check if Docker is running
try {
    docker version --format '{{.Server.Version}}' 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker is not running"
    }
} catch {
    Write-Error "Docker is not running or not installed"
    exit 1
}

# Check if docker-compose file exists
if (-not (Test-Path $composeFile)) {
    Write-Error "docker-compose-working.yml not found"
    exit 1
}

# Execute action
if ($Stop) {
    Write-Step "Stopping Services"
    
    $stopArgs = @("-f", $composeFile, "stop")
    if ($Services) {
        $stopArgs += $Services.Split(',')
        Write-Info "Stopping services: $Services"
    } else {
        Write-Info "Stopping all services..."
    }
    
    try {
        docker compose @stopArgs
        Write-Success "Services stopped"
        Write-Info "Containers are still present. Use -Down to remove them."
    } catch {
        Write-Error "Failed to stop services"
        exit 1
    }
}

if ($Down) {
    Write-Step "Removing Containers"
    
    $downArgs = @("-f", $composeFile, "down")
    if ($Services) {
        Write-Warning "Removing specific services..."
        # For specific services, we need to stop them first
        docker compose -f $composeFile stop $Services.Split(',')
        docker compose -f $composeFile rm -f $Services.Split(',')
        Write-Success "Services removed: $Services"
    } else {
        Write-Info "Removing all containers..."
        docker compose @downArgs
        Write-Success "All containers removed"
        Write-Info "Volumes are preserved. Use -Clean to remove data."
    }
}

if ($Clean) {
    Write-Step "Cleaning Up (Removing Data)"
    
    Write-Warning "⚠️  WARNING: This will delete all data including MongoDB databases!"
    Write-Host ""
    $confirm = Read-Host "Are you sure you want to continue? Type 'yes' to confirm"
    
    if ($confirm -ne "yes") {
        Write-Info "Cleanup cancelled"
        exit 0
    }
    
    Write-Info "Stopping and removing containers and volumes..."
    try {
        docker compose -f $composeFile down -v
        Write-Success "Containers and volumes removed"
        Write-Warning "All application data has been deleted!"
    } catch {
        Write-Error "Failed to cleanup"
        exit 1
    }
}

if ($Prune) {
    Write-Step "Complete Cleanup (Including Images)"
    
    Write-Warning "⚠️  WARNING: This will:"
    Write-Host "  • Stop all containers" -ForegroundColor Red
    Write-Host "  • Remove all containers" -ForegroundColor Red
    Write-Host "  • Remove all volumes (data loss!)" -ForegroundColor Red
    Write-Host "  • Remove all application images" -ForegroundColor Red
    Write-Host ""
    $confirm = Read-Host "Are you sure you want to continue? Type 'yes' to confirm"
    
    if ($confirm -ne "yes") {
        Write-Info "Cleanup cancelled"
        exit 0
    }
    
    Write-Info "Removing containers and volumes..."
    try {
        docker compose -f $composeFile down -v
        Write-Success "Containers and volumes removed"
    } catch {
        Write-Warning "No containers to remove"
    }
    
    Write-Info "Removing application images..."
    try {
        $images = @(
            "docker-authentication",
            "docker-catalog",
            "docker-cart",
            "docker-crm",
            "docker-order",
            "docker-gateway",
            "docker-customer-app"
        )
        
        foreach ($image in $images) {
            $imageExists = docker images -q $image
            if ($imageExists) {
                docker rmi $image -f
                Write-Success "Removed image: $image"
            }
        }
    } catch {
        Write-Warning "Some images could not be removed"
    }
    
    Write-Info "Running Docker system prune..."
    docker system prune -f
    
    Write-Success "Complete cleanup finished"
    Write-Warning "All application data and images have been removed!"
}

# Display summary
Write-Host "`n" -NoNewline
Write-Host "═══════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Cleanup completed successfully!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Show current status
Write-Info "Current status:"
try {
    $runningContainers = docker compose -f $composeFile ps --format json | ConvertFrom-Json
    if ($runningContainers) {
        Write-Host "`nRunning containers:" -ForegroundColor White
        foreach ($container in $runningContainers) {
            Write-Host "  • $($container.Name): $($container.State)" -ForegroundColor Gray
        }
    } else {
        Write-Success "No containers running"
    }
} catch {
    Write-Success "No containers running"
}

Write-Host ""
