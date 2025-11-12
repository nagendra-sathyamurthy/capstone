# Dual Deployment Strategy - Local vs Production

This document outlines the dual deployment architecture for the Capstone microservices application, providing optimized configurations for both local development and production environments.

## 🏗️ Architecture Overview

### Local Development Environment
- **Purpose:** Resource-efficient development and testing
- **MongoDB Strategy:** Single shared MongoDB instance
- **Resource Allocation:** Minimal (optimized for laptops/workstations)
- **Service Discovery:** NodePort services
- **Storage:** EmptyDir (temporary)

### Production Environment  
- **Purpose:** High availability, scalability, and data isolation
- **MongoDB Strategy:** Dedicated MongoDB per service
- **Resource Allocation:** High availability with health checks
- **Service Discovery:** LoadBalancer services
- **Storage:** Persistent volumes with backup capability

## 📁 Directory Structure

```
devops/kubernetes/
├── local/                          # Local development deployments
│   ├── namespace.yaml              # Shared namespace
│   ├── mongodb-secret.yaml         # Shared MongoDB credentials
│   ├── mongodb-config.yaml         # MongoDB configuration
│   ├── mongodb.yaml               # Single MongoDB instance
│   ├── authentication.yaml        # Auth service (shared DB)
│   ├── catalog.yaml               # Catalog service (shared DB)
│   ├── crm.yaml                   # CRM service (shared DB)
│   └── cart.yaml                  # Cart service (shared DB)
├── production/                     # Production deployments
│   ├── namespace.yaml              # Production namespace
│   ├── mongodb-secrets.yaml       # Dedicated DB secrets per service
│   ├── authentication.yaml        # Auth + dedicated MongoDB
│   ├── catalog.yaml               # Catalog + dedicated MongoDB
│   ├── crm.yaml                   # CRM + dedicated MongoDB
│   └── cart.yaml                  # Cart + dedicated MongoDB
├── deploy-local.ps1               # Local deployment script
├── deploy-production.ps1          # Production deployment script
└── cleanup-environment.ps1       # Environment cleanup script
```

## 🚀 Quick Start

### Deploy Local Development Environment

```powershell
# Navigate to Kubernetes directory
cd c:\dotnet\capstone\fda\devops\kubernetes

# Deploy local environment
.\deploy-local.ps1

# Optional: Clean deployment first
.\deploy-local.ps1 -Clean
```

### Deploy Production Environment

```powershell
# Deploy production environment
.\deploy-production.ps1

# Clean deployment first (WARNING: Deletes data)
.\deploy-production.ps1 -Clean
```

### Cleanup Environments

```powershell
# Clean local only
.\cleanup-environment.ps1 -Environment local

# Clean production (requires confirmation)
.\cleanup-environment.ps1 -Environment production

# Clean everything (requires confirmation)
.\cleanup-environment.ps1 -Environment all
```

## 🏠 Local Development Details

### Architecture Benefits
- **Resource Efficiency:** Single MongoDB serves all services
- **Fast Startup:** Minimal resource requirements
- **Easy Debugging:** All data in one database instance
- **Cost Effective:** Suitable for development machines

### MongoDB Configuration
- **Instance:** `mongodb-deployment` (single pod)
- **Service:** `mongodb-service:27017`
- **Databases:** 
  - `authenticationdb` (users collection)
  - `catalogdb` (items collection)  
  - `crmdb` (customers collection)
  - `cartdb` (carts collection)

### Service Endpoints
- **Authentication:** `http://localhost:30001`
- **Catalog:** `http://localhost:30002`
- **CRM:** `http://localhost:30003`  
- **Cart:** `http://localhost:30004`
- **MongoDB:** `mongodb://localhost:30000`

### Resource Allocation (Per Service)
- **Memory:** 128Mi request, 256Mi limit
- **CPU:** 100m request, 250m limit
- **Replicas:** 1 per service

## 🏢 Production Environment Details

### Architecture Benefits
- **Data Isolation:** Each service has dedicated MongoDB
- **Independent Scaling:** Services scale independently
- **Security:** Isolated credentials per service
- **High Availability:** Multiple replicas with health checks
- **Persistent Storage:** Data survives pod restarts

### MongoDB Instances
1. **Authentication MongoDB**
   - Service: `auth-mongodb-service:27017`
   - Database: `authenticationdb`
   - Storage: 10Gi PVC

2. **Catalog MongoDB** 
   - Service: `catalog-mongodb-service:27017`
   - Database: `catalogdb`
   - Storage: 20Gi PVC

3. **CRM MongoDB**
   - Service: `crm-mongodb-service:27017` 
   - Database: `crmdb`
   - Storage: 50Gi PVC (largest - customer data)

4. **Cart MongoDB**
   - Service: `cart-mongodb-service:27017`
   - Database: `cartdb` 
   - Storage: 15Gi PVC

### Service Configuration
- **Replicas:** 2-3 per service (high availability)
- **Service Type:** LoadBalancer (external access)
- **Health Checks:** Liveness and readiness probes
- **Resource Limits:** Higher allocation for production load

## 🔐 Security Configuration

### Local Development
- **Credentials:** Athena / Ath3n@Str0ngP@ssw0rd2024!
- **Secret Management:** Single `mongodb-secret` for all services
- **Connection Strings:** Point to shared `mongodb-service`

### Production
- **Credentials:** Athena / Ath3n@Str0ngP@ssw0rd2024! (per service)
- **Secret Management:** Dedicated secrets per MongoDB instance
- **Connection Strings:** Point to service-specific MongoDB instances
- **Network Isolation:** ClusterIP for MongoDB (internal only)

## 🔄 Migration Between Environments

### From Local to Production
1. Export data from shared MongoDB:
   ```bash
   kubectl exec -it mongodb-deployment-xxx -n capstone-services -- mongodump --authenticationDatabase admin -u Athena -p 'Ath3n@Str0ngP@ssw0rd2024!' --out /tmp/backup
   ```

2. Deploy production environment:
   ```powershell
   .\deploy-production.ps1
   ```

3. Import data to respective MongoDB instances:
   ```bash
   # Import to each dedicated MongoDB instance
   kubectl exec -it auth-mongodb-deployment-xxx -n capstone-services -- mongorestore --authenticationDatabase admin -u Athena -p 'Ath3n@Str0ngP@ssw0rd2024!' --db authenticationdb /tmp/backup/authenticationdb
   ```

### From Production to Local
1. Export data from each MongoDB instance
2. Deploy local environment
3. Import all data to shared MongoDB instance

## 📊 Monitoring and Troubleshooting

### Health Checks

```powershell
# Check all pods
kubectl get pods -n capstone-services

# Check services
kubectl get services -n capstone-services

# Check persistent volumes (production only)
kubectl get pvc -n capstone-services

# Check pod logs
kubectl logs deployment/mongodb-deployment -n capstone-services
kubectl logs deployment/authentication-deployment -n capstone-services
```

### Common Issues

1. **MongoDB Connection Issues**
   ```powershell
   # Check secret configuration
   kubectl get secret mongodb-secret -n capstone-services -o yaml
   
   # Verify service connectivity
   kubectl exec -it authentication-deployment-xxx -n capstone-services -- nslookup mongodb-service
   ```

2. **Resource Constraints (Local)**
   ```powershell
   # Scale down replicas if needed
   kubectl scale deployment authentication-deployment --replicas=1 -n capstone-services
   ```

3. **Storage Issues (Production)**
   ```powershell
   # Check PVC status
   kubectl describe pvc auth-mongodb-pvc -n capstone-services
   ```

## 🎯 Best Practices

### Local Development
- Use `deploy-local.ps1` for consistent setup
- Monitor resource usage: `kubectl top pods -n capstone-services`
- Use port forwarding for direct database access when debugging

### Production
- Always backup data before deployment changes
- Monitor PVC usage and plan for scaling
- Use `kubectl wait` commands to ensure proper startup sequence
- Set up monitoring and alerting for production workloads

### Security
- Rotate passwords regularly in production
- Use Kubernetes secrets for all sensitive data
- Never commit secrets to version control
- Consider using external secret management in production

## 🔗 Integration with Development Workflow

### Postman Collections
Both environments work with the existing Postman collections:
- **Local:** Use `localhost:3000X` endpoints
- **Production:** Update base URLs to LoadBalancer IPs

### .NET User Secrets
Local development also supports .NET User Secrets for direct service testing:
```powershell
# Set up user secrets (already configured)
.\devops\jobs\setup-user-secrets.ps1
```

---

This dual deployment strategy provides the flexibility to develop efficiently locally while ensuring production-ready deployments with proper isolation and scalability.