# MongoDB Secrets Management

This document explains the secure MongoDB credentials management implementation for the Capstone microservices platform.

## Overview

MongoDB credentials and connection strings have been moved from configuration files to secure secrets management:

- **Kubernetes Deployment**: Uses Kubernetes Secrets with shared MongoDB instance
- **Local Development**: Uses .NET User Secrets with NodePort access
- **Configuration Files**: Contain placeholder values only

## Current Architecture

The Capstone platform now uses a **streamlined Kubernetes-first approach**:

- **Local Development**: Single shared MongoDB accessible via NodePort (localhost:30000)
- **Production**: Dedicated MongoDB instances per service for isolation
- **Services**: Direct access via NodePorts (30001-30004) - no port forwarding needed
- **Secrets**: Centralized management through Kubernetes secrets

## Security Benefits

✅ **No Hardcoded Credentials**: Credentials are not stored in source code  
✅ **Environment Isolation**: Different credentials for different environments  
✅ **Kubernetes Integration**: Native secrets management in cluster  
✅ **Developer Friendly**: Simple setup for local development  
✅ **Version Control Safe**: No sensitive data in git repository

## Architecture

### Kubernetes Secrets Structure

```yaml
# mongodb-secret.yaml contains:
- mongodb-secret: Base MongoDB credentials
- authentication-secret: Authentication service specific
- catalog-secret: Catalog service specific  
- crm-secret: CRM service specific
- cart-secret: Cart service specific
```

### Configuration Hierarchy

1. **appsettings.json**: Placeholder values (no real credentials)
2. **appsettings.Development.json**: Local development overrides
3. **User Secrets**: Local development credentials (not in git)
4. **Kubernetes Secrets**: Production/deployment credentials
5. **Environment Variables**: Runtime injection from secrets

## Setup Instructions

### For Developers (Local Development)

1. **Automatic Setup** (Recommended):
   ```powershell
   cd .\devops\jobs
   .\setup-user-secrets.ps1
   ```

2. **Manual Setup**:
   ```powershell
   # For each service (authentication, catalog, crm, cart)
   cd .\src\services\[service]\Services
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "mongodb://[username]:[password]@localhost:[port]/[database]?authSource=admin"
   dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://[username]:[password]@localhost:[port]/[database]?authSource=admin"
   ```

### For DevOps (Kubernetes Deployment)

1. **Apply Secrets**:
   ```powershell
   cd .\devops\jobs
   .\apply-secrets.ps1
   ```

2. **Manual Application**:
   ```bash
   kubectl apply -f ../kubernetes/local/mongodb-secret.yaml
   ```

3. **Restart Deployments** (if already running):
   ```bash
   kubectl rollout restart deployment -n capstone-services
   ```

## Configuration Structure

### Project Files (.csproj)
Each service now includes:
```xml
<PropertyGroup>
  <UserSecretsId>[service-name]-service-secrets</UserSecretsId>
</PropertyGroup>
```

### appsettings.json (Placeholders Only)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PlaceholderConnectionString",
    "KubernetesConnection": "PlaceholderKubernetesConnection"
  },
  "DatabaseSettings": {
    "ConnectionString": "PlaceholderConnectionString",
    "DatabaseName": "[database]",
    "CollectionName": "[collection]"
  }
}
```

### appsettings.Development.json (Local Override)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "mongodb://[username]:[password]@localhost:[port]/[database]?authSource=admin"
  },
  "DatabaseSettings": {
    "ConnectionString": "mongodb://[username]:[password]@localhost:[port]/[database]?authSource=admin"
  }
}
```

## Environment Variables (Kubernetes)

Services receive credentials through environment variables:

```yaml
env:
- name: ConnectionStrings__DefaultConnection
  valueFrom:
    secretKeyRef:
      name: [service]-secret
      key: connection-string
- name: DatabaseSettings__ConnectionString
  valueFrom:
    secretKeyRef:
      name: [service]-secret  
      key: connection-string
```

## Database Configuration

### Services and Databases

| Service | Database | Collection | Secret Name |
|---------|----------|------------|-------------|
| Authentication | `authenticationdb` | `users` | `authentication-secret` |
| Catalog | `catalogdb` | `items` | `catalog-secret` |
| CRM | `crmdb` | `customers` | `crm-secret` |
| Cart | `cartdb` | `carts` | `cart-secret` |

### Connection String Format

- **Local Development**: `mongodb://[username]:[password]@localhost:[nodeport]/[database]?authSource=admin`
- **Kubernetes**: `mongodb://[username]:[password]@mongodb-service:[port]/[database]?authSource=admin`

## Security Best Practices

### What's Secure ✅
- Credentials stored in Kubernetes secrets (base64 encoded)
- User secrets stored in OS user profile (encrypted)
- No credentials in source code or configuration files
- Environment-specific credential isolation
- Documentation uses placeholder values only (no real credentials)

### ⚠️ CRITICAL SECURITY WARNING
**NEVER commit real credentials or specific ports to version control or documentation files!**
- This documentation uses `[username]`, `[password]`, and `[port]` placeholders only
- Actual credentials are stored securely in Kubernetes secrets and .NET User Secrets
- Real connection strings and port mappings are configured via automation scripts only
- Specific port numbers should not be exposed in documentation or public repositories
- Use deployment scripts to discover actual service endpoints

### Additional Recommendations 🔒
1. **Rotate Credentials Regularly**: Update MongoDB passwords periodically
2. **Strong Authentication**: Use strong, unique credentials for each environment
3. **Network Policies**: Implement Kubernetes network policies for production
4. **RBAC**: Configure appropriate Kubernetes role-based access control
5. **Monitoring**: Monitor secret access and usage patterns
6. **Port Security**: Use non-standard ports and firewall rules to limit exposure
7. **TLS Encryption**: Enable MongoDB TLS for production deployments
8. **Documentation Security**: Never expose real ports, IPs, or credentials in documentation
9. **Access Discovery**: Use automation scripts to discover service endpoints securely

## Troubleshooting

### Common Issues

1. **Service Can't Connect to MongoDB**
   ```powershell
   # Check if secrets exist
   kubectl get secrets -n capstone-services
   
   # Verify secret content (keys only)
   kubectl get secret [service]-secret -n capstone-services -o yaml
   
   # Check environment variables in pod
   kubectl exec -it [pod-name] -n capstone-services -- env | grep -i mongo
   ```

2. **Local Development Connection Issues**
   ```powershell
   # Verify user secrets
   cd .\src\services\[service]\Services
   dotnet user-secrets list
   
   # Check MongoDB is running in Kubernetes
   kubectl get pods -n capstone-services | findstr mongodb
   kubectl get svc -n capstone-services | findstr mongodb
   ```

3. **Configuration Not Loading**
   ```powershell
   # Verify UserSecretsId in .csproj
   # Check appsettings hierarchy
   # Restart application/pod
   ```

### Verification Commands

```powershell
# List all secrets in namespace
kubectl get secrets -n capstone-services

# Check if secret exists (DO NOT decode credentials in logs)
kubectl get secret mongodb-secret -n capstone-services -o jsonpath='{.metadata.name}'

# Check pod environment variables (credentials will be masked)
kubectl describe pod [pod-name] -n capstone-services

# View user secrets for a service (shows keys, not values)
cd [service-path]
dotnet user-secrets list
```

## Migration Notes

### Recent Changes (November 2025)

1. **Architecture Simplification**: Moved from Helm charts to direct Kubernetes manifests
2. **Docker Compose Removal**: Eliminated Docker Compose in favor of Kubernetes NodePorts
3. **Port Forwarding Removal**: Services directly accessible via localhost NodePorts
4. **Script Consolidation**: All PowerShell automation moved to `devops/jobs/` folder
5. **Shared MongoDB**: Single MongoDB instance for local development efficiency
6. **NodePort Services**: Direct access without kubectl port-forward commands

### Implementation Details

1. **Project Files**: Added `UserSecretsId` to each service
2. **appsettings.json**: Replaced credentials with placeholders
3. **Kubernetes Manifests**: Updated to use secretKeyRef with shared MongoDB
4. **Secrets Structure**: Service-specific secrets pointing to shared MongoDB instance
5. **Automation**: Complete PowerShell job automation for deployment and secrets

### Migration from Previous Approach

- **Docker Compose**: Removed - replaced with Kubernetes NodePort services
- **Port Forwarding**: No longer needed - services accessible via localhost:30001-30004
- **Helm Charts**: Removed - replaced with direct Kubernetes manifests
- **Shared MongoDB**: Single instance for local development efficiency
- **Automation**: All scripts consolidated in `devops/jobs/` folder

## Secure Endpoint Discovery

**Never hardcode ports or endpoints in documentation!** Use these secure methods:

```powershell
# Discover current service endpoints securely
cd .\devops\jobs
.\show-architecture.ps1

# Check Kubernetes service status
kubectl get services -n capstone-services

# View deployment status
kubectl get pods -n capstone-services
```

## Automation Jobs

All PowerShell automation scripts are centralized in `devops/jobs/`:

### Available Jobs
- **`setup-user-secrets.ps1`**: Configure .NET User Secrets for all services
- **`apply-secrets.ps1`**: Apply Kubernetes secrets to cluster
- **`deploy-local-k8s.ps1`**: Deploy local Kubernetes with shared MongoDB
- **`cleanup-local-k8s.ps1`**: Clean up local Kubernetes deployments
- **`deploy-production.ps1`**: Deploy production with dedicated MongoDB instances
- **`show-architecture.ps1`**: Display current system status and architecture

### Quick Setup Workflow
```powershell
# Navigate to jobs folder
cd .\devops\jobs

# Setup local development secrets (credentials configured automatically)
.\setup-user-secrets.ps1

# Deploy local Kubernetes environment (secrets applied securely)
.\deploy-local-k8s.ps1

# Services now available on localhost with NodePort access
# Use .\show-architecture.ps1 to see current endpoints
# MongoDB: accessible via automation scripts only
```

## Service Endpoints

| Service | Access Method | URL Pattern | Database | Connection |
|---------|---------------|-------------|----------|------------|
| Authentication | NodePort | http://localhost:[port] | authenticationdb | Via User Secrets |
| Catalog | NodePort | http://localhost:[port] | catalogdb | Via User Secrets |
| CRM | NodePort | http://localhost:[port] | crmdb | Via User Secrets |
| Cart | NodePort | http://localhost:[port] | cartdb | Via User Secrets |
| MongoDB | Cluster Internal | *Connect via automation* | All databases | Via K8s Secrets |

**Note**: Specific ports are configured via automation scripts - consult deployment scripts for actual values.

## Future Enhancements

- [ ] Implement Azure Key Vault integration
- [ ] Add certificate-based authentication
- [ ] Implement credential rotation automation
- [ ] Add secrets validation scripts
- [ ] Integrate with CI/CD pipeline secrets management