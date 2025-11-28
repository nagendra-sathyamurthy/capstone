# Port Configuration Reference

This document outlines the standardized port configuration across Docker Compose and Kubernetes deployments.

## Port Mapping Strategy

To maintain consistency between Docker Compose and Kubernetes (NodePort) deployments, we use the following port mapping strategy:

- **Docker Compose**: Uses standard ports directly (e.g., 5000, 8081, 8082)
- **Kubernetes NodePort**: Uses 30XXX range that maps to the Docker Compose ports
  - Formula: `NodePort = 30000 + (Last 3 digits of Docker port)`
  - Example: Docker port 8081 → NodePort 30081

## Standardized Port Configuration

| Service | Container Port | Docker Compose Host Port | Kubernetes NodePort | Access URL (Docker) | Access URL (Kubernetes) |
|---------|---------------|-------------------------|---------------------|--------------------|-----------------------|
| **MongoDB** | 27017 | 27017 | 32017 | mongodb://localhost:27017 | mongodb://localhost:32017 |
| **Authentication** | 8080 | 8081 | 30081 | http://localhost:8081 | http://localhost:30081 |
| **Catalog** | 8080 | 8082 | 30082 | http://localhost:8082 | http://localhost:30082 |
| **CRM** | 8080 | 8083 | 30083 | http://localhost:8083 | http://localhost:30083 |
| **Cart** | 8080 | 8084 | 30084 | http://localhost:8084 | http://localhost:30084 |
| **Order** | 8080 | 8085 | 30085 | http://localhost:8085 | http://localhost:30085 |
| **Payment** | 8080 | 8086 | 30086 | http://localhost:8086 | http://localhost:30086 |
| **Gateway** | 5000 | 5000 | 30500 | http://localhost:5000 | http://localhost:30500 |
| **Customer App** | 3000 | 3000 | 30300 | http://localhost:3000 | http://localhost:30300 |

## Internal Service Communication

### Docker Compose (Container-to-Container)
Services communicate using container names on port 8080 (for .NET services) or 5000 (for Gateway):
```
http://authentication:8080
http://catalog:8080
http://crm:8080
http://cart:8080
http://order:8080
http://payment:8080
http://gateway:5000
```

### Kubernetes (Pod-to-Pod)
Services communicate using Kubernetes DNS on port 8080 (for .NET services) or 5000 (for Gateway):
```
http://authentication-service.capstone-services.svc.cluster.local:8080
http://catalog-service.capstone-services.svc.cluster.local:8080
http://crm-service.capstone-services.svc.cluster.local:8080
http://cart-service.capstone-services.svc.cluster.local:8080
http://order-service.capstone-services.svc.cluster.local:8080
http://payment-service.capstone-services.svc.cluster.local:8080
http://gateway-service.capstone-gateway.svc.cluster.local:5000
```

## Gateway Configuration

The Gateway routes to backend services using internal URLs:

### Docker Compose
```bash
AUTH_SERVICE_URL=http://authentication:8080
CATALOG_SERVICE_URL=http://catalog:8080
CRM_SERVICE_URL=http://crm:8080
CART_SERVICE_URL=http://cart:8080
ORDER_SERVICE_URL=http://order:8080
PAYMENT_SERVICE_URL=http://payment:8080
```

### Kubernetes
```bash
AUTH_SERVICE_URL=http://authentication-service.capstone-services.svc.cluster.local:8080
CATALOG_SERVICE_URL=http://catalog-service.capstone-services.svc.cluster.local:8080
CRM_SERVICE_URL=http://crm-service.capstone-services.svc.cluster.local:8080
CART_SERVICE_URL=http://cart-service.capstone-services.svc.cluster.local:8080
ORDER_SERVICE_URL=http://order-service.capstone-services.svc.cluster.local:8080
PAYMENT_SERVICE_URL=http://payment-service.capstone-services.svc.cluster.local:8080
```

## Frontend Configuration

### Docker Compose
Customer app connects to gateway at:
```bash
REACT_APP_GATEWAY_URL=http://localhost:5000
```

### Kubernetes
Customer app connects to gateway NodePort at:
```bash
REACT_APP_GATEWAY_URL=http://localhost:30500
```

## Port Conflict Resolution

If you encounter port conflicts:

### Docker Compose
Edit `docker-compose-working.yml` and update the host port (left side of mapping):
```yaml
ports:
  - "8081:8080"  # Change 8081 to available port
```

### Kubernetes
Edit the respective `*.yaml` file in `fda/devops/kubernetes/local/` and update the nodePort:
```yaml
ports:
  - port: 8080
    targetPort: 8080
    nodePort: 30081  # Change to available port (30000-32767 range)
```

## Firewall Configuration

Ensure these ports are open in your firewall:

### For Docker Compose:
- 3000 (Customer App)
- 5000 (Gateway)
- 8081-8086 (Backend Services)
- 27017 (MongoDB)

### For Kubernetes:
- 30300 (Customer App)
- 30500 (Gateway)
- 30081-30086 (Backend Services)
- 32017 (MongoDB)

## Testing Port Accessibility

### Docker Compose
```powershell
# Test Gateway
curl http://localhost:5000/health

# Test Backend Services
curl http://localhost:8081/swagger  # Authentication
curl http://localhost:8082/swagger  # Catalog
curl http://localhost:8083/swagger  # CRM

# Test Customer App
curl http://localhost:3000
```

### Kubernetes
```powershell
# Test Gateway
curl http://localhost:30500/health

# Test Backend Services
curl http://localhost:30081/swagger  # Authentication
curl http://localhost:30082/swagger  # Catalog
curl http://localhost:30083/swagger  # CRM

# Test Customer App
curl http://localhost:30300
```

## Notes

1. **Container Ports** (8080, 5000, 3000) remain consistent across all deployments
2. **Host Ports** vary between Docker Compose (direct mapping) and Kubernetes (NodePort range)
3. **Internal Communication** always uses container ports, not host ports
4. **MongoDB Port** uses special mapping: 27017 (Docker) → 32017 (Kubernetes) to avoid conflicts

## Updating Ports

When changing ports:

1. Update `docker-compose-working.yml` for Docker Compose deployment
2. Update `fda/devops/kubernetes/local/*.yaml` for Kubernetes deployment
3. Update this documentation
4. Update deployment scripts if they reference specific ports
5. Update environment variables in `.env.example`
6. Notify team members of port changes
