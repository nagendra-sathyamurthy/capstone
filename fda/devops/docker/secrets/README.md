# Docker Secrets Directory

This directory contains sensitive configuration files used by Docker Compose.

## ⚠️ IMPORTANT SECURITY NOTICE

**NEVER commit actual secret files to version control!**

All `.txt`, `.key`, and `.pem` files in this directory are gitignored.

## Setup

Run the setup script to create your secret files:

```powershell
.\setup-docker-secrets.ps1
```

## Required Secret Files

The following files must be present for Docker Compose to work:

- `mongo_root_username.txt` - MongoDB root username
- `mongo_root_password.txt` - MongoDB root password
- `mongo_connection_string.txt` - MongoDB connection string for most services
- `mongo_connection_string_crm.txt` - MongoDB connection string for CRM service (includes database name)
- `jwt_secret_key.txt` - JWT secret key for authentication service

## Manual Setup

If you prefer to create secrets manually:

```powershell
# MongoDB credentials
echo "admin" > secrets/mongo_root_username.txt
echo "YourSecurePassword" > secrets/mongo_root_password.txt

# Connection strings
echo "mongodb://admin:YourSecurePassword@mongodb:27017" > secrets/mongo_connection_string.txt
echo "mongodb://admin:YourSecurePassword@mongodb:27017/crmdb?authSource=admin" > secrets/mongo_connection_string_crm.txt

# JWT secret (generate a random 64-character string)
echo "YourJWTSecretKeyHere" > secrets/jwt_secret_key.txt
```

## Production Usage

For production deployments:

1. Use Docker Swarm secrets or Kubernetes secrets instead of files
2. Use a proper secrets management solution (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
3. Rotate secrets regularly
4. Use strong, randomly generated passwords
5. Never share secrets via insecure channels

## Verification

To verify your secrets are set up correctly:

```powershell
# Check if all required files exist
Get-ChildItem -Path secrets -Filter *.txt | Select-Object Name

# Verify Docker Compose can read secrets
docker compose -f docker-compose-working.yml config
```
