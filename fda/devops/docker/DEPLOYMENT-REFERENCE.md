# Deployment Quick Reference Card

## 📦 Initial Setup (One-time)

```powershell
cd fda/devops/docker
.\setup-docker-secrets.ps1
```

## 🚀 Deployment

### Full Deployment
```powershell
.\deploy.ps1
```

### Production Deployment
```powershell
.\deploy.ps1 -Environment production -SkipSecrets
```

### Deploy Specific Services
```powershell
.\deploy.ps1 -Services "catalog,gateway,mongodb"
```

### Clean Deployment (Fresh Start)
```powershell
.\deploy.ps1 -Clean
```

### Quick Redeploy (Skip Build)
```powershell
.\deploy.ps1 -SkipBuild
```

## 🛑 Stopping Services

### Stop (Keep Containers)
```powershell
.\cleanup.ps1 -Stop
```

### Remove Containers (Keep Data)
```powershell
.\cleanup.ps1 -Down
```

### Remove Everything (⚠️ Data Loss!)
```powershell
.\cleanup.ps1 -Clean
```

### Complete Cleanup (⚠️ Removes Images Too!)
```powershell
.\cleanup.ps1 -Prune
```

## 🔍 Monitoring

### View All Logs
```powershell
docker compose -f docker-compose-working.yml logs -f
```

### View Specific Service Logs
```powershell
docker compose -f docker-compose-working.yml logs -f gateway
docker compose -f docker-compose-working.yml logs -f catalog
```

### Check Container Status
```powershell
docker compose -f docker-compose-working.yml ps
```

### Check Service Health
```powershell
# Gateway
curl http://localhost:5000/health

# Individual services
curl http://localhost:8081/swagger    # Authentication
curl http://localhost:8082/swagger    # Catalog
curl http://localhost:8083/swagger    # CRM
```

## 🔧 Troubleshooting

### Restart Specific Service
```powershell
docker compose -f docker-compose-working.yml restart gateway
```

### Rebuild and Restart Service
```powershell
docker compose -f docker-compose-working.yml up -d --build gateway
```

### Access MongoDB Shell
```powershell
docker compose -f docker-compose-working.yml exec mongodb mongosh -u admin -p AdminPass2024
```

### View Container Logs (Last 50 Lines)
```powershell
docker compose -f docker-compose-working.yml logs --tail=50 catalog
```

### Check Disk Space
```powershell
docker system df
```

### Clean Docker System
```powershell
docker system prune -a
```

## 📊 Service URLs

| Service | URL |
|---------|-----|
| Customer App | http://localhost:3000 |
| API Gateway | http://localhost:5000 |
| Gateway Health | http://localhost:5000/health |
| Authentication | http://localhost:8081 |
| Catalog | http://localhost:8082 |
| CRM | http://localhost:8083 |
| Cart | http://localhost:8084 |
| Order | http://localhost:8085 |
| MongoDB | mongodb://localhost:27017 |

## 🔐 Default Credentials (Development)

**MongoDB:**
- Username: `admin`
- Password: `AdminPass2024`

**Note:** Change these in production!

## 📁 Important Files

| File | Purpose |
|------|---------|
| `deploy.ps1` | Main deployment script |
| `cleanup.ps1` | Cleanup script |
| `setup-docker-secrets.ps1` | Generate secrets |
| `docker-compose-working.yml` | Service definitions |
| `.env` | Environment configuration |
| `secrets/*.txt` | Secret files (gitignored) |

## ⚡ Quick Commands Reference

```powershell
# Full deployment
.\deploy.ps1

# Stop everything
.\cleanup.ps1 -Stop

# View logs
docker compose -f docker-compose-working.yml logs -f

# Check status
docker compose -f docker-compose-working.yml ps

# Restart service
docker compose -f docker-compose-working.yml restart <service-name>
```

## 🆘 Common Issues

### Port Already in Use
```powershell
# Find process using port
netstat -ano | findstr :5000

# Kill process (Windows)
taskkill /F /PID <process-id>
```

### MongoDB Connection Failed
```powershell
# Check MongoDB is running
docker compose -f docker-compose-working.yml ps mongodb

# Check MongoDB logs
docker compose -f docker-compose-working.yml logs mongodb

# Test connection
docker compose -f docker-compose-working.yml exec mongodb mongosh --eval "db.adminCommand('ping')"
```

### Service Won't Start
```powershell
# Check logs
docker compose -f docker-compose-working.yml logs <service-name>

# Rebuild service
docker compose -f docker-compose-working.yml up -d --build <service-name>

# Check secrets
ls secrets/*.txt
```

## 💡 Tips

- Always run scripts from `fda/devops/docker` directory
- Use `-Clean` flag for fresh deployments
- Check logs if services fail health checks
- Use `-Services` parameter for faster partial deployments
- Keep secrets files secure and never commit them
- Use `.\deploy.ps1 -SkipBuild` for quick configuration changes
