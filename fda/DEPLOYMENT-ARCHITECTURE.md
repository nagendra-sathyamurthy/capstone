# Deployment Architecture Summary

## Overview
The Capstone Food Delivery Application now has complete deployment configurations for both Docker Compose and Kubernetes environments, supporting local development, testing, and production deployments.

## Deployment Options

### 1. Docker Compose Deployment

#### Full Stack (`docker-compose.yml`)
- **Services**: MongoDB, Authentication, Catalog, CRM, Cart, Order, Payment, Gateway, Customer App
- **Network**: Custom bridge network `capstone-network`
- **Ports**: 
  - Customer App: 3000
  - Gateway: 5000
  - Backend Services: 8081-8086
  - MongoDB: 27017
- **Usage**: `docker-compose up -d`

#### Development Mode (`docker-compose.dev.yml`)
- **Services**: MongoDB, Gateway only
- **Purpose**: Run core infrastructure while developing services locally
- **Usage**: `docker-compose -f docker-compose.dev.yml up -d`

### 2. Kubernetes Deployment

#### Local Environment
**Namespaces:**
- `capstone-services` - Backend services and MongoDB
- `capstone-gateway` - API Gateway
- `capstone-frontend` - Customer App

**Services:**
- Customer App: http://localhost:30080 (NodePort)
- Gateway: http://localhost:30005 (NodePort)
- Authentication: http://localhost:30001
- Catalog: http://localhost:30002
- CRM: http://localhost:30003
- Cart: http://localhost:30004
- MongoDB: http://localhost:30000

**Deployment Scripts:**
- `deploy-gateway-local.ps1` - Deploy gateway only
- `deploy-customer-app-local.ps1` - Deploy customer app only
- `deploy-full-stack-local.ps1` - Deploy complete stack

#### Production Environment
**Differences from Local:**
- LoadBalancer service type (instead of NodePort)
- Horizontal Pod Autoscaler (HPA) configured
- Higher replica counts (3-10 based on load)
- Increased resource limits
- Image pull from container registry
- Rolling update strategy
- Production environment variables

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Internet/Users                       │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │   Customer App (React) │
        │   Port: 3000/30080     │
        │   Namespace: frontend  │
        └────────────┬───────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │   API Gateway (Node)   │
        │   Port: 5000/30005     │
        │   Namespace: gateway   │
        └────────────┬───────────┘
                     │
        ┌────────────┴───────────────────────────┐
        │                                        │
        ▼                                        ▼
┌──────────────┐                        ┌──────────────┐
│   Backend    │                        │   Backend    │
│   Services   │◄──────────────────────►│   Services   │
│ (.NET Core)  │                        │ (.NET Core)  │
└──────┬───────┘                        └──────┬───────┘
       │                                       │
       │          ┌──────────────┐             │
       └─────────►│   MongoDB    │◄────────────┘
                  │  Port: 27017 │
                  └──────────────┘
```

## Container Images

### Customer App Image
- **Base**: node:18-alpine (build) + nginx:alpine (serve)
- **Build**: Multi-stage for optimized size
- **Features**: 
  - React production build
  - Nginx with gzip compression
  - Security headers
  - Health check endpoint
  - React Router support

### Gateway Image
- **Base**: node:18-alpine
- **Features**:
  - Express.js API gateway
  - Rate limiting
  - CORS configuration
  - Request logging
  - Health check endpoint

### Backend Services
- **Base**: ASP.NET Core SDK (build) + Runtime (serve)
- **Features**:
  - Health checks
  - MongoDB connectivity
  - RESTful APIs
  - JWT authentication

## Environment Configuration

### Local Development
```yaml
Gateway URL: http://localhost:5000
Customer App: http://localhost:30080
MongoDB: mongodb://admin:AdminPass2024@localhost:27017
Environment: development
Log Level: debug
CORS: http://localhost:3000
```

### Production
```yaml
Gateway URL: https://api.your-domain.com
Customer App: https://app.your-domain.com
MongoDB: mongodb://[secure-credentials]@[cluster-url]
Environment: production
Log Level: info
CORS: https://app.your-domain.com
```

## Resource Requirements

### Local Development (Minimum)
- **CPU**: 4 cores
- **RAM**: 8GB
- **Disk**: 20GB free space
- **Docker**: Desktop 4.x+
- **Kubernetes**: Minikube/Docker Desktop K8s

### Production (Recommended)
- **CPU**: 8+ cores
- **RAM**: 16GB+
- **Disk**: 100GB+ SSD
- **Kubernetes**: Managed cluster (AKS, EKS, GKE)
- **Load Balancer**: Cloud provider LB

## Scaling Configuration

### Kubernetes HPA (Production)
```yaml
Customer App:
  Min Replicas: 3
  Max Replicas: 10
  CPU Threshold: 70%
  Memory Threshold: 80%

Gateway:
  Min Replicas: 3
  Max Replicas: 10
  CPU Threshold: 70%
  Memory Threshold: 80%

Backend Services:
  Min Replicas: 2
  Max Replicas: 5
  CPU Threshold: 75%
```

## Deployment Workflows

### Local Development Workflow
1. Start Docker Desktop
2. Enable Kubernetes
3. Run `./devops/kubernetes/deploy-full-stack-local.ps1`
4. Access Customer App at http://localhost:30080

### Docker Compose Workflow
1. Navigate to `devops/docker`
2. Run `docker-compose up -d`
3. Access Customer App at http://localhost:3000
4. View logs: `docker-compose logs -f`

### Production Deployment Workflow
1. Build and push images to registry
2. Update image tags in production YAML files
3. Apply Kubernetes configurations
4. Verify deployment: `kubectl get pods -A`
5. Monitor logs and metrics

## Health Checks

All services include health check endpoints:
- **Customer App**: `GET /health` (200 OK)
- **Gateway**: `GET /health` (200 OK)
- **Backend Services**: `GET /health` (200 OK)

Kubernetes uses these for:
- Liveness probes (restart if failing)
- Readiness probes (remove from load balancer if not ready)

## Networking

### Kubernetes Service Discovery
Services communicate using Kubernetes DNS:
```
http://[service-name].[namespace].svc.cluster.local:[port]
Example: http://crm-service.capstone-services.svc.cluster.local:8080
```

### Docker Compose Networking
Services communicate using service names:
```
http://[service-name]:[port]
Example: http://crm:8080
```

## Security Considerations

### Implemented
✅ Network isolation (namespaces/networks)
✅ Resource limits and requests
✅ Health checks
✅ Non-root containers
✅ Security headers in Nginx
✅ CORS configuration
✅ Rate limiting

### TODO for Production
⚠️ Update MongoDB credentials
⚠️ Enable TLS/SSL
⚠️ Implement secrets management (Vault, Sealed Secrets)
⚠️ Configure network policies
⚠️ Enable pod security policies
⚠️ Set up monitoring and alerting
⚠️ Configure log aggregation
⚠️ Implement backup strategy

## Monitoring & Observability

### Recommended Tools
- **Metrics**: Prometheus + Grafana
- **Logs**: ELK Stack or Loki
- **Tracing**: Jaeger or Zipkin
- **APM**: New Relic, DataDog, or Application Insights

### Key Metrics to Monitor
- Request latency (p50, p95, p99)
- Error rate
- Request throughput
- Pod CPU/Memory usage
- MongoDB connection pool
- Disk usage
- Network I/O

## Troubleshooting

### Common Issues

**Pods not starting:**
```bash
kubectl describe pod [pod-name] -n [namespace]
kubectl logs [pod-name] -n [namespace]
```

**Service connectivity issues:**
```bash
kubectl exec -it [pod-name] -n [namespace] -- curl http://[service]:[port]/health
```

**Image pull errors:**
```bash
# For local: Ensure imagePullPolicy: Never
# For production: Verify registry credentials
```

**Port conflicts:**
```bash
# Check what's using the port
netstat -ano | findstr :[port]
```

## Backup & Recovery

### MongoDB Backup
```bash
# Docker Compose
docker-compose exec mongodb mongodump --out /backup --username admin --password AdminPass2024

# Kubernetes
kubectl exec -n capstone-services mongodb-deployment-[pod-id] -- mongodump --out /backup
```

### Disaster Recovery
1. Regular MongoDB backups (daily recommended)
2. Store backups in cloud storage (S3, Azure Blob)
3. Test restore procedure regularly
4. Document recovery steps
5. Keep infrastructure as code in version control

## Continuous Deployment

### Recommended CI/CD Pipeline
1. Code commit → GitHub
2. Build images → Docker/Container Registry
3. Run tests → Unit + Integration
4. Push images → Registry
5. Update K8s manifests → GitOps (ArgoCD/Flux)
6. Deploy to staging → Automated
7. Run smoke tests → Automated
8. Deploy to production → Manual approval

## Next Steps

1. ✅ Deploy to local Kubernetes for testing
2. ⚠️ Update MongoDB credentials
3. ⚠️ Set up container registry
4. ⚠️ Configure production domain and SSL
5. ⚠️ Implement monitoring
6. ⚠️ Set up CI/CD pipeline
7. ⚠️ Document runbooks
8. ⚠️ Perform load testing

## Quick Reference

### Deploy Everything (Local)
```powershell
.\devops\kubernetes\deploy-full-stack-local.ps1
```

### Deploy with Docker Compose
```bash
cd devops/docker
docker-compose up -d
```

### View All Pods
```bash
kubectl get pods -A
```

### Access Customer App
- Kubernetes: http://localhost:30080
- Docker Compose: http://localhost:3000

### View Logs
```bash
# Kubernetes
kubectl logs -n capstone-frontend -l app=customer-app --tail=100 -f

# Docker Compose
docker-compose logs -f customer-app
```
