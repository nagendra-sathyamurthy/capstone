# Kubernetes Setup Guide for Capstone Project

## Current Status
- ✅ kubectl installed (v1.34.1)
- ✅ Docker Desktop installed and running
- ✅ All service Docker images built
- ✅ Services currently running via Docker Compose
- ⏳ **Kubernetes needs to be enabled in Docker Desktop**

## Enable Kubernetes in Docker Desktop

### Step-by-Step Instructions:

1. **Open Docker Desktop Settings**
   - Right-click the Docker Desktop icon in your system tray (bottom-right corner)
   - Click **"Settings"** or **"Dashboard"** → **"Settings"**

2. **Navigate to Kubernetes**
   - In the left sidebar, click on **"Kubernetes"**

3. **Enable Kubernetes**
   - Check the box: ☑ **"Enable Kubernetes"**
   - (Optional) Check: ☑ **"Show system containers (advanced)"**
   - Click **"Apply & Restart"**

4. **Wait for Startup**
   - Docker Desktop will restart
   - Wait 2-3 minutes for Kubernetes to initialize
   - You'll see **"Kubernetes is running"** in the bottom-left corner when ready

5. **Verify Installation**
   ```powershell
   # Run the verification script
   cd C:\dotnet\capstone\fda\devops\jobs
   .\setup-kubectl.ps1
   ```

## After Enabling Kubernetes

### Option 1: Deploy to Kubernetes

```powershell
# Stop Docker Compose services first
cd C:\dotnet\capstone\fda\devops\jobs
docker compose -f docker-compose-local.yml down

# Deploy to Kubernetes
cd C:\dotnet\capstone\fda\devops\kubernetes
.\deploy-full-stack-local.ps1
```

### Option 2: Keep Using Docker Compose

Your services are currently running successfully via Docker Compose on:
- Authentication: http://localhost:30001
- Catalog: http://localhost:30002
- CRM: http://localhost:30003
- Cart: http://localhost:30004
- Order: http://localhost:30005
- MongoDB: mongodb://localhost:30000

**You can continue using Docker Compose if it works for your needs!**

## Kubernetes vs Docker Compose

### Use Kubernetes when:
- Testing production-like deployments
- Working with multiple replicas/scaling
- Testing service mesh, ingress, or advanced networking
- Deploying gateway and customer-app together
- Need persistent volumes and StatefulSets

### Use Docker Compose when:
- Quick local development
- Simple testing
- Debugging individual services
- Faster startup times
- Less resource usage

## Useful Commands

### Kubectl Commands (after enabling Kubernetes)
```powershell
# Check cluster status
kubectl cluster-info
kubectl get nodes

# View all resources
kubectl get all -A

# Check specific namespace
kubectl get pods -n capstone-services
kubectl get services -n capstone-services

# View logs
kubectl logs -n capstone-services <pod-name>

# Delete everything
kubectl delete namespace capstone-services
```

### Docker Compose Commands (current setup)
```powershell
cd C:\dotnet\capstone\fda\devops\jobs

# View status
docker compose -f docker-compose-local.yml ps

# View logs
docker compose -f docker-compose-local.yml logs -f

# Stop services
docker compose -f docker-compose-local.yml down

# Restart specific service
docker compose -f docker-compose-local.yml restart authentication
```

## Configuration Files

- **Kubeconfig**: `C:\Users\Nagen\.kube\config` (auto-created when Kubernetes is enabled)
- **Docker Compose**: `C:\dotnet\capstone\fda\devops\jobs\docker-compose-local.yml`
- **Kubernetes Manifests**: `C:\dotnet\capstone\fda\devops\kubernetes\local\*.yaml`
- **Environment Variables**: `C:\dotnet\capstone\fda\devops\jobs\.env`

## Troubleshooting

### Kubernetes won't start
- Make sure Hyper-V or WSL2 is enabled
- Check Docker Desktop has enough resources (Settings → Resources)
- Restart Docker Desktop completely
- Check Windows Event Viewer for errors

### kubectl can't connect
```powershell
# Check if context is set
kubectl config current-context

# Set context to docker-desktop
kubectl config use-context docker-desktop

# Verify connection
kubectl cluster-info
```

### Port conflicts
If ports 30001-30005 are in use:
```powershell
# Stop Docker Compose services
docker compose -f docker-compose-local.yml down

# Or check what's using the ports
netstat -ano | findstr "30001"
```

## Next Steps

1. **Enable Kubernetes** (if you want to use it)
2. **Run setup script**: `.\setup-kubectl.ps1`
3. **Deploy services**: `.\deploy-full-stack-local.ps1`
4. **Test APIs**: Use Postman collections in `fda/postman-collections/`

## Current Deployment Status

✅ **All services are running via Docker Compose**
- Ready for API testing
- MongoDB is initialized
- Authentication, Catalog, CRM, Cart, and Order services are operational

You can continue with your current Docker Compose setup or migrate to Kubernetes when needed!
