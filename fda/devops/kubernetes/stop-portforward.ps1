# Stop all port forwarding processes

Write-Host "Stopping all kubectl port-forward processes..." -ForegroundColor Yellow

# Kill all kubectl processes (port-forward processes)
Get-Process kubectl -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "All port forwarding processes stopped." -ForegroundColor Green
Write-Host "Services are now only accessible via NodePort (30000-30004)." -ForegroundColor Cyan