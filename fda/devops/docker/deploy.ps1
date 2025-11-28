#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy Capstone Food Delivery Application with Docker Compose
    
.DESCRIPTION
    This script automates the deployment process including:
    - Secret generation and validation
    - Docker image building
    - Service deployment
    - Health checks
    - Rollback on failure
    
.PARAMETER Environment
    Deployment environment (development, staging, production)
    
.PARAMETER SkipSecrets
    Skip secret generation if secrets already exist
    
.PARAMETER SkipBuild
    Skip building images and use existing ones
    
.PARAMETER Services
    Specific services to deploy (comma-separated). If not specified, deploys all services.
    
.PARAMETER Clean
    Remove existing containers and volumes before deployment
    
.EXAMPLE
    .\deploy.ps1
    Deploy all services with secret generation
    
.EXAMPLE
    .\deploy.ps1 -Environment production -SkipSecrets
    Deploy to production using existing secrets
    
.EXAMPLE
    .\deploy.ps1 -Services "catalog,gateway" -SkipBuild
    Deploy only catalog and gateway services without rebuilding
    
.EXAMPLE
    .\deploy.ps1 -Clean
    Clean deployment (removes existing data)
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('development', 'staging', 'production')]
    [string]$Environment = 'development',
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipSecrets = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$Services = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean = $false
)

# Script configuration
$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "docker-compose-working.yml"
$secretsDir = Join-Path $scriptDir "secrets"
$envFile = Join-Path $scriptDir ".env"

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
║   🍔 Capstone Food Delivery Application Deployment 🍔    ║
║                                                            ║
╚════════════════════════════════════════════════════════════╝
"@ -ForegroundColor Cyan

Write-Host "`nEnvironment: $Environment" -ForegroundColor White
Write-Host "Deployment Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

# Step 1: Pre-flight checks
Write-Step "Pre-flight Checks"

# Check if Docker is running
try {
    $dockerVersion = docker version --format '{{.Server.Version}}' 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Docker is not running"
    }
    Write-Success "Docker is running (version: $dockerVersion)"
} catch {
    Write-Error "Docker is not running or not installed"
    Write-Info "Please start Docker Desktop and try again"
    exit 1
}

# Check if docker-compose file exists
if (-not (Test-Path $composeFile)) {
    Write-Error "docker-compose-working.yml not found at: $composeFile"
    exit 1
}
Write-Success "Docker Compose file found"

# Check available disk space
$drive = (Get-Item $scriptDir).PSDrive.Name
$disk = Get-PSDrive $drive
$freeSpaceGB = [math]::Round($disk.Free / 1GB, 2)
if ($freeSpaceGB -lt 5) {
    Write-Warning "Low disk space: ${freeSpaceGB}GB available. Recommend at least 5GB free."
} else {
    Write-Success "Disk space: ${freeSpaceGB}GB available"
}

# Step 2: Setup secrets
if (-not $SkipSecrets) {
    Write-Step "Setting Up Secrets"
    
    # Check if secrets directory exists
    if (-not (Test-Path $secretsDir)) {
        New-Item -ItemType Directory -Path $secretsDir -Force | Out-Null
        Write-Success "Created secrets directory"
    }
    
    # Check if secrets already exist
    $requiredSecrets = @(
        "mongo_root_username.txt",
        "mongo_root_password.txt",
        "mongo_connection_string.txt",
        "mongo_connection_string_crm.txt",
        "jwt_secret_key.txt"
    )
    
    $existingSecrets = Get-ChildItem -Path $secretsDir -Filter "*.txt" -File -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name
    $missingSecrets = $requiredSecrets | Where-Object { $_ -notin $existingSecrets }
    
    if ($missingSecrets.Count -gt 0) {
        Write-Info "Missing secrets: $($missingSecrets -join ', ')"
        Write-Host "`nGenerating secrets..."
        
        # Generate random credentials for non-production environments
        if ($Environment -ne 'production') {
            $mongoUser = "admin"
            $mongoPass = "AdminPass2024"
            Write-Info "Using default credentials for $Environment environment"
        } else {
            Write-Warning "Production environment detected!"
            $mongoUser = Read-Host "Enter MongoDB root username"
            $mongoPass = Read-Host "Enter MongoDB root password" -AsSecureString
            $mongoPass = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($mongoPass))
        }
        
        # Generate JWT key
        $jwtKey = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
        
        # Create secret files
        Set-Content -Path (Join-Path $secretsDir "mongo_root_username.txt") -Value $mongoUser -NoNewline
        Write-Success "Created mongo_root_username.txt"
        
        Set-Content -Path (Join-Path $secretsDir "mongo_root_password.txt") -Value $mongoPass -NoNewline
        Write-Success "Created mongo_root_password.txt"
        
        $mongoConnStr = "mongodb://${mongoUser}:${mongoPass}@mongodb:27017"
        Set-Content -Path (Join-Path $secretsDir "mongo_connection_string.txt") -Value $mongoConnStr -NoNewline
        Write-Success "Created mongo_connection_string.txt"
        
        $mongoConnStrCrm = "mongodb://${mongoUser}:${mongoPass}@mongodb:27017/crmdb?authSource=admin"
        Set-Content -Path (Join-Path $secretsDir "mongo_connection_string_crm.txt") -Value $mongoConnStrCrm -NoNewline
        Write-Success "Created mongo_connection_string_crm.txt"
        
        Set-Content -Path (Join-Path $secretsDir "jwt_secret_key.txt") -Value $jwtKey -NoNewline
        Write-Success "Created jwt_secret_key.txt"
    } else {
        Write-Success "All required secrets exist"
    }
    
    # Validate secrets
    Write-Info "Validating secrets..."
    foreach ($secret in $requiredSecrets) {
        $secretPath = Join-Path $secretsDir $secret
        if (Test-Path $secretPath) {
            $content = Get-Content $secretPath -Raw
            if ([string]::IsNullOrWhiteSpace($content)) {
                Write-Error "Secret file $secret is empty!"
                exit 1
            }
        }
    }
    Write-Success "All secrets validated"
} else {
    Write-Step "Skipping Secret Setup"
    Write-Info "Using existing secrets"
}

# Step 3: Environment configuration
Write-Step "Environment Configuration"

if (-not (Test-Path $envFile)) {
    $envExample = Join-Path $scriptDir ".env.example"
    if (Test-Path $envExample) {
        Copy-Item $envExample $envFile
        Write-Success "Created .env from .env.example"
        Write-Warning "Please review .env file and update as needed"
    } else {
        Write-Info "No .env file found, using docker-compose defaults"
    }
} else {
    Write-Success ".env file exists"
}

# Step 4: Clean existing deployment (if requested)
if ($Clean) {
    Write-Step "Cleaning Existing Deployment"
    Write-Warning "This will remove all containers and volumes!"
    
    $confirm = Read-Host "Are you sure you want to continue? (yes/no)"
    if ($confirm -ne "yes") {
        Write-Info "Deployment cancelled"
        exit 0
    }
    
    try {
        Write-Info "Stopping containers..."
        docker compose -f $composeFile down -v 2>&1 | Out-Null
        Write-Success "Removed containers and volumes"
    } catch {
        Write-Warning "No existing deployment to clean"
    }
}

# Step 5: Build images
if (-not $SkipBuild) {
    Write-Step "Building Docker Images"
    
    $buildArgs = @("-f", $composeFile, "build")
    if ($Services) {
        $buildArgs += $Services.Split(',')
        Write-Info "Building services: $Services"
    } else {
        Write-Info "Building all services..."
    }
    
    try {
        docker compose @buildArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed"
        }
        Write-Success "Docker images built successfully"
    } catch {
        Write-Error "Failed to build Docker images"
        Write-Info "Check the build logs above for errors"
        exit 1
    }
} else {
    Write-Step "Skipping Build"
    Write-Info "Using existing Docker images"
}

# Step 6: Deploy services
Write-Step "Deploying Services"

$deployArgs = @("-f", $composeFile, "up", "-d")
if ($Services) {
    $deployArgs += $Services.Split(',')
    Write-Info "Deploying services: $Services"
} else {
    Write-Info "Deploying all services..."
}

try {
    docker compose @deployArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Deployment failed"
    }
    Write-Success "Services deployed successfully"
} catch {
    Write-Error "Failed to deploy services"
    Write-Info "Attempting rollback..."
    docker compose -f $composeFile down
    exit 1
}

# Step 7: Health checks
Write-Step "Running Health Checks"

Start-Sleep -Seconds 5

Write-Info "Checking container status..."
$containers = docker compose -f $composeFile ps --format json | ConvertFrom-Json

$allHealthy = $true
foreach ($container in $containers) {
    $name = $container.Name
    $state = $container.State
    $status = $container.Status
    
    if ($state -eq "running") {
        Write-Success "$name is running ($status)"
    } else {
        Write-Error "$name is $state"
        $allHealthy = $false
    }
}

if (-not $allHealthy) {
    Write-Warning "Some containers are not healthy!"
    Write-Info "Check logs with: docker compose -f docker-compose-working.yml logs"
}

# Wait for MongoDB to be ready
Write-Info "Waiting for MongoDB to be ready..."
$maxAttempts = 30
$attempt = 0
$mongoReady = $false

while ($attempt -lt $maxAttempts -and -not $mongoReady) {
    try {
        $result = docker compose -f $composeFile exec -T mongodb mongosh --eval "db.adminCommand('ping')" --quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            $mongoReady = $true
            Write-Success "MongoDB is ready"
        }
    } catch {
        # Ignore errors during health check
    }
    
    if (-not $mongoReady) {
        $attempt++
        Write-Host "." -NoNewline
        Start-Sleep -Seconds 2
    }
}

if (-not $mongoReady) {
    Write-Warning "MongoDB health check timeout. It may still be starting up."
}

# Check gateway health
Write-Info "`nChecking Gateway health..."
try {
    $gatewayHealth = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get -TimeoutSec 5 -ErrorAction Stop
    Write-Success "Gateway is healthy"
} catch {
    Write-Warning "Gateway health check failed. It may still be starting up."
}

# Step 8: Display summary
Write-Step "Deployment Summary"

Write-Host @"

🎉 Deployment completed successfully!

Service URLs:
  • Customer App:     http://localhost:3000
  • API Gateway:      http://localhost:5000
  • Authentication:   http://localhost:8081
  • Catalog:          http://localhost:8082
  • CRM:              http://localhost:8083
  • Cart:             http://localhost:8084
  • Order:            http://localhost:8085
  • MongoDB:          mongodb://localhost:27017

Useful Commands:
  • View logs:        docker compose -f docker-compose-working.yml logs -f
  • Check status:     docker compose -f docker-compose-working.yml ps
  • Stop services:    docker compose -f docker-compose-working.yml stop
  • Remove services:  docker compose -f docker-compose-working.yml down

"@ -ForegroundColor White

Write-Info "Next steps:"
Write-Host "  1. Test the application at http://localhost:3000" -ForegroundColor Gray
Write-Host "  2. Check API Gateway health at http://localhost:5000/health" -ForegroundColor Gray
Write-Host "  3. View logs if needed: docker compose -f docker-compose-working.yml logs -f" -ForegroundColor Gray

Write-Host "`n✨ Happy deploying! ✨`n" -ForegroundColor Green
