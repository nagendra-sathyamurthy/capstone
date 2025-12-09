#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Master deployment script - Complete workflow from verify to test
.DESCRIPTION
    Runs the complete deployment workflow:
    1. Verify - Check Kubernetes is ready
    2. Build - Build all Docker images
    3. Secrets - Apply Kubernetes secrets
    4. Deploy - Deploy all services to Kubernetes
    5. Seed - Seed sample data to services
    6. Test - Run Newman API tests
.EXAMPLE
    .\master-deploy.ps1
    Runs the complete deployment workflow
.EXAMPLE
    .\master-deploy.ps1 -SkipBuild
    Skips the build step (uses existing images)
.EXAMPLE
    .\master-deploy.ps1 -SkipSeed
    Skips the seed data step
#>

param(
    [switch]$SkipBuild,
    [switch]$SkipSeed,
    [switch]$SkipTest
)

$ErrorActionPreference = "Stop"

# Script metadata
$scriptName = "Master Deploy"
$scriptVersion = "1.0.0"

# Color functions
function Write-Step {
    param([string]$Message)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Blue
}

function Write-Error-Message {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Warning-Message {
    param([string]$Message)
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

# Header
Write-Host "`n╔════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║     MASTER DEPLOYMENT WORKFLOW         ║" -ForegroundColor Magenta
Write-Host "║     Version: $scriptVersion                    ║" -ForegroundColor Magenta
Write-Host "╚════════════════════════════════════════╝`n" -ForegroundColor Magenta

$startTime = Get-Date
Write-Info "Started at: $($startTime.ToString('yyyy-MM-dd HH:mm:ss'))"

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Track overall success
$overallSuccess = $true
$failedSteps = @()

try {
    # Step 1: Verify
    Write-Step "STEP 1: VERIFY KUBERNETES"
    Write-Info "Checking if Kubernetes cluster is ready..."
    
    $verifyScript = Join-Path $scriptDir "verify.ps1"
    if (Test-Path $verifyScript) {
        & $verifyScript
        if ($LASTEXITCODE -ne 0) {
            throw "Verification failed"
        }
        Write-Success "Kubernetes verification completed"
    } else {
        Write-Warning-Message "verify.ps1 not found, skipping verification"
    }

    # Step 2: Build
    if (-not $SkipBuild) {
        Write-Step "STEP 2: BUILD DOCKER IMAGES"
        Write-Info "Building all Docker images..."
        
        $buildScript = Join-Path $scriptDir "build.ps1"
        if (Test-Path $buildScript) {
            & $buildScript
            if ($LASTEXITCODE -ne 0) {
                throw "Build failed"
            }
            Write-Success "Docker images built successfully"
        } else {
            Write-Warning-Message "build.ps1 not found, skipping build"
        }
    } else {
        Write-Info "Skipping build step (using existing images)"
    }

    # Step 3: Secrets
    Write-Step "STEP 3: APPLY KUBERNETES SECRETS"
    Write-Info "Applying MongoDB secrets..."
    
    $secretsScript = Join-Path $scriptDir "secrets.ps1"
    if (Test-Path $secretsScript) {
        & $secretsScript
        if ($LASTEXITCODE -ne 0) {
            throw "Secrets application failed"
        }
        Write-Success "Kubernetes secrets applied successfully"
    } else {
        Write-Error-Message "secrets.ps1 not found"
        throw "Secrets script not found"
    }

    # Step 4: Deploy
    Write-Step "STEP 4: DEPLOY SERVICES"
    Write-Info "Deploying all services to Kubernetes..."
    
    $deployScript = Join-Path $scriptDir "deploy.ps1"
    if (Test-Path $deployScript) {
        & $deployScript
        if ($LASTEXITCODE -ne 0) {
            throw "Deployment failed"
        }
        Write-Success "Services deployed successfully"
    } else {
        Write-Error-Message "deploy.ps1 not found"
        throw "Deploy script not found"
    }

    # Wait for services to be ready
    Write-Info "Waiting 30 seconds for services to stabilize..."
    Start-Sleep -Seconds 30

    # Step 5: Seed
    if (-not $SkipSeed) {
        Write-Step "STEP 5: SEED SAMPLE DATA"
        Write-Info "Seeding menu items and sample data..."
        
        $seedScript = Join-Path $scriptDir "seed.ps1"
        if (Test-Path $seedScript) {
            & $seedScript
            if ($LASTEXITCODE -ne 0) {
                Write-Warning-Message "Seeding failed, but continuing..."
                $failedSteps += "Seed"
                $overallSuccess = $false
            } else {
                Write-Success "Sample data seeded successfully"
            }
        } else {
            Write-Warning-Message "seed.ps1 not found, skipping seed"
        }
    } else {
        Write-Info "Skipping seed step"
    }

    # Step 6: Test
    if (-not $SkipTest) {
        Write-Step "STEP 6: RUN API TESTS"
        Write-Info "Running Newman API tests..."
        
        $testScript = Join-Path $scriptDir "test.ps1"
        if (Test-Path $testScript) {
            & $testScript
            if ($LASTEXITCODE -ne 0) {
                Write-Warning-Message "Tests failed, but deployment is complete"
                $failedSteps += "Test"
                $overallSuccess = $false
            } else {
                Write-Success "API tests passed successfully"
            }
        } else {
            Write-Warning-Message "test.ps1 not found, skipping tests"
        }
    } else {
        Write-Info "Skipping test step"
    }

} catch {
    Write-Error-Message "Deployment workflow failed: $_"
    $overallSuccess = $false
    $failedSteps += $_.Exception.Message
}

# Summary
$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host "`n╔════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║          DEPLOYMENT SUMMARY            ║" -ForegroundColor Magenta
Write-Host "╚════════════════════════════════════════╝`n" -ForegroundColor Magenta

Write-Info "Started:  $($startTime.ToString('yyyy-MM-dd HH:mm:ss'))"
Write-Info "Finished: $($endTime.ToString('yyyy-MM-dd HH:mm:ss'))"
Write-Info "Duration: $($duration.ToString('hh\:mm\:ss'))"

if ($overallSuccess) {
    Write-Host "`n✅ DEPLOYMENT COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "`nServices are ready at:" -ForegroundColor Cyan
    Write-Host "  Gateway:      http://localhost:5000" -ForegroundColor White
    Write-Host "  Customer App: http://localhost:4200" -ForegroundColor White
    Write-Host "`nNext steps:" -ForegroundColor Yellow
    Write-Host "  1. Open browser to http://localhost:4200" -ForegroundColor White
    Write-Host "  2. Check service logs: kubectl logs -n capstone-services <pod-name>" -ForegroundColor White
    Write-Host "  3. Run additional tests from postman-collections/" -ForegroundColor White
    exit 0
} else {
    Write-Host "`n⚠️  DEPLOYMENT COMPLETED WITH WARNINGS" -ForegroundColor Yellow
    if ($failedSteps.Count -gt 0) {
        Write-Host "`nFailed/Warning Steps:" -ForegroundColor Yellow
        $failedSteps | ForEach-Object { Write-Host "  • $_" -ForegroundColor Red }
    }
    Write-Host "`nCore services may still be running. Check with:" -ForegroundColor Cyan
    Write-Host "  kubectl get pods -n capstone-services" -ForegroundColor White
    exit 1
}
