# Docker Deployment Guide

This directory contains Docker Compose configurations for deploying the Capstone Food Delivery Application.

## Files

- **docker-compose-working.yml** - Production-ready stack with Docker secrets
- **.env.example** - Environment variables template (copy to .env)
- **secrets/** - Directory for sensitive configuration files
- **setup-docker-secrets.ps1** - Script to generate secret files

## Prerequisites

- Docker Desktop installed and running
- Docker Compose v2.0 or later
- At least 8GB RAM allocated to Docker
- Ports 3000, 5000, 8081-8085, 27017 available
- PowerShell (for Windows) to run setup scripts

## Quick Start

### 1. Setup Secrets

First, create the required secret files:

```powershell
# Run the setup script
.\setup-docker-secrets.ps1

# Or create secrets manually (see secrets/README.md)
```

### 2. Deploy with Script (Recommended)

Use the deployment script for automated deployment:

```powershell
# Full deployment with secret generation
.\deploy.ps1

# Production deployment with existing secrets
.\deploy.ps1 -Environment production -SkipSecrets

# Deploy specific services without rebuilding
.\deploy.ps1 -Services "catalog,gateway" -SkipBuild

# Clean deployment (removes existing data)
.\deploy.ps1 -Clean
```

### 3. Manual Deployment (Alternative)

Or deploy manually:

```bash
# Navigate to docker directory
cd fda/devops/docker

# Start all services
docker compose -f docker-compose-working.yml up -d

# View logs
docker compose -f docker-compose-working.yml logs -f
```

### 4. Stopping Services

```powershell
# Using cleanup script (recommended)
.\cleanup.ps1 -Stop                    # Stop services
.\cleanup.ps1 -Down                    # Remove containers
.\cleanup.ps1 -Clean                   # Remove data (WARNING!)
.\cleanup.ps1 -Prune                   # Complete cleanup

# Manual
docker compose -f docker-compose-working.yml stop        # Stop services
docker compose -f docker-compose-working.yml down        # Remove containers
docker compose -f docker-compose-working.yml down -v     # Remove containers and data
```

## Security with Docker Secrets

This deployment uses Docker secrets to protect sensitive information:

### ⚠️ Important Security Notes

- **NEVER** commit files in `secrets/` directory
- Secrets are stored in `/run/secrets/` inside containers
- Services read from secret files or environment variables
- `.gitignore` is configured to exclude secret files

### Secret Files Used

| Secret File | Purpose | Used By |
|-------------|---------|---------|
| `mongo_root_username.txt` | MongoDB root username | MongoDB |
| `mongo_root_password.txt` | MongoDB root password | MongoDB |
| `mongo_connection_string.txt` | MongoDB connection for services | Authentication, Catalog, Cart, Order |
| `mongo_connection_string_crm.txt` | MongoDB connection with CRM database | CRM |
| `jwt_secret_key.txt` | JWT signing key | Authentication |

### How Services Read Secrets

Services automatically:
1. Check `/run/secrets/<secret_name>` (Docker secrets)
2. Fall back to environment variables (local development)
3. Use default values if neither exists

Example from Program.cs:
```csharp
static string GetSecret(IConfiguration config, string secretName, string? defaultValue = null)
{
    var secretPath = $"/run/secrets/{secretName}";
    if (File.Exists(secretPath))
        return File.ReadAllText(secretPath).Trim();
    
    var envValue = config[secretName];
    if (!string.IsNullOrEmpty(envValue))
        return envValue;
    
    return defaultValue ?? throw new InvalidOperationException($"Secret not found: {secretName}");
}
```

## Service Ports

| Service | Port | URL |
|---------|------|-----|
| Customer App | 3000 | http://localhost:3000 |
| API Gateway | 5000 | http://localhost:5000 |
| Authentication | 8081 | http://localhost:8081 |
| Catalog | 8082 | http://localhost:8082 |
| CRM | 8083 | http://localhost:8083 |
| Cart | 8084 | http://localhost:8084 |
| Order | 8085 | http://localhost:8085 |
| Payment | 8086 | http://localhost:8086 |
| MongoDB | 27017 | mongodb://localhost:27017 |

## Service Dependencies

```
Customer App → Gateway → Backend Services → MongoDB
```

- Customer App requires Gateway
- Gateway requires all Backend Services
- Backend Services require MongoDB

## MongoDB Credentials

**Default credentials (Development):**
- Username: `admin`
- Password: `AdminPass2024`
- Connection String: `mongodb://admin:AdminPass2024@localhost:27017`

⚠️ **Important:** Change these credentials for production deployments!

## Building Images

Docker Compose will automatically build images from Dockerfiles. To rebuild:

```bash
# Rebuild all images
docker-compose build

# Rebuild specific service
docker-compose build gateway

# Rebuild without cache
docker-compose build --no-cache
```

## Useful Commands

### View running containers
```bash
docker-compose ps
```

### View logs for specific service
```bash
docker-compose logs gateway
docker-compose logs customer-app
docker-compose logs -f mongodb
```

### Restart specific service
```bash
docker-compose restart gateway
```

### Scale services
```bash
docker-compose up -d --scale gateway=3
```

### Execute commands in container
```bash
# Access MongoDB shell
docker-compose exec mongodb mongosh -u admin -p AdminPass2024

# Access Gateway container
docker-compose exec gateway sh
```

### Check service health
```bash
# Gateway health
curl http://localhost:5000/health

# Customer App
curl http://localhost:3000
```

## Network Configuration

All services run on a custom bridge network called `capstone-network`. This allows:
- Service-to-service communication using service names
- Isolation from other Docker networks
- Custom DNS resolution

## Volume Management

### View volumes
```bash
docker volume ls | grep capstone
```

### Backup MongoDB data
```bash
docker-compose exec mongodb mongodump --out /backup --username admin --password AdminPass2024
docker cp capstone-mongodb:/backup ./mongodb-backup
```

### Restore MongoDB data
```bash
docker cp ./mongodb-backup capstone-mongodb:/backup
docker-compose exec mongodb mongorestore /backup --username admin --password AdminPass2024
```

## Troubleshooting

### Services won't start
```bash
# Check Docker resources
docker system df

# Remove old containers and images
docker-compose down
docker system prune -a

# Rebuild from scratch
docker-compose build --no-cache
docker-compose up -d
```

### Port conflicts
```bash
# Find processes using ports
netstat -ano | findstr :3000
netstat -ano | findstr :5000

# Kill process (Windows)
taskkill /F /PID <process-id>
```

### Gateway can't reach backend services
```bash
# Check network connectivity
docker-compose exec gateway ping authentication
docker-compose exec gateway curl http://authentication:8080/health

# Verify service URLs in gateway logs
docker-compose logs gateway | grep -i "service url"
```

### MongoDB connection issues
```bash
# Test MongoDB connection
docker-compose exec mongodb mongosh -u admin -p AdminPass2024 --eval "db.adminCommand('ping')"

# Check MongoDB logs
docker-compose logs mongodb | tail -50
```

## Production Considerations

When deploying to production:

1. **Update Credentials**: Change MongoDB username/password
2. **Environment Variables**: Update `CORS_ORIGIN`, `NODE_ENV`, etc.
3. **Image Registry**: Push images to container registry (Docker Hub, ECR, ACR)
4. **Health Checks**: Configure proper health check endpoints
5. **Resource Limits**: Adjust CPU/memory limits based on load
6. **Logging**: Configure log aggregation (ELK, CloudWatch)
7. **Monitoring**: Set up monitoring (Prometheus, Grafana)
8. **Secrets Management**: Use Docker secrets or external secret managers
9. **SSL/TLS**: Configure HTTPS with proper certificates
10. **Backup Strategy**: Implement automated MongoDB backups

## Docker Compose vs Kubernetes

**Use Docker Compose when:**
- Local development
- Small deployments
- Single-server deployments
- Quick prototyping

**Use Kubernetes when:**
- Production deployments
- Multi-server clusters
- Auto-scaling needed
- Advanced orchestration required

## Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Dockerfile Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Docker Networking](https://docs.docker.com/network/)
