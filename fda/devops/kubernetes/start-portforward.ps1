# Port Forward Script to match Docker Compose ports exactly
# This script sets up port forwarding to make Kubernetes services accessible on the same ports as Docker Compose

Write-Host "Setting up port forwarding to match Docker Compose ports..." -ForegroundColor Green

# Start port forwarding in the background for each service
Write-Host "Starting port forwarding for MongoDB (9000 -> 27017)..." -ForegroundColor Yellow
Start-Process -FilePath "kubectl" -ArgumentList "port-forward", "svc/mongodb-service", "9000:27017", "-n", "capstone-services" -WindowStyle Hidden

Write-Host "Starting port forwarding for Authentication (5001 -> 5001)..." -ForegroundColor Yellow  
Start-Process -FilePath "kubectl" -ArgumentList "port-forward", "svc/authentication-service", "5001:5001", "-n", "capstone-services" -WindowStyle Hidden

Write-Host "Starting port forwarding for Catalog (5002 -> 5002)..." -ForegroundColor Yellow
Start-Process -FilePath "kubectl" -ArgumentList "port-forward", "svc/catalog-service", "5002:5002", "-n", "capstone-services" -WindowStyle Hidden

Write-Host "Starting port forwarding for CRM (5003 -> 5003)..." -ForegroundColor Yellow
Start-Process -FilePath "kubectl" -ArgumentList "port-forward", "svc/crm-service", "5003:5003", "-n", "capstone-services" -WindowStyle Hidden

Write-Host "Starting port forwarding for Cart (5004 -> 5004)..." -ForegroundColor Yellow
Start-Process -FilePath "kubectl" -ArgumentList "port-forward", "svc/cart-service", "5004:5004", "-n", "capstone-services" -WindowStyle Hidden

Start-Sleep 3

Write-Host "`nPort forwarding active! Services now accessible on Docker Compose ports:" -ForegroundColor Green
Write-Host "MongoDB:        localhost:9000" -ForegroundColor White
Write-Host "Authentication: http://localhost:5001" -ForegroundColor White
Write-Host "Catalog:        http://localhost:5002" -ForegroundColor White
Write-Host "CRM:            http://localhost:5003" -ForegroundColor White
Write-Host "Cart:           http://localhost:5004" -ForegroundColor White

Write-Host "`nPress Ctrl+C to stop port forwarding and exit." -ForegroundColor Cyan
Write-Host "Or run 'stop-portforward.ps1' to stop all port forwarding processes." -ForegroundColor Cyan

# Keep the script running
try {
    while ($true) {
        Start-Sleep 1
    }
}
catch [System.Management.Automation.PipelineStoppedException] {
    Write-Host "`nStopping port forwarding..." -ForegroundColor Yellow
    Get-Process kubectl -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "Port forwarding stopped." -ForegroundColor Green
}