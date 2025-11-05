# MongoDB Secrets Management

This document explains the secure MongoDB credentials management implementation for the Capstone microservices platform.

## Overview

MongoDB credentials and connection strings have been moved from configuration files to secure secrets management:

- **Kubernetes Deployment**: Uses Kubernetes Secrets
- **Local Development**: Uses .NET User Secrets
- **Configuration Files**: Contain placeholder values only

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
   cd c:\dotnet\capstone\fda\devops\scripts
   .\setup-user-secrets.ps1
   ```

2. **Manual Setup**:
   ```powershell
   # For each service (authentication, catalog, crm, cart)
   cd c:\dotnet\capstone\fda\src\services\[service]\Services
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "mongodb://sa:sa@localhost:9000/[database]?authSource=admin"
   dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://sa:sa@localhost:9000/[database]?authSource=admin"
   ```

### For DevOps (Kubernetes Deployment)

1. **Apply Secrets**:
   ```powershell
   cd c:\dotnet\capstone\fda\devops\scripts
   .\apply-secrets.ps1
   ```

2. **Manual Application**:
   ```bash
   kubectl apply -f ../kubernetes/mongodb-secret.yaml
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
    "DefaultConnection": "mongodb://sa:sa@localhost:9000/[database]?authSource=admin"
  },
  "DatabaseSettings": {
    "ConnectionString": "mongodb://sa:sa@localhost:9000/[database]?authSource=admin"
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

- **Local Development**: `mongodb://Athena:Ath3n@Str0ngP@ssw0rd2024!@localhost:9000/[database]?authSource=admin`
- **Kubernetes**: `mongodb://Athena:Ath3n@Str0ngP@ssw0rd2024!@mongodb-service:27017/[database]?authSource=admin`

## Security Best Practices

### What's Secure ✅
- Credentials stored in Kubernetes secrets (base64 encoded)
- User secrets stored in OS user profile (encrypted)
- No credentials in source code or configuration files
- Environment-specific credential isolation

### Additional Recommendations 🔒
1. **Rotate Credentials Regularly**: Update MongoDB passwords periodically
2. **Use Strong Passwords**: Replace default 'sa/sa' with strong credentials
3. **Network Policies**: Implement Kubernetes network policies
4. **RBAC**: Configure appropriate Kubernetes role-based access
5. **Monitoring**: Monitor secret access and usage

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
   cd c:\dotnet\capstone\fda\src\services\[service]\Services
   dotnet user-secrets list
   
   # Check MongoDB is running
   docker ps | findstr mongo
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

# Decode secret value (for debugging)
kubectl get secret mongodb-secret -n capstone-services -o jsonpath='{.data.username}' | base64 -d

# Check pod environment variables
kubectl describe pod [pod-name] -n capstone-services

# View user secrets for a service
cd [service-path]
dotnet user-secrets list
```

## Migration Notes

### Changes Made

1. **Project Files**: Added `UserSecretsId` to each service
2. **appsettings.json**: Replaced credentials with placeholders
3. **Kubernetes Manifests**: Updated to use secretKeyRef instead of hardcoded values
4. **New Secrets**: Created service-specific secrets in Kubernetes
5. **Scripts**: Added automation for setup and deployment

### Backward Compatibility

- **Development**: Still works with appsettings.Development.json overrides
- **Docker Compose**: Uses environment variables (no change needed)
- **Kubernetes**: Requires new secrets to be applied

## Future Enhancements

- [ ] Implement Azure Key Vault integration
- [ ] Add certificate-based authentication
- [ ] Implement credential rotation automation
- [ ] Add secrets validation scripts
- [ ] Integrate with CI/CD pipeline secrets management