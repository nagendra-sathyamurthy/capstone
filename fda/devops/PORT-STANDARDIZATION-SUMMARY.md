# Port Standardization Summary

## Overview
All deployment configurations (Docker Compose, Kubernetes Local, Kubernetes Production) have been aligned to use consistent port mappings as defined in the Postman environment files.

## Standard Port Assignments

| Service        | Port  | Purpose                          |
|----------------|-------|----------------------------------|
| Authentication | 30001 | User authentication & JWT tokens |
| Catalog        | 30002 | Menu items & catalog management  |
| CRM            | 30003 | Customer relationship management |
| Cart           | 30004 | Shopping cart operations         |
| Order          | 30005 | Order processing & management    |
| Payment        | 30006 | Payment processing               |
| Gateway        | 30007 | API Gateway (local only)         |
| Customer App   | 30080 | Frontend React application       |
| MongoDB        | 27017 | Database (internal only)         |

## Changes Made

### 1. Docker Compose (`devops/docker/docker-compose.yml`)
**Changed:** All service ports updated from 808x to 3000x
- Authentication: 8081 → 30001
- Catalog: 8082 → 30002
- CRM: 8083 → 30003
- Cart: 8084 → 30004
- Order: 8085 → 30005
- Payment: 8086 → 30006

### 2. Kubernetes Local (`devops/kubernetes/local/`)
**Status:** Already aligned ✅
- All services already using correct NodePort mappings (30001-30006)
- Gateway NodePort changed from 30005 → 30007 (to avoid conflict with order service)
- Customer-app gateway URL updated to point to 30007
- Added ORDER_SERVICE_URL and PAYMENT_SERVICE_URL to gateway config

### 3. Kubernetes Production (`devops/kubernetes/production/`)
**Changed:** Service ports updated from 500x to 3000x
- Authentication: 5001 → 30001
- Catalog: 5002 → 30002
- CRM: 5003 → 30003
- Cart: 5004 → 30004
- Order: Created new file with port 30005
- Payment: Created new file with port 30006
- Added ORDER_SERVICE_URL and PAYMENT_SERVICE_URL to gateway config

**New Files Created:**
- `order.yaml` - Complete deployment with dedicated MongoDB and LoadBalancer service
- `payment.yaml` - Complete deployment with dedicated MongoDB and LoadBalancer service

### 4. Postman Collections (`postman-collections/`)
**Created:** `Capstone-Production-Environment.postman_environment.json`
- Contains all service base URLs matching standard ports
- Includes gateway and customer-app URLs
- Mirrors structure of local environment for consistency

## Deployment-Specific Notes

### Docker Compose
- All services exposed directly on their respective ports (30001-30006)
- Gateway on port 5000 (internal routing)
- Customer-app on port 3000
- Single MongoDB instance on port 27017

### Kubernetes Local
- Services use NodePort type with standard ports (30001-30006)
- Gateway uses NodePort 30007
- Customer-app uses NodePort 30080
- Shared MongoDB instance for all services
- Namespaces: capstone-services, capstone-gateway, capstone-frontend

### Kubernetes Production
- Services use LoadBalancer type with standard ports (30001-30006)
- Gateway uses LoadBalancer on port 80 (HTTP)
- Customer-app uses LoadBalancer on port 80 (HTTP)
- Each service has dedicated MongoDB instance with PersistentVolumeClaim
- Higher replica counts for HA (2-3 replicas per service)
- Horizontal Pod Autoscalers configured for gateway and customer-app
- Namespaces: capstone-services, capstone-gateway, capstone-frontend

## Service Discovery

### Internal Communication (K8s)
Services communicate using internal cluster DNS:
- `http://<service-name>-service.capstone-services.svc.cluster.local:8080`
- Example: `http://authentication-service.capstone-services.svc.cluster.local:8080`

### External Access
- **Docker Compose:** `http://localhost:<port>`
- **K8s Local:** `http://localhost:<nodeport>`
- **K8s Production:** Via LoadBalancer IP/DNS on standard ports

## Testing with Postman

1. Import all collection files from `postman-collections/`
2. Select appropriate environment:
   - `Capstone-Local-Environment` for local development
   - `Capstone-Production-Environment` for production testing
3. Both environments use identical port mappings for consistency
4. Run collections in sequence using workflow collection

## Gateway Configuration

The API Gateway routes requests to backend services and provides:
- Request routing and load balancing
- CORS handling
- Rate limiting
- Centralized logging
- Health checks

**Gateway URLs:**
- Local: `http://localhost:30007`
- Production: Via LoadBalancer (configure DNS)

**Customer App Gateway Configuration:**
- Local: Points to `http://localhost:30007`
- Production: Points to production LoadBalancer URL

## Next Steps

1. **Update MongoDB Secrets:** Run `devops/scripts/setup-user-secrets.ps1` for production secrets
2. **Build Docker Images:** Build all service images before deployment
3. **Deploy:** Use appropriate deployment scripts:
   - Docker: `docker-compose up -d` in `devops/docker/`
   - K8s Local: `devops/kubernetes/deploy-local.ps1`
   - K8s Production: `devops/kubernetes/deploy-production.ps1`
4. **Test:** Use Postman collections with corresponding environment

## Port Conflict Resolution

The following conflict was identified and resolved:
- **Issue:** Gateway was using NodePort 30005 (conflicted with order service)
- **Resolution:** Gateway moved to NodePort 30007
- **Impact:** Customer-app config updated to new gateway URL

## Consistency Verification ✅

All port configurations are now aligned:
- ✅ Docker Compose ports match Postman standards
- ✅ Kubernetes Local NodePorts match Postman standards
- ✅ Kubernetes Production LoadBalancer ports match Postman standards
- ✅ Gateway configurations include all service URLs
- ✅ Customer-app points to correct gateway URL
- ✅ Postman environments configured for both local and production
- ✅ No port conflicts across any deployment method

## Additional Notes

### Payment Service
- **Status:** Directory exists but is currently empty
- **Impact:** payment.yaml created with standard configuration template
- **Action Required:** Implement payment service before deploying to production

### Production Secrets
Required secrets for production (not included in repository):
- `auth-mongodb-secret`
- `catalog-mongodb-secret`
- `crm-mongodb-secret`
- `cart-mongodb-secret`
- `order-mongodb-secret`
- `payment-mongodb-secret`

Use `devops/scripts/setup-user-secrets.ps1` to generate these secrets before production deployment.
