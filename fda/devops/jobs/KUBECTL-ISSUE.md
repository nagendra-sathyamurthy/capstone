# kubectl Issue and Workarounds

## Problem

Rancher Desktop's `kubectl` is actually `kuberlr` (kubectl version manager) which attempts to download specific Kubernetes versions. This fails when:
- The requested version doesn't exist (e.g., v1.34.2)
- Corporate firewall blocks downloads from dl.k8s.io
- Network restrictions prevent HTTPS access

Error message:
```
kuberlr: ensure compatible kubectl available: error while trying to get contents of 
https://dl.k8s.io/release/vX.XX.X/bin/windows/amd64/kubectl.exe.sha512: 
GET ... returned http status 403 Forbidden
```

## Solutions

### ✅ Solution 1: Use Docker Compose (Recommended for Local Development)

Instead of Kubernetes, use Docker Compose which we've already configured:

```powershell
cd c:\dotnet\capstone\fda\devops\jobs

# Start all services
docker-compose -f docker-compose-local.yml up -d

# Check status
docker ps --filter "name=capstone"

# View logs
docker-compose -f docker-compose-local.yml logs -f

# Stop services
docker-compose -f docker-compose-local.yml down
```

**Services will be available at:**
- Authentication: http://localhost:30001
- Catalog: http://localhost:30002
- CRM: http://localhost:30003
- Cart: http://localhost:30004
- MongoDB: mongodb://localhost:30000

### ✅ Solution 2: Use rdctl shell

Execute kubectl commands through Rancher Desktop's shell:

```powershell
# Get pods
rdctl shell kubectl get pods

# Get services
rdctl shell kubectl get svc

# Apply configuration
rdctl shell kubectl apply -f namespace.yaml

# Get all resources
rdctl shell kubectl get all -A
```

### ✅ Solution 3: Install Standalone kubectl

Download kubectl separately from Rancher Desktop:

1. **Using Chocolatey (if available):**
   ```powershell
   choco install kubernetes-cli
   ```

2. **Manual download** (if corporate firewall allows):
   ```powershell
   # Download a stable version
   $version = "v1.31.0"
   $url = "https://dl.k8s.io/release/$version/bin/windows/amd64/kubectl.exe"
   Invoke-WebRequest -Uri $url -OutFile "$env:USERPROFILE\bin\kubectl.exe"
   
   # Add to PATH
   $env:PATH = "$env:USERPROFILE\bin;$env:PATH"
   ```

3. **Copy from another machine:**
   - Download kubectl.exe on a machine with internet access
   - Copy to your development machine
   - Place in a directory in your PATH

### ✅ Solution 4: Disable kuberlr (Advanced)

If you have admin rights:

```powershell
# Rename kuberlr to prevent it from running
Rename-Item "C:\Program Files\Rancher Desktop\resources\resources\win32\bin\kubectl.exe" "kubectl-kuberlr.exe"

# Copy your own kubectl binary to replace it
Copy-Item "path\to\real\kubectl.exe" "C:\Program Files\Rancher Desktop\resources\resources\win32\bin\kubectl.exe"
```

## Recommendation

**For local development: Use Docker Compose (Solution 1)**

The Docker Compose setup is:
- ✅ Simpler to use
- ✅ Doesn't require Kubernetes knowledge
- ✅ No kubectl/kuberlr issues
- ✅ Easier to troubleshoot
- ✅ Better for local development
- ✅ Already working and tested

**For production/staging: Use Kubernetes**

Use the kubernetes YAML files in `fda/devops/kubernetes/local` or `production` directories when deploying to actual Kubernetes clusters (AKS, EKS, GKE, etc.) where kubectl works properly.

## Current Status

- ✅ Docker Compose deployment: **Working**
- ❌ Kubernetes local deployment: **Blocked by kubectl/kuberlr issue**
- ✅ Services running: All 4 services + MongoDB operational
- ✅ Security: Credentials moved to environment variables

## Next Steps

Continue development using Docker Compose. When ready for Kubernetes deployment to cloud:
1. Ensure cloud provider's kubectl works
2. Use `rdctl shell kubectl` as backup
3. Deploy using existing YAML files in `fda/devops/kubernetes/`
