# Quick Deployment Guide

Get the Capstone Food Delivery Application running in minutes!

## Prerequisites

- Docker Desktop installed and running
- Kubernetes enabled in Docker Desktop (Settings → Kubernetes → Enable)
- PowerShell (Windows) or Bash (Linux/Mac)
- 8GB+ RAM available

## Option 1: Kubernetes (Recommended)

### Deploy Complete Stack

```powershell
# Navigate to Kubernetes directory
cd devops/kubernetes

# Deploy everything with one command
.\deploy-full-stack-local.ps1
```

**Access the application:**
- Customer App: http://localhost:30080
- API Gateway: http://localhost:30005

### Deploy Individual Components

```powershell
# Deploy Gateway only
.\deploy-gateway-local.ps1

# Deploy Customer App only
.\deploy-customer-app-local.ps1
```

### Verify Deployment

```powershell
# Check all pods
kubectl get pods -A

# Check services
kubectl get services -A

# View customer app logs
kubectl logs -n capstone-frontend -l app=customer-app --tail=50 -f
```

### Cleanup

```powershell
# Delete customer app
kubectl delete -f local/customer-app.yaml

# Delete gateway
kubectl delete -f local/gateway.yaml

# Delete everything
kubectl delete namespace capstone-frontend capstone-gateway capstone-services
```

## Option 2: Docker Compose

### Full Stack Deployment

```bash
# Navigate to docker directory
cd devops/docker

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Check status
docker-compose ps
```

**Access the application:**
- Customer App: http://localhost:3000
- API Gateway: http://localhost:5000

### Development Mode (MongoDB + Gateway Only)

```bash
# Start minimal services
docker-compose -f docker-compose.dev.yml up -d

# Run backend services in Kubernetes
# Run customer-app with: cd src/customer-app && npm start
```

### Cleanup

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (delete data)
docker-compose down -v
```

## Common Commands

### Kubernetes

```bash
# View all resources
kubectl get all -A

# Get pod details
kubectl describe pod [pod-name] -n [namespace]

# View logs
kubectl logs [pod-name] -n [namespace] --tail=100 -f

# Execute command in pod
kubectl exec -it [pod-name] -n [namespace] -- sh

# Port forward
kubectl port-forward -n [namespace] [pod-name] [local-port]:[pod-port]

# Restart deployment
kubectl rollout restart deployment/[deployment-name] -n [namespace]
```

### Docker Compose

```bash
# View logs for specific service
docker-compose logs -f gateway

# Restart service
docker-compose restart gateway

# Rebuild and restart
docker-compose up -d --build gateway

# Scale service
docker-compose up -d --scale gateway=3

# Execute command in container
docker-compose exec gateway sh
```

## Troubleshooting

### Services not accessible

1. **Check if pods are running:**
   ```bash
   kubectl get pods -A
   ```

2. **Check pod logs:**
   ```bash
   kubectl logs -n capstone-frontend -l app=customer-app
   ```

3. **Verify ports are not in use:**
   ```bash
   netstat -ano | findstr :30080
   ```

### MongoDB connection issues

```bash
# Test MongoDB connection
kubectl exec -n capstone-services mongodb-deployment-[pod-id] -- mongosh -u admin -p AdminPass2024 --eval "db.adminCommand('ping')"

# Docker Compose
docker-compose exec mongodb mongosh -u admin -p AdminPass2024 --eval "db.adminCommand('ping')"
```

### Image pull errors

For local Kubernetes:
- Ensure `imagePullPolicy: Never` in YAML
- Build images before deploying

### Gateway can't reach backend services

```bash
# Kubernetes - Test connectivity
kubectl exec -n capstone-gateway gateway-[pod-id] -- wget -qO- http://crm-service.capstone-services.svc.cluster.local:8080/health

# Docker Compose - Test connectivity
docker-compose exec gateway curl http://crm:8080/health
```

## Test the Application

1. **Open Customer App:** http://localhost:30080
2. **Register/Login:** Enter phone number and OTP
3. **Browse Restaurants:** View available restaurants
4. **Add to Cart:** Select items
5. **Checkout:** Add delivery address
6. **Payment:** Complete mock payment
7. **View Orders:** Check order history in profile

## Next Steps

- Review the [DEPLOYMENT-ARCHITECTURE.md](DEPLOYMENT-ARCHITECTURE.md) for detailed information
- Check [devops/docker/README.md](devops/docker/README.md) for Docker-specific details
- Update MongoDB credentials for production
- Set up monitoring and logging
- Configure CI/CD pipeline

## Getting Help

**View service health:**
```bash
# Customer App
curl http://localhost:30080/health

# Gateway
curl http://localhost:30005/health
```

**Check resource usage:**
```bash
# Kubernetes
kubectl top pods -A

# Docker
docker stats
```

**Need more help?**
- Check logs: `kubectl logs` or `docker-compose logs`
- Review pod events: `kubectl describe pod`
- Verify configuration: `kubectl get configmap -A`
