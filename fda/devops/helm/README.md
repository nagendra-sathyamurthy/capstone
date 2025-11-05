# Capstone Microservices - Helm Charts

This directory contains production-ready Helm charts for deploying MongoDB databases that support the capstone microservices architecture.

## � Chart Structure

```
helm/
├── README.md
├── authentication/charts/    # Authentication service MongoDB
├── cart/charts/             # Shopping cart service MongoDB  
├── catalog/charts/          # Product catalog service MongoDB
└── crm/charts/             # Customer relationship service MongoDB
```

## 🚀 Chart Features

### Production-Ready Configuration
- **Version**: All charts are at `v1.0.0` production release
- **MongoDB Image**: Stable `mongo:7.0` (pinned version)
- **Chart Type**: Application charts with full Kubernetes integration
- **Maintainers**: DevOps team with proper contact information
- **Documentation**: Comprehensive values.yaml with inline documentation

### 🔒 Security & Authentication
```yaml
mongodb:
  auth:
    enabled: true
    rootUser: admin
    rootPassword: "SecurePass123!"
    database: [service-specific-db]
    username: [service-user]
    password: "[Service]Pass123!"
```

### 📊 Resource Management
```yaml
resources:
  limits:
    cpu: 500m
    memory: 512Mi
  requests:
    cpu: 100m
    memory: 256Mi
```

### 🏥 Health Monitoring
```yaml
livenessProbe:
  enabled: true
  initialDelaySeconds: 30
  periodSeconds: 10
  command: ["mongo", "--eval", "db.adminCommand('ping')"]

readinessProbe:
  enabled: true
  initialDelaySeconds: 5
  periodSeconds: 5
  command: ["mongo", "--eval", "db.adminCommand('ping')"]
```

### 💾 Storage Configuration
```yaml
persistence:
  enabled: true
  storageClass: ""
  accessMode: ReadWriteOnce
  size: 2Gi  # Upgraded from 1Gi
  annotations: {}
```

### 🛡️ Security Context
```yaml
securityContext:
  enabled: true
  fsGroup: 999
  runAsUser: 999
```

## 📦 Available Services

Each service has its own dedicated MongoDB database with secure authentication:

| Service | Database | Username | Chart Location |
|---------|----------|----------|----------------|
| **Authentication** | `authenticationdb` | `authuser` | `./authentication/charts/` |
| **Cart** | `cartdb` | `cartuser` | `./cart/charts/` |
| **Catalog** | `catalogdb` | `cataloguser` | `./catalog/charts/` |
| **CRM** | `crmdb` | `crmuser` | `./crm/charts/` |

> **Security Note**: All databases use strong authentication with service-specific credentials configured in `values.yaml`

## 🎯 Helm Best Practices Implemented

### Template Helpers (`_helpers.tpl`)
- Consistent naming functions
- Proper label generation
- Kubernetes standard labels
- Chart and app version tracking

### Kubernetes Labels
```yaml
labels:
  app.kubernetes.io/name: [service]-mongodb
  app.kubernetes.io/instance: [release-name]
  app.kubernetes.io/version: "7.0"
  app.kubernetes.io/managed-by: Helm
  app.kubernetes.io/component: database
  app.kubernetes.io/part-of: capstone-microservices
```

### Configuration Options
- Node selectors for placement control
- Tolerations for special node requirements
- Affinity rules for pod placement
- Pod annotations for additional metadata

## � Quick Start

### Prerequisites
- Kubernetes cluster (v1.20+)
- Helm 3.x installed
- kubectl configured

### Deploy Individual Service
```bash
# Deploy authentication database
helm install auth-db ./authentication/charts/ -n capstone-services --create-namespace

# Deploy cart database  
helm install cart-db ./cart/charts/ -n capstone-services

# Deploy catalog database
helm install catalog-db ./catalog/charts/ -n capstone-services

# Deploy CRM database
helm install crm-db ./crm/charts/ -n capstone-services
```

### Deploy All Services (Batch)
```bash
# Create namespace first
kubectl create namespace capstone-services

# Deploy all databases
for service in authentication cart catalog crm; do
    helm install "${service}-db" ./${service}/charts/ -n capstone-services
done
```

### Custom Configuration Examples
```bash
# Deploy with custom storage size
helm install auth-db ./authentication/charts/ \
  --set persistence.size=5Gi \
  --set mongodb.resources.limits.memory=1Gi \
  -n capstone-services

# Deploy with custom credentials
helm install cart-db ./cart/charts/ \
  --set mongodb.auth.rootPassword="MySecureRootPass" \
  --set mongodb.auth.password="MySecureUserPass" \
  -n capstone-services
```

### Verify Deployments
```bash
# Check all pods
kubectl get pods -n capstone-services

# Check services
kubectl get svc -n capstone-services

# Check persistent volumes
kubectl get pvc -n capstone-services
```

## � Maintenance & Operations

### Upgrade Charts
```bash
# Upgrade specific service
helm upgrade auth-db ./authentication/charts/ -n capstone-services

# Upgrade all services
for service in authentication cart catalog crm; do
    helm upgrade "${service}-db" ./${service}/charts/ -n capstone-services
done
```

### Uninstall Services
```bash
# Remove specific service
helm uninstall auth-db -n capstone-services

# Remove all services
helm list -n capstone-services | grep -E "(auth|cart|catalog|crm)-db" | awk '{print $1}' | xargs -I {} helm uninstall {} -n capstone-services
```

### Monitoring & Troubleshooting
```bash
# Check deployment status
helm status auth-db -n capstone-services

# View logs
kubectl logs -l app.kubernetes.io/name=authentication-mongodb -n capstone-services

# Port forward for local access
kubectl port-forward svc/authentication-mongodb 27017:27017 -n capstone-services
```

## 🚦 Production Readiness Checklist

All charts include enterprise-grade features:

✅ **Stability & Versioning**
- Pinned MongoDB image (`mongo:7.0`)
- Semantic versioning for charts (`v1.0.0`)
- Immutable deployments

✅ **Security & Authentication** 
- MongoDB authentication enabled
- Service-specific user accounts
- Kubernetes security contexts
- Non-root container execution

✅ **Resource Management**
- CPU/Memory limits and requests
- Quality of Service (QoS) guaranteed
- Resource quotas compatible

✅ **High Availability**
- Liveness and readiness probes
- Persistent volume claims
- Graceful shutdown handling
- Rolling update strategy

✅ **Observability**
- Structured logging
- Health check endpoints  
- Kubernetes standard labels
- Helm release tracking

✅ **Best Practices**
- Helm template helpers
- Configurable via values.yaml
- Kubernetes naming conventions
- GitOps-ready structure

## 📚 Additional Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Helm Documentation](https://helm.sh/docs/)
- [MongoDB on Kubernetes](https://www.mongodb.com/kubernetes)
- [Project Repository](https://github.com/nagendra-sathyamurthy/capstone)