#!/usr/bin/env pwsh
# Gateway Docker Deployment Script
# Builds and runs the API Gateway in Docker with access to Kubernetes services

param(
    [switch]$Rebuild,
    [switch]$Stop,
    [switch]$Logs,
    [switch]$Restart
)

$CONTAINER_NAME = "capstone-gateway"
$IMAGE_NAME = "capstone-gateway:latest"
$PORT = "5000"

function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

function Stop-Gateway {
    Write-ColorOutput Yellow "`n🛑 Stopping gateway container..."
    $existing = docker ps -a --filter "name=$CONTAINER_NAME" --format "{{.ID}}"
    if ($existing) {
        docker rm -f $existing | Out-Null
        Write-ColorOutput Green "✅ Gateway container stopped and removed"
    } else {
        Write-ColorOutput Yellow "⚠️  No running gateway container found"
    }
}

function Build-Gateway {
    Write-ColorOutput Yellow "`n🔨 Building gateway Docker image..."
    docker build -t $IMAGE_NAME .
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput Green "✅ Gateway image built successfully"
    } else {
        Write-ColorOutput Red "❌ Failed to build gateway image"
        exit 1
    }
}

function Start-Gateway {
    Write-ColorOutput Yellow "`n🚀 Starting gateway container..."
    docker run -d `
        --name $CONTAINER_NAME `
        -p ${PORT}:${PORT} `
        --add-host=host.docker.internal:host-gateway `
        $IMAGE_NAME

    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput Green "✅ Gateway container started successfully"
        Start-Sleep -Seconds 2
        Show-Status
    } else {
        Write-ColorOutput Red "❌ Failed to start gateway container"
        exit 1
    }
}

function Show-Status {
    Write-ColorOutput Cyan "`n📊 Gateway Status:"
    docker ps --filter "name=$CONTAINER_NAME" --format "table {{.ID}}\t{{.Image}}\t{{.Status}}\t{{.Ports}}"
    
    Write-ColorOutput Cyan "`n📋 Recent Logs:"
    docker logs $CONTAINER_NAME --tail 10
}

function Show-Logs {
    Write-ColorOutput Cyan "`n📋 Gateway Logs (follow with Ctrl+C to exit):"
    docker logs -f $CONTAINER_NAME
}

# Main execution
Write-ColorOutput Cyan @"
╔════════════════════════════════════════════════════════════╗
║          Capstone Gateway - Docker Deployment              ║
╚════════════════════════════════════════════════════════════╝
"@

if ($Stop) {
    Stop-Gateway
    exit 0
}

if ($Logs) {
    Show-Logs
    exit 0
}

if ($Restart) {
    Stop-Gateway
    Start-Gateway
    exit 0
}

# Check if container is already running
$running = docker ps --filter "name=$CONTAINER_NAME" --format "{{.ID}}"
if ($running -and -not $Rebuild) {
    Write-ColorOutput Yellow "`n⚠️  Gateway is already running!"
    Write-ColorOutput Cyan "Use -Stop to stop it, -Restart to restart it, or -Rebuild to rebuild and redeploy"
    Show-Status
    exit 0
}

# Stop existing container if rebuilding
if ($Rebuild -or $running) {
    Stop-Gateway
}

# Build and start
if ($Rebuild -or -not (docker images -q $IMAGE_NAME)) {
    Build-Gateway
}

Start-Gateway

Write-ColorOutput Green @"

✅ Gateway deployed successfully!

🌐 Access Points:
   - Health Check: http://localhost:$PORT/health
   - API Gateway: http://localhost:$PORT

📡 Backend Service Connections:
   - Authentication: http://host.docker.internal:30001
   - Catalog: http://host.docker.internal:30002
   - CRM: http://host.docker.internal:30003
   - Cart: http://host.docker.internal:30004

🔧 Management Commands:
   - View logs: docker logs -f $CONTAINER_NAME
   - Stop gateway: docker stop $CONTAINER_NAME
   - Restart gateway: docker restart $CONTAINER_NAME
   - Or use: .\docker-deploy.ps1 -Stop | -Logs | -Restart

"@
