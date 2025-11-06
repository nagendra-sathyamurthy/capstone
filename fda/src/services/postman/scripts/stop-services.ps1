# PowerShell Script to Stop All Capstone Services

Write-Host "🛑 Stopping Capstone Microservices..." -ForegroundColor Red

$BaseDir = "c:\dotnet\capstone\fda\src\services"
$PidFile = "$BaseDir\postman\test-results\service-pids.txt"

if (Test-Path $PidFile) {
    $Pids = Get-Content $PidFile
    
    foreach ($Pid in $Pids) {
        if ($Pid -and ($Pid -ne "")) {
            try {
                Stop-Process -Id $Pid -Force -ErrorAction SilentlyContinue
                Write-Host "✅ Stopped process $Pid" -ForegroundColor Green
            }
            catch {
                Write-Host "⚠️  Could not stop process $Pid (may already be stopped)" -ForegroundColor Yellow
            }
        }
    }
    
    # Clean up PID file
    Remove-Item $PidFile -Force -ErrorAction SilentlyContinue
} else {
    Write-Host "⚠️  No PID file found. Manually stopping dotnet processes..." -ForegroundColor Yellow
    
    # Find and stop all dotnet processes running our services
    $DotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    
    foreach ($Process in $DotnetProcesses) {
        try {
            $CommandLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $($Process.Id)").CommandLine
            if ($CommandLine -and ($CommandLine -match "authentication|catalog|crm|cart")) {
                Stop-Process -Id $Process.Id -Force
                Write-Host "✅ Stopped $($Process.ProcessName) (PID: $($Process.Id))" -ForegroundColor Green
            }
        }
        catch {
            # Skip if we can't access the process
        }
    }
}

Write-Host ""
Write-Host "✨ All services stopped!" -ForegroundColor Green