# Centralized Environment Configuration

This directory uses a centralized approach for managing environment variables across all services to ensure consistency and ease of maintenance.

## Configuration Structure

### 1. **common-config.yaml** - Shared Non-Sensitive Configuration
Contains environment variables that are common across all .NET services:
- `ASPNETCORE_ENVIRONMENT`: Development/Production mode
- `ASPNETCORE_URLS`: Service listening URLs
- Logging configurations
- Service URLs for inter-service communication
- MongoDB connection pool settings
- CORS settings
- JWT configuration (non-secret)
- Rate limiting defaults

**Used by**: All .NET services (authentication, catalog, crm, cart, order, payment)

### 2. **mongodb-secret.yaml** - Sensitive Database Credentials
Contains MongoDB connection strings for each service:
- `auth-connection-string`: For authentication service
- `catalog-connection-string`: For catalog service
- `crm-connection-string`: For CRM service
- `cart-connection-string`: For cart service
- `order-connection-string`: For order service
- `payment-connection-string`: For payment service

**Type**: Kubernetes Secret (base64 encoded)
**Used by**: All .NET services that need database access

### 3. **Service-Specific ConfigMaps**
Some services have their own ConfigMaps for service-specific settings:
- `gateway-config`: Gateway service URLs, ports, rate limits
- `customer-app-config`: Frontend environment variables

## How It Works

### For .NET Services

Each service YAML now follows this pattern:

```yaml
spec:
  containers:
  - name: service-name
    # Load common environment variables
    envFrom:
    - configMapRef:
        name: common-service-config
    # Override or add service-specific variables
    env:
    - name: MONGO_CONNECTION_STRING
      valueFrom:
        secretKeyRef:
          name: mongodb-secret
          key: service-connection-string
    - name: DatabaseSettings__DatabaseName
      value: "servicedb"
```

**Benefits:**
1. ✅ Common variables defined once in `common-config.yaml`
2. ✅ Service-specific variables (like database names) defined in individual YAMLs
3. ✅ Sensitive data (connection strings) in Secrets
4. ✅ Easy to update environment for all services at once
5. ✅ Reduced duplication and configuration drift

### Configuration Priority

When a variable is defined in multiple places, Kubernetes uses this priority:
1. **env** variables (service-specific) - Highest priority
2. **envFrom** ConfigMap variables
3. Container defaults - Lowest priority

This means service-specific `env` variables will override common ConfigMap values if there's a conflict.

## Updating Configuration

### To Change a Common Setting:
1. Edit `common-config.yaml`
2. Apply the change: `kubectl apply -f common-config.yaml`
3. Restart affected services: `kubectl rollout restart deployment/service-name -n capstone-services`

### To Change a Service-Specific Setting:
1. Edit the service's YAML file (e.g., `authentication.yaml`)
2. Apply the change: `kubectl apply -f authentication.yaml`
3. Service will automatically restart with new config

### To Change MongoDB Connection Strings:
1. Edit `mongodb-secret.yaml`
2. Update the base64-encoded connection string
3. Apply: `kubectl apply -f mongodb-secret.yaml`
4. Restart services: `kubectl rollout restart deployment -n capstone-services`

## Environment Variables Reference

### Common to All Services (from common-config.yaml)

| Variable | Value | Description |
|----------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Development | ASP.NET Core environment |
| `ASPNETCORE_URLS` | http://+:8080 | Service listening URL |
| `AUTHENTICATION_SERVICE_URL` | http://authentication-service:8080 | Auth service URL |
| `CATALOG_SERVICE_URL` | http://catalog-service:8080 | Catalog service URL |
| `CRM_SERVICE_URL` | http://crm-service:8080 | CRM service URL |
| `CART_SERVICE_URL` | http://cart-service:8080 | Cart service URL |
| `ORDER_SERVICE_URL` | http://order-service:8080 | Order service URL |
| `GATEWAY_SERVICE_URL` | http://gateway-service:5000 | Gateway URL |
| `CORS_ORIGIN` | http://localhost:3000 | Allowed CORS origin |
| `JWT_ISSUER` | CapstoneAuthService | JWT token issuer |
| `JWT_AUDIENCE` | CapstoneServices | JWT token audience |
| `JWT_EXPIRY_MINUTES` | 480 | JWT token expiry (8 hours) |
| `DatabaseSettings__MaxConnectionPoolSize` | 100 | MongoDB max connections |
| `DatabaseSettings__ConnectTimeout` | 30000 | MongoDB connect timeout |

### Service-Specific Variables

Each service defines:
- `MONGO_CONNECTION_STRING`: From mongodb-secret
- `ConnectionStrings__DefaultConnection`: From mongodb-secret
- `DatabaseSettings__ConnectionString`: From mongodb-secret
- `DatabaseSettings__DatabaseName`: Service-specific database name
- `DatabaseSettings__CollectionName`: Primary collection name

## Migration from Old Configuration

**Before:**
```yaml
env:
- name: ASPNETCORE_ENVIRONMENT
  value: "Development"
- name: MONGO_CONNECTION_STRING
  valueFrom:
    secretKeyRef:
      name: mongodb-secret
      key: auth-connection-string
# ... repeated in every service
```

**After:**
```yaml
envFrom:
- configMapRef:
    name: common-service-config
env:
- name: MONGO_CONNECTION_STRING
  valueFrom:
    secretKeyRef:
      name: mongodb-secret
      key: auth-connection-string
# Only service-specific variables here
```

## Deployment Order

The `deploy.ps1` script applies configuration in this order:
1. Namespace
2. **common-config.yaml** ← Common configuration
3. mongodb-secret.yaml
4. mongodb-config.yaml
5. mongodb.yaml
6. All service deployments

This ensures the common configuration exists before services try to reference it.

## Troubleshooting

### Service not picking up common config?
```bash
# Check if ConfigMap exists
kubectl get configmap common-service-config -n capstone-services

# View ConfigMap contents
kubectl describe configmap common-service-config -n capstone-services

# Check if service is loading it
kubectl describe pod <pod-name> -n capstone-services | grep -A 10 "Environment"
```

### Need to verify environment variables in a running pod?
```bash
kubectl exec -it <pod-name> -n capstone-services -- printenv | sort
```

### Service-specific variable not overriding common config?
Ensure the service-specific variable is in the `env:` section, not `envFrom:`. Variables in `env:` have higher priority.

## Best Practices

1. **Common values go in common-config.yaml**: If 3+ services use it, put it in common config
2. **Secrets stay in Secrets**: Never put sensitive data in ConfigMaps
3. **Service-specific stays in service YAML**: Database names, collection names, etc.
4. **Document changes**: Update this README when adding new common variables
5. **Test after changes**: Always restart services and verify they pick up new config

## Future Enhancements

Consider adding:
- Environment-specific configs (dev, staging, prod)
- External configuration service (e.g., Consul, etcd)
- Dynamic configuration reload without restarts
- Configuration validation at deployment time
