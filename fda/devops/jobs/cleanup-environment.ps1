# Cleanup Deployment - Remove all Capstone services
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("local", "production", "all")]
    [string]$Environment
)

Write-Host "🧹 Cleaning up Capstone Services - $($Environment.ToUpper()) environment" -ForegroundColor Red

switch ($Environment) {
    "local" {
        Write-Host "🗑️ Removing local development deployment..." -ForegroundColor Yellow
        kubectl delete namespace capstone-services --ignore-not-found=true
        Write-Host "✅ Local environment cleaned up!" -ForegroundColor Green
    }
    "production" {
        Write-Host "🗑️ Removing production deployment..." -ForegroundColor Yellow
        Write-Host "⚠️  This will delete persistent volumes and data!" -ForegroundColor Red
        
        $confirm = Read-Host "Are you sure you want to delete PRODUCTION data? (yes/no)"
        if ($confirm -eq "yes") {
            kubectl delete namespace capstone-services --ignore-not-found=true
            Write-Host "✅ Production environment cleaned up!" -ForegroundColor Green
        } else {
            Write-Host "❌ Cleanup cancelled." -ForegroundColor Yellow
        }
    }
    "all" {
        Write-Host "🗑️ Removing ALL deployments..." -ForegroundColor Yellow
        Write-Host "⚠️  This will delete ALL persistent volumes and data!" -ForegroundColor Red
        
        $confirm = Read-Host "Are you sure you want to delete ALL environments and data? (yes/no)"
        if ($confirm -eq "yes") {
            kubectl delete namespace capstone-services --ignore-not-found=true
            Write-Host "✅ All environments cleaned up!" -ForegroundColor Green
        } else {
            Write-Host "❌ Cleanup cancelled." -ForegroundColor Yellow
        }
    }
}

Write-Host "`n📊 Verify cleanup:" -ForegroundColor Cyan
Write-Host "kubectl get namespaces | grep capstone" -ForegroundColor Gray