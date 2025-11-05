# Apply MongoDB Secrets to Kubernetes
# This script applies the updated MongoDB secrets configuration to the capstone-services namespace

Write-Host "Applying MongoDB secrets to Kubernetes..." -ForegroundColor Green

# Check if kubectl is available and cluster is accessible
try {
    $context = kubectl config current-context
    Write-Host "✓ Kubernetes context: $context" -ForegroundColor Green
}
catch {
    Write-Error "✗ kubectl not found or cluster not accessible"
    exit 1
}

# Check if namespace exists
$namespace = "capstone-services"
try {
    $nsExists = kubectl get namespace $namespace --ignore-not-found
    if ($nsExists) {
        Write-Host "✓ Namespace '$namespace' exists" -ForegroundColor Green
    } else {
        Write-Host "Creating namespace '$namespace'..." -ForegroundColor Yellow
        kubectl create namespace $namespace
    }
}
catch {
    Write-Error "✗ Failed to check/create namespace: $_"
    exit 1
}

# Apply secrets
$secretsPath = "../kubernetes/mongodb-secret.yaml"

try {
    Write-Host "Applying MongoDB secrets..." -ForegroundColor Cyan
    kubectl apply -f $secretsPath
    
    Write-Host "✓ MongoDB secrets applied successfully" -ForegroundColor Green
    
    # Verify secrets
    Write-Host "`nVerifying applied secrets:" -ForegroundColor Cyan
    kubectl get secrets -n $namespace | Where-Object { $_ -match "mongodb|authentication|catalog|crm|cart" }
    
    Write-Host "`nSecret details:" -ForegroundColor Cyan
    $secrets = @("mongodb-secret", "authentication-secret", "catalog-secret", "crm-secret", "cart-secret")
    
    foreach ($secret in $secrets) {
        Write-Host "- $secret" -ForegroundColor Gray
        $secretInfo = kubectl get secret $secret -n $namespace --ignore-not-found -o jsonpath='{.data}' 2>$null
        if ($secretInfo) {
            $keys = kubectl get secret $secret -n $namespace -o jsonpath='{.data}' | ConvertFrom-Json | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
            Write-Host "  Keys: $($keys -join ', ')" -ForegroundColor DarkGray
        } else {
            Write-Host "  Not found" -ForegroundColor Red
        }
    }
}
catch {
    Write-Error "✗ Failed to apply secrets: $_"
    exit 1
}

Write-Host "`n✓ MongoDB secrets configuration complete!" -ForegroundColor Green
Write-Host "Services can now access MongoDB credentials securely through Kubernetes secrets." -ForegroundColor Yellow

Write-Host "`nTo restart deployments and pick up new secrets:" -ForegroundColor Cyan
Write-Host "kubectl rollout restart deployment -n $namespace" -ForegroundColor Gray