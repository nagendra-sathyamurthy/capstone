# Port Configuration Simplification - Summary

## What Changed?

Previously, Docker Compose and Kubernetes used **different port numbers**:
- Docker Compose: 3000, 5000, 8081-8086, 27017
- Kubernetes NodePort: 30300, 30500, 30081-30086, 30017

Now they use **the same ports everywhere**: 3000, 5000, 8081-8086, 27017

## Why the Change?

You were absolutely right! Having the same port numbers is simpler and better for developer experience.

**The Problem**: Kubernetes NodePort services must use ports in the 30000-32767 range. We cannot use standard ports like 3000, 5000, or 8081 as NodePorts.

**The Solution**: Use ClusterIP services (internal only) + kubectl port-forward to expose services on the same ports as Docker Compose.

## Files Modified

### Kubernetes Manifests (9 files)
All services changed from `NodePort` to `ClusterIP`:

1. `fda/devops/kubernetes/local/mongodb.yaml`
2. `fda/devops/kubernetes/local/authentication.yaml`
3. `fda/devops/kubernetes/local/catalog.yaml`
4. `fda/devops/kubernetes/local/crm.yaml`
5. `fda/devops/kubernetes/local/cart.yaml`
6. `fda/devops/kubernetes/local/order.yaml`
7. `fda/devops/kubernetes/local/payment.yaml`
8. `fda/devops/kubernetes/local/gateway.yaml`
9. `fda/devops/kubernetes/local/customer-app.yaml`

**Before:**
```yaml
spec:
  type: NodePort
  ports:
  - port: 8080
    targetPort: 8080
    nodePort: 30081
```

**After:**
```yaml
spec:
  type: ClusterIP
  ports:
  - port: 8080
    targetPort: 8080
```

### New Script
**`fda/devops/kubernetes/port-forward.ps1`** - Automated port forwarding

Features:
- Forwards all 9 services to match Docker Compose ports
- Background job monitoring
- Auto-restart failed port-forwards
- Easy start/stop with `-Stop` parameter

### Updated Documentation
**`fda/devops/PORT-CONFIGURATION.md`** - Completely rewritten

New content:
- Explanation of same-port strategy
- Why ClusterIP instead of NodePort
- Port forwarding setup guide
- Troubleshooting section
- Testing commands (identical for both platforms)

## How to Use

### Docker Compose (No Change)
```powershell
cd fda/devops/docker
.\deploy.ps1
```

Services available at: http://localhost:3000, http://localhost:5000, etc.

### Kubernetes (New Workflow)
```powershell
# Deploy to Kubernetes
kubectl apply -f fda/devops/kubernetes/local/

# Start port forwarding (new step!)
cd fda/devops/kubernetes
.\port-forward.ps1
```

Services available at: http://localhost:3000, http://localhost:5000, etc. **(same as Docker!)**

### Stop Port Forwarding
```powershell
.\port-forward.ps1 -Stop
```

## Benefits

✅ **Same ports everywhere** - No confusion, no mental mapping  
✅ **Simpler URLs** - Same localhost:3000, localhost:5000 for both platforms  
✅ **Production-like** - ClusterIP is the standard Kubernetes service type  
✅ **One script** - Automated port forwarding for all services  
✅ **Easy switch** - Move between Docker and Kubernetes seamlessly  

## Trade-offs

| Aspect | Docker Compose | Kubernetes (Old) | Kubernetes (New) |
|--------|---------------|------------------|------------------|
| **Ports** | 3000, 5000, 8081... | 30300, 30500, 30081... | 3000, 5000, 8081... ✅ |
| **Setup** | `docker-compose up` | `kubectl apply` | `kubectl apply` + `port-forward.ps1` |
| **Extra Step** | None | None | Run port-forward script |
| **Service Type** | N/A | NodePort (uncommon) | ClusterIP (standard) ✅ |
| **Consistency** | ❌ Different from K8s | ❌ Different from Docker | ✅ Same as Docker |

## Alternative Approaches (Not Chosen)

1. **Keep NodePort with 30000+ ports**
   - ❌ Different ports from Docker Compose
   - ❌ Hard to remember (30081 vs 8081?)
   - ❌ Not production-like

2. **Use LoadBalancer**
   - ✅ Same ports possible
   - ❌ Requires MetalLB or cloud provider
   - ❌ Complex setup for local development

3. **Use Ingress Controller**
   - ✅ Production-ready
   - ✅ Hostname-based routing
   - ❌ Requires nginx-ingress or similar
   - ❌ Requires DNS or hosts file configuration
   - ❌ Overkill for local development

4. **ClusterIP + port-forward** ✅ **CHOSEN**
   - ✅ Same ports as Docker Compose
   - ✅ Simple one-script setup
   - ✅ Production-like service type
   - ⚠️ Requires extra script (but automated)

## Testing

Both platforms now have **identical** testing commands:

```powershell
# Test frontend
curl http://localhost:3000

# Test gateway
curl http://localhost:5000/health

# Test microservices
curl http://localhost:8081/health  # Authentication
curl http://localhost:8082/health  # Catalog
curl http://localhost:8083/health  # CRM
curl http://localhost:8084/health  # Cart
curl http://localhost:8085/health  # Order
curl http://localhost:8086/health  # Payment
```

No more "Is this Docker on 8081 or Kubernetes on 30081?" 🎉

## Next Steps

1. **Commit changes**:
   ```powershell
   git add .
   git commit -m "Simplify port configuration: use same ports for Docker and Kubernetes"
   git push origin user-story/sample-data
   ```

2. **Test Kubernetes deployment**:
   ```powershell
   kubectl apply -f fda/devops/kubernetes/local/
   cd fda/devops/kubernetes
   .\port-forward.ps1
   # Test services...
   .\port-forward.ps1 -Stop
   ```

3. **Update team documentation** if needed

## Questions?

- **Q: Why not just use NodePort?**  
  A: NodePort requires 30000-32767 range. Can't use 3000, 5000, 8081 directly.

- **Q: Is port-forward production-ready?**  
  A: No, port-forward is for local development. Production uses ClusterIP + Ingress or LoadBalancer.

- **Q: What if I forget to run port-forward.ps1?**  
  A: Services won't be accessible from localhost. You'll get connection refused errors.

- **Q: Can I use both Docker and Kubernetes simultaneously?**  
  A: No, you'll get port conflicts. Stop one before starting the other.

- **Q: Do I need to restart port-forward if a pod restarts?**  
  A: No, the script auto-monitors and restarts failed port-forwards.
