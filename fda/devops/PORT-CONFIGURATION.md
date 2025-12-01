# Port Configuration Reference

## Overview

This document explains the port configuration strategy across Docker Compose and Kubernetes deployments. Both platforms now use **identical ports** for services to provide a consistent developer experience.

## Port Mapping Strategy

### Same Ports Everywhere
All services use the same ports across Docker Compose and Kubernetes:

| Service | Port | Access URL |
|---------|------|------------|
| Customer App | 3000 | http://localhost:3000 |
| API Gateway | 5000 | http://localhost:5000 |
| Authentication | 8081 | http://localhost:8081 |
| Catalog | 8082 | http://localhost:8082 |
| CRM | 8083 | http://localhost:8083 |
| Cart | 8084 | http://localhost:8084 |
| Order | 8085 | http://localhost:8085 |
| Payment | 8086 | http://localhost:8086 |
| MongoDB | 27017 | mongodb://localhost:27017 |

## Platform-Specific Implementation

### Docker Compose
- **Method**: Direct host port binding
- **Configuration**: `docker-compose.yml` ports section
- **Example**: `ports: ["8081:8080"]` maps container port 8080 to host port 8081
- **Automatic**: Ports exposed when containers start

### Kubernetes
- **Method**: ClusterIP services + kubectl port-forward
- **Configuration**: Services use `type: ClusterIP` (internal only)
- **Port Forwarding**: Run `.\port-forward.ps1` to expose services
- **Example**: `kubectl port-forward service/authentication-service 8081:8080`

## Why ClusterIP Instead of NodePort?

### Kubernetes NodePort Limitation
NodePorts must be in the range **30000-32767** (platform constraint). This means we **cannot** use standard ports like 3000, 5000, or 8081 directly as NodePorts.

### Solution: ClusterIP + Port Forwarding
Using ClusterIP services with port forwarding provides:

✅ **Same ports** as Docker Compose  
✅ **Consistent** developer experience  
✅ **No confusion** about different port numbers  
✅ **Production-like** configuration (ClusterIP is standard)  
✅ **Simple** one-command setup

### Alternatives Considered

| Approach | Pros | Cons | Chosen? |
|----------|------|------|---------|
| **NodePort (30000+)** | No extra tools needed | Different ports than Docker, hard to remember | ❌ No |
| **LoadBalancer** | Production-like | Requires MetalLB/cloud provider, complex setup | ❌ No |
| **Ingress** | Production-ready, hostname routing | Requires Ingress controller, DNS/hosts setup | ❌ No |
| **ClusterIP + port-forward** | Same ports, simple, production-like | Requires port-forward script | ✅ Yes |

## Port Forwarding Setup

### Automated (Recommended)

#### Start All Services
```powershell
# Forward all services to match Docker Compose ports
.\fda\devops\kubernetes\port-forward.ps1
```

The script will:
- Forward all 9 services to their Docker Compose ports
- Run in background with job monitoring
- Auto-restart failed port-forwards
- Display service URLs

#### Stop All Services
```powershell
# Stop all port-forward processes
.\fda\devops\kubernetes\port-forward.ps1 -Stop
```

### Manual Port Forwarding

For individual services or troubleshooting:

```powershell
# Frontend
kubectl port-forward -n capstone-frontend svc/customer-app-service 3000:3000

# Gateway
kubectl port-forward -n capstone-gateway svc/gateway-service 5000:5000

# Microservices
kubectl port-forward -n capstone-services svc/authentication-service 8081:8080
kubectl port-forward -n capstone-services svc/catalog-service 8082:8080
kubectl port-forward -n capstone-services svc/crm-service 8083:8080
kubectl port-forward -n capstone-services svc/cart-service 8084:8080
kubectl port-forward -n capstone-services svc/order-service 8085:8080
kubectl port-forward -n capstone-services svc/payment-service 8086:8080

# Database
kubectl port-forward -n capstone-services svc/mongodb-service 27017:27017
```

## Service Access URLs

After starting port forwarding (or Docker Compose), all services are available at:

### Frontend
- **Customer App**: http://localhost:3000

### API Gateway
- **Gateway**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

### Microservices (Direct Access)
- **Authentication**: http://localhost:8081/swagger
- **Catalog**: http://localhost:8082/swagger
- **CRM**: http://localhost:8083/swagger
- **Cart**: http://localhost:8084/swagger
- **Order**: http://localhost:8085/swagger
- **Payment**: http://localhost:8086/swagger

### Database
- **MongoDB**: mongodb://localhost:27017

**Note:** In production, you would typically access microservices through the Gateway, not directly.

## Internal Service Communication

Services communicate internally using Kubernetes DNS:

```
http://<service-name>.<namespace>.svc.cluster.local:<port>
```

Examples:
- `http://authentication-service.capstone-services.svc.cluster.local:8080`
- `http://catalog-service.capstone-services.svc.cluster.local:8080`
- `mongodb://mongodb-service.capstone-services.svc.cluster.local:27017`

## Gateway Configuration

The API Gateway is configured to route requests to backend services:

### Docker Compose
```yaml
AUTHENTICATION_URL: http://authentication:8080
CATALOG_URL: http://catalog:8080
CRM_URL: http://crm:8080
CART_URL: http://cart:8080
ORDER_URL: http://order:8080
PAYMENT_URL: http://payment:8080
```

### Kubernetes
```yaml
AUTHENTICATION_URL: http://authentication-service.capstone-services:8080
CATALOG_URL: http://catalog-service.capstone-services:8080
CRM_URL: http://crm-service.capstone-services:8080
CART_URL: http://cart-service.capstone-services:8080
ORDER_URL: http://order-service.capstone-services:8080
PAYMENT_URL: http://payment-service.capstone-services:8080
```

## Frontend Configuration

### Docker Compose
```yaml
REACT_APP_GATEWAY_URL: http://localhost:5000
```

### Kubernetes
When using port-forward:
```yaml
REACT_APP_GATEWAY_URL: http://localhost:5000
```

**Note:** Same URL because port-forward exposes Gateway on port 5000

## Troubleshooting

### Port Conflicts

If you get "port already in use" errors:

1. **Check what's using the port:**
   ```powershell
   netstat -ano | findstr :<port>
   ```

2. **Stop the conflicting service:**
   - Docker Compose: `docker-compose down`
   - Kubernetes port-forward: `.\port-forward.ps1 -Stop`
   - Other process: Use Task Manager or `Stop-Process -Id <PID>`

3. **Stop both Docker and Kubernetes:**
   ```powershell
   # Stop Docker Compose
   cd fda/devops/docker
   docker-compose -f docker-compose-working.yml down
   
   # Stop Kubernetes port-forwarding
   cd ../kubernetes
   .\port-forward.ps1 -Stop
   ```

### Port Forwarding Not Working

1. **Check if Kubernetes cluster is running:**
   ```powershell
   kubectl cluster-info
   kubectl get nodes
   ```

2. **Check if services are running:**
   ```powershell
   kubectl get svc -A
   kubectl get pods -A
   ```

3. **Check service endpoints:**
   ```powershell
   kubectl get endpoints -n capstone-services
   ```

4. **Restart port-forward script:**
   ```powershell
   .\port-forward.ps1 -Stop
   Start-Sleep -Seconds 2
   .\port-forward.ps1
   ```

### Service Not Accessible

1. **Verify port-forward is running:**
   ```powershell
   Get-Process kubectl | Where-Object { $_.CommandLine -like "*port-forward*" }
   ```

2. **Check service logs:**
   ```powershell
   kubectl logs -n capstone-services deployment/authentication-deployment
   ```

3. **Test internal connectivity:**
   ```powershell
   kubectl run test-pod --rm -it --image=curlimages/curl -- sh
   # Inside pod:
   curl http://authentication-service.capstone-services:8080/health
   ```

## Firewall Configuration

If you have firewall rules, ensure these ports are allowed:

**Docker Compose:**
- Ports 3000, 5000, 8081-8086, 27017 (localhost only)

**Kubernetes:**
- Port-forward uses localhost connections (no special firewall rules needed)
- If using NodePort, allow 30000-32767 range

## Testing Commands

### Docker Compose
```powershell
# Test all services are accessible
curl http://localhost:5000/health
curl http://localhost:8081/health
curl http://localhost:8082/health
curl http://localhost:8083/health
curl http://localhost:8084/health
curl http://localhost:8085/health
curl http://localhost:8086/health
```

### Kubernetes (with port-forward)
```powershell
# Same tests - same ports!
curl http://localhost:5000/health
curl http://localhost:8081/health
curl http://localhost:8082/health
curl http://localhost:8083/health
curl http://localhost:8084/health
curl http://localhost:8085/health
curl http://localhost:8086/health
```

## Best Practices

1. **Don't run both simultaneously**: Stop Docker Compose before starting Kubernetes port-forward (or vice versa) to avoid port conflicts

2. **Use the port-forward script**: Don't manually forward ports unless troubleshooting

3. **Keep ports consistent**: If adding new services, use the same port number in both Docker Compose and Kubernetes

4. **Document port changes**: Update this file and DEPLOYMENT-REFERENCE.md when adding services

5. **Test both platforms**: Ensure services work identically on both Docker Compose and Kubernetes

## Summary

- ✅ **Same ports** across Docker Compose and Kubernetes
- ✅ **Simple setup**: One script starts all port-forwards
- ✅ **Production-like**: ClusterIP is the standard Kubernetes service type
- ✅ **Developer-friendly**: No mental mapping between different port numbers
- ✅ **Flexible**: Can easily switch between Docker and Kubernetes
