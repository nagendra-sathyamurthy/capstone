# Local Docker Deployment

This directory contains Docker Compose configuration for local development deployment of all Capstone services.

## Prerequisites

- Docker Desktop installed and running
- All service Docker images built

## Setup

1. **Copy the environment template:**
   ```powershell
   Copy-Item .env.example .env
   ```

2. **Edit `.env` file** with your actual credentials:
   ```powershell
   notepad .env
   ```
   
   Update the MongoDB credentials and connection strings as needed.

## Usage

### Start all services:
```powershell
docker-compose -f docker-compose-local.yml up -d
```

### Stop all services:
```powershell
docker-compose -f docker-compose-local.yml down
```

### View logs:
```powershell
# All services
docker-compose -f docker-compose-local.yml logs -f

# Specific service
docker-compose -f docker-compose-local.yml logs -f authentication
```

### Check status:
```powershell
docker-compose -f docker-compose-local.yml ps
```

## Service Endpoints

- **Authentication**: http://localhost:30001
- **Catalog**: http://localhost:30002
- **CRM**: http://localhost:30003
- **Cart**: http://localhost:30004
- **MongoDB**: mongodb://localhost:30000

## Environment Variables

All sensitive configuration is stored in the `.env` file, which is **NOT** committed to Git for security.

See `.env.example` for the required environment variables.

## Security Notes

- The `.env` file contains sensitive credentials and should NEVER be committed to version control
- For production deployments, use proper secrets management (Azure Key Vault, Kubernetes Secrets, etc.)
- Change default passwords before deploying to any non-local environment
