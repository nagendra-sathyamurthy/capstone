# HTTPS/TLS Configuration Guide for Production Deployment

## Overview
This guide explains how to deploy the Capstone application with HTTPS/TLS support in Kubernetes production environment using Ingress with SSL/TLS termination.

## Architecture Changes

### Before (HTTP)
- Services exposed via LoadBalancer with individual ports
- Direct HTTP access to each service
- No SSL/TLS encryption

### After (HTTPS)
- Services exposed via Ingress Controller with TLS termination
- All services use ClusterIP (internal only)
- Ingress handles SSL/TLS and routing
- Automatic certificate management with cert-manager

## Prerequisites

### 1. Install NGINX Ingress Controller
```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.9.0/deploy/static/provider/cloud/deploy.yaml
```

Verify installation:
```bash
kubectl get pods -n ingress-nginx
kubectl get svc -n ingress-nginx
```

### 2. Install cert-manager (for automatic certificate management)
```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml
```

Verify installation:
```bash
kubectl get pods -n cert-manager
```

### 3. Domain Configuration
You need to configure DNS records pointing to your Ingress Controller's external IP:

Get the external IP:
```bash
kubectl get svc -n ingress-nginx ingress-nginx-controller
```

Configure these DNS A records:
- `auth.your-domain.com` → Ingress IP
- `catalog.your-domain.com` → Ingress IP
- `crm.your-domain.com` → Ingress IP
- `cart.your-domain.com` → Ingress IP
- `order.your-domain.com` → Ingress IP
- `payment.your-domain.com` → Ingress IP
- `api.your-domain.com` → Ingress IP (Gateway)
- `gateway.your-domain.com` → Ingress IP (Gateway alternative)
- `app.your-domain.com` → Ingress IP (Customer App)
- `www.your-domain.com` → Ingress IP (Customer App)

## Deployment Steps

### Step 1: Update Domain Names
Edit the following files and replace `your-domain.com` with your actual domain:

1. **devops/kubernetes/production/ingress.yaml**
   - Update all host entries
   - Update TLS certificate domains

2. **devops/kubernetes/production/tls-certificates.yaml**
   - Uncomment the certificate sections
   - Update email address for Let's Encrypt
   - Update domain names in certificates

3. **devops/kubernetes/production/gateway.yaml**
   - Update CORS_ORIGIN domain

4. **postman-collections/Capstone-Production-Environment.postman_environment.json**
   - Update all URL domains

### Step 2: Configure TLS Certificates

#### Option A: Let's Encrypt (Recommended for Production)

1. Edit `tls-certificates.yaml` and uncomment the ClusterIssuer section
2. Update the email address for Let's Encrypt notifications
3. Apply the configuration:
```bash
kubectl apply -f devops/kubernetes/production/tls-certificates.yaml
```

4. Verify the issuer:
```bash
kubectl get clusterissuer
```

#### Option B: Self-Signed Certificates (Testing Only)

1. Generate self-signed certificates:
```bash
# For services
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout tls.key -out tls.crt \
  -subj "/CN=*.your-domain.com"

# Create secret
kubectl create secret tls capstone-services-tls \
  --cert=tls.crt --key=tls.key \
  -n capstone-services

kubectl create secret tls capstone-gateway-tls \
  --cert=tls.crt --key=tls.key \
  -n capstone-gateway

kubectl create secret tls capstone-frontend-tls \
  --cert=tls.crt --key=tls.key \
  -n capstone-frontend
```

#### Option C: Existing Certificates

If you have existing certificates from a CA:
```bash
kubectl create secret tls capstone-services-tls \
  --cert=path/to/your/certificate.crt \
  --key=path/to/your/private.key \
  -n capstone-services
```

### Step 3: Deploy Application

1. Deploy namespaces:
```bash
kubectl apply -f devops/kubernetes/production/namespace.yaml
```

2. Deploy MongoDB secrets:
```bash
kubectl apply -f devops/kubernetes/production/mongodb-secrets.yaml
```

3. Deploy all services:
```bash
kubectl apply -f devops/kubernetes/production/authentication.yaml
kubectl apply -f devops/kubernetes/production/catalog.yaml
kubectl apply -f devops/kubernetes/production/crm.yaml
kubectl apply -f devops/kubernetes/production/cart.yaml
kubectl apply -f devops/kubernetes/production/order.yaml
kubectl apply -f devops/kubernetes/production/payment.yaml
kubectl apply -f devops/kubernetes/production/gateway.yaml
kubectl apply -f devops/kubernetes/production/customer-app.yaml
```

4. Deploy Ingress:
```bash
kubectl apply -f devops/kubernetes/production/ingress.yaml
```

### Step 4: Verify Deployment

1. Check all pods are running:
```bash
kubectl get pods -n capstone-services
kubectl get pods -n capstone-gateway
kubectl get pods -n capstone-frontend
```

2. Check services:
```bash
kubectl get svc -n capstone-services
kubectl get svc -n capstone-gateway
kubectl get svc -n capstone-frontend
```

3. Check Ingress:
```bash
kubectl get ingress -n capstone-services
kubectl get ingress -n capstone-gateway
kubectl get ingress -n capstone-frontend
```

4. Check certificates (if using cert-manager):
```bash
kubectl get certificate -n capstone-services
kubectl get certificate -n capstone-gateway
kubectl get certificate -n capstone-frontend
```

5. Check certificate secrets:
```bash
kubectl describe certificate -n capstone-services
```

## Service URLs

After deployment, your services will be accessible via:

| Service         | URL                                  |
|-----------------|--------------------------------------|
| Authentication  | https://auth.your-domain.com         |
| Catalog         | https://catalog.your-domain.com      |
| CRM             | https://crm.your-domain.com          |
| Cart            | https://cart.your-domain.com         |
| Order           | https://order.your-domain.com        |
| Payment         | https://payment.your-domain.com      |
| API Gateway     | https://api.your-domain.com          |
| Customer App    | https://app.your-domain.com          |

## Testing

### 1. Test Certificate
```bash
curl -v https://api.your-domain.com/health
openssl s_client -connect api.your-domain.com:443 -showcerts
```

### 2. Test Services
Use the updated Postman collection with the Production environment:
- Select "Capstone-Production-Environment"
- All URLs are now HTTPS
- Test authentication, catalog, cart workflows

### 3. Test Customer App
Open https://app.your-domain.com in your browser and verify:
- SSL certificate is valid (check padlock icon)
- All API calls work correctly
- No mixed content warnings

## Troubleshooting

### Issue: Certificate not issued
```bash
# Check certificate status
kubectl describe certificate -n capstone-services

# Check cert-manager logs
kubectl logs -n cert-manager -l app=cert-manager

# Check certificate request
kubectl get certificaterequest -n capstone-services
kubectl describe certificaterequest <name> -n capstone-services
```

### Issue: Ingress not working
```bash
# Check Ingress controller logs
kubectl logs -n ingress-nginx -l app.kubernetes.io/name=ingress-nginx

# Check Ingress configuration
kubectl describe ingress -n capstone-services

# Check backend service
kubectl get endpoints -n capstone-services
```

### Issue: DNS not resolving
```bash
# Verify external IP
kubectl get svc -n ingress-nginx ingress-nginx-controller

# Test DNS resolution
nslookup api.your-domain.com
dig api.your-domain.com
```

### Issue: Mixed content warnings
Ensure all internal service URLs in gateway configuration use HTTPS, or use internal cluster DNS for service-to-service communication.

## Security Considerations

### 1. Certificate Management
- Use Let's Encrypt for automatic renewal
- Monitor certificate expiration dates
- Keep cert-manager updated

### 2. TLS Configuration
- Current setup uses TLS 1.2+ (configured in Ingress)
- Strong cipher suites are enabled by default
- HTTP to HTTPS redirect is enforced

### 3. CORS Configuration
Update gateway CORS_ORIGIN to match your frontend domain:
```yaml
CORS_ORIGIN: "https://app.your-domain.com"
```

### 4. Rate Limiting
Gateway includes rate limiting (100 requests per 15 minutes per IP).
Adjust in gateway-config ConfigMap if needed.

### 5. Network Policies (Optional)
Consider implementing NetworkPolicies to restrict traffic between namespaces.

## Monitoring

### Certificate Expiry
```bash
kubectl get certificate -n capstone-services -o wide
```

### Ingress Metrics
```bash
kubectl top pods -n ingress-nginx
```

### Service Health
```bash
# Check all service endpoints
for svc in authentication catalog crm cart order payment; do
  echo "Testing $svc..."
  curl -k https://${svc}.your-domain.com/health
done
```

## Rollback

If issues occur, you can temporarily disable HTTPS:

1. Scale down Ingress controller:
```bash
kubectl scale deployment ingress-nginx-controller -n ingress-nginx --replicas=0
```

2. Change services back to LoadBalancer (not recommended)

## Cost Considerations

- **NGINX Ingress Controller**: Runs on cluster nodes (minimal cost)
- **cert-manager**: Lightweight, runs on cluster (minimal cost)
- **Let's Encrypt**: Free certificates
- **LoadBalancer costs eliminated**: Using single Ingress instead of multiple LoadBalancers saves significant cloud provider costs

## Next Steps

1. Set up monitoring with Prometheus/Grafana
2. Configure log aggregation for Ingress
3. Implement backup strategy for TLS secrets
4. Set up automated testing for certificate renewal
5. Configure CDN (optional) for customer app static assets
6. Implement Web Application Firewall (WAF) rules

## Additional Resources

- [NGINX Ingress Documentation](https://kubernetes.github.io/ingress-nginx/)
- [cert-manager Documentation](https://cert-manager.io/docs/)
- [Let's Encrypt Documentation](https://letsencrypt.org/docs/)
- [Kubernetes Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/)
