# Local Development Setup Guide

This guide helps developers set up their local development environment for debugging microservices in VS Code.

## Prerequisites

- .NET 8.0 SDK
- MongoDB (local or remote)
- VS Code with C# Dev Kit extension
- Git

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/nagendra-sathyamurthy/capstone.git
cd capstone
```

### 2. Configure User Secrets

MongoDB connection strings are stored in **user secrets** for security. Run the setup script:

```powershell
.\fda\devops\jobs\setup-local-user-secrets.ps1
```

This will prompt you for your MongoDB credentials and configure all services.

**Alternative: Manual Setup**

If you prefer to set secrets manually for each service:

```bash
# Authentication Service
dotnet user-secrets set "MONGO_CONNECTION_STRING" "mongodb://username:password@localhost:27017/authenticationdb?authSource=admin" --project fda/src/services/authentication/API

# Cart Service
dotnet user-secrets set "MONGO_CONNECTION_STRING" "mongodb://username:password@localhost:27017/cartdb?authSource=admin" --project fda/src/services/cart/API

# Catalog Service
dotnet user-secrets set "MONGO_CONNECTION_STRING" "mongodb://username:password@localhost:27017/catalogdb?authSource=admin" --project fda/src/services/catalog/API

# CRM Service
dotnet user-secrets set "MONGO_CONNECTION_STRING" "mongodb://username:password@localhost:27017/crmdb?authSource=admin" --project fda/src/services/crm/API
```

### 3. Build All Services

```bash
dotnet build fda/src/services/capstone.sln
```

Or use VS Code task: `Ctrl+Shift+B`

### 4. Run & Debug Services

#### Option 1: Debug Single Service
1. Press `F5` in VS Code
2. Select the service you want to debug from the dropdown

#### Option 2: Debug All Services
1. Press `F5` in VS Code
2. Select "All Microservices" compound configuration
3. All services will start simultaneously

## Service Ports

When running locally, services listen on these ports (matching Postman environment):

- **Authentication**: http://localhost:30001
- **Catalog**: http://localhost:30002
- **CRM**: http://localhost:30003
- **Cart**: http://localhost:30004

## Managing User Secrets

### View Current Secrets

```bash
dotnet user-secrets list --project fda/src/services/authentication/API
```

### Update a Secret

```bash
dotnet user-secrets set "MONGO_CONNECTION_STRING" "new-connection-string" --project fda/src/services/authentication/API
```

### Remove a Secret

```bash
dotnet user-secrets remove "MONGO_CONNECTION_STRING" --project fda/src/services/authentication/API
```

### Clear All Secrets

```bash
dotnet user-secrets clear --project fda/src/services/authentication/API
```

## MongoDB Setup

### Local MongoDB with Docker

```bash
docker run -d -p 27017:27017 --name mongodb \
  -e MONGO_INITDB_ROOT_USERNAME=admin \
  -e MONGO_INITDB_ROOT_PASSWORD=admin123 \
  mongo:latest
```

### MongoDB Connection String Format

```
mongodb://username:password@host:port/database?authSource=admin
```

## VS Code Tasks

Available tasks (Terminal > Run Task):

- `build-all` - Build all services (default: Ctrl+Shift+B)
- `build-authentication` - Build Authentication service
- `build-cart` - Build Cart service
- `build-catalog` - Build Catalog service
- `build-crm` - Build CRM service
- `watch-authentication` - Run Authentication with hot reload
- `watch-cart` - Run Cart with hot reload
- `watch-catalog` - Run Catalog with hot reload
- `watch-crm` - Run CRM with hot reload
- `clean-all` - Clean all build artifacts
- `restore-all` - Restore NuGet packages

## Testing with Postman

1. Import collections from `fda/postman-collections/`
2. Select "Capstone Local Development Environment"
3. Services should be accessible at the configured ports

## Troubleshooting

### Issue: Service fails to start with MongoDB connection error

**Solution**: Verify your user secrets are set correctly:
```bash
dotnet user-secrets list --project fda/src/services/<service-name>/API
```

### Issue: Port already in use

**Solution**: Check if another instance is running:
```powershell
# Find process using port 30001
netstat -ano | findstr :30001
# Kill the process
taskkill /PID <process-id> /F
```

### Issue: Cannot connect to MongoDB

**Solution**: 
1. Verify MongoDB is running: `docker ps` or check MongoDB service
2. Test connection: `mongosh "mongodb://admin:admin123@localhost:27017"`
3. Check firewall settings

## Security Best Practices

✅ **DO:**
- Store credentials in user secrets
- Use different passwords for dev/staging/production
- Never commit secrets to git

❌ **DON'T:**
- Hardcode connection strings in code
- Store secrets in launchSettings.json
- Share secrets via chat/email

## Additional Resources

- [.NET User Secrets Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [VS Code Debugging](https://code.visualstudio.com/docs/editor/debugging)
- [MongoDB Connection Strings](https://www.mongodb.com/docs/manual/reference/connection-string/)

## Support

For issues or questions, contact the development team or create an issue in the repository.
