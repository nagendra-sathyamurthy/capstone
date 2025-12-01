# RabbitMQ Quick Reference Card

## 🚀 Quick Start

### Start with Docker Compose
```powershell
cd C:\dotnet\capstone\fda\devops\docker
docker-compose -f docker-compose-working.yml up -d
```

### Start with Kubernetes
```powershell
cd C:\dotnet\capstone\fda\devops\kubernetes\local
kubectl apply -f namespace.yaml
kubectl apply -f mongodb-config.yaml
kubectl apply -f mongodb-secret.yaml
kubectl apply -f mongodb.yaml
kubectl apply -f rabbitmq.yaml
kubectl apply -f authentication.yaml
kubectl apply -f order.yaml
kubectl apply -f cart.yaml
```

## 🔗 Access Points

| Service | URL | Credentials |
|---------|-----|-------------|
| **RabbitMQ Management UI** | http://localhost:15672 | admin / AdminPass2024 |
| **Order API** | http://localhost:5000/api/order | Requires JWT token |
| **Cart API** | http://localhost:5000/api/cart | Requires JWT token |
| **Authentication API** | http://localhost:5000/api/authentication | Public |

### Kubernetes Port Forwarding
```powershell
kubectl port-forward -n capstone-services svc/rabbitmq-service 15672:15672
kubectl port-forward -n capstone-services svc/order-service 5000:80
```

## 📬 Events Catalog

| Event | Routing Key | When Published |
|-------|-------------|----------------|
| **OrderCreatedEvent** | `order.created` | Order placed |
| **OrderAcceptedEvent** | `order.accepted` | Restaurant accepts order |
| **OrderDeclinedEvent** | `order.declined` | Restaurant declines order |
| **OrderStatusChangedEvent** | `order.status.changed` | Order status updates |
| **OrderReadyForPickupEvent** | `order.ready` | Food ready for pickup |
| **OrderDeliveredEvent** | `order.delivered` | Order delivered to customer |
| **PaymentCompletedEvent** | `payment.completed` | Payment successful |
| **PaymentFailedEvent** | `payment.failed` | Payment failed |

## 🎯 Service Queues

| Queue Name | Consumer Service | Purpose |
|------------|------------------|---------|
| `order-service-queue` | Order Service | Receive payment/delivery updates |
| `cart-service-queue` | Cart Service | Clear cart after payment |
| `payment-service-queue` | Payment Service | Process order payments |
| `crm-service-queue` | CRM Service | Send customer notifications |

## 🧪 Testing with Postman

### Import Files
1. Collection: `fda/postman-collections/RabbitMQ-Messaging-Tests.postman_collection.json`
2. Environment: `fda/postman-collections/RabbitMQ-Testing-Local.postman_environment.json`

### Test Flow
```
1. Authentication > Login as Operator
2. Order Events > Create Order
3. Check RabbitMQ UI → Exchanges → food-delivery-exchange
4. Order Events > Accept Order
5. Order Events > Update Status...
6. Verify events in RabbitMQ queues
```

## 🔍 Monitoring Commands

### Docker Compose
```powershell
# View logs
docker logs order-api -f | Select-String "RabbitMQ|Event"
docker logs cart-api -f | Select-String "RabbitMQ|Event"
docker logs rabbitmq -f

# Check status
docker ps | Select-String "rabbitmq|order|cart"

# Restart services
docker restart order-api cart-api rabbitmq
```

### Kubernetes
```powershell
# View logs
kubectl logs -n capstone-services -l app=order-api --tail=100 -f
kubectl logs -n capstone-services -l app=cart-api --tail=100 -f
kubectl logs -n capstone-services -l app=rabbitmq --tail=100 -f

# Check status
kubectl get pods -n capstone-services
kubectl get svc -n capstone-services

# Restart services
kubectl rollout restart -n capstone-services deployment/order-api
kubectl rollout restart -n capstone-services deployment/cart-api
kubectl delete pod -n capstone-services -l app=rabbitmq
```

## 🛠️ Common Operations

### Check RabbitMQ Queue Status
```powershell
# Via Management API
curl -u admin:AdminPass2024 http://localhost:15672/api/queues
```

### Verify Service Configuration
```powershell
# Docker
docker exec order-api printenv | Select-String "RABBITMQ"

# Kubernetes
kubectl exec -n capstone-services deployment/order-api -- printenv | Select-String "RABBITMQ"
```

### Build Services Locally
```powershell
cd C:\dotnet\capstone\fda\src\services\order
dotnet build order.sln

cd C:\dotnet\capstone\fda\src\services\cart
dotnet build Cart.sln
```

## 🐛 Troubleshooting

### Issue: Can't connect to RabbitMQ
```powershell
# Check if RabbitMQ is running
docker ps | Select-String rabbitmq
kubectl get pods -n capstone-services | Select-String rabbitmq

# Check RabbitMQ logs
docker logs rabbitmq
kubectl logs -n capstone-services -l app=rabbitmq
```

### Issue: Events not appearing
1. Check RabbitMQ Management UI → Exchanges → food-delivery-exchange
2. Verify exchange exists and has bindings
3. Check service logs for "Event published" messages
4. Verify RABBITMQ_* environment variables are set

### Issue: No consumers on queue
1. Check if Cart service is running
2. Verify CartMessageConsumerService started:
   ```
   Hosted service: CartMessageConsumerService started
   ```
3. Check RabbitMQ UI → Queues → cart-service-queue → Consumers tab

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| **RABBITMQ-IMPLEMENTATION.md** | Complete technical implementation guide |
| **RABBITMQ-TESTING-GUIDE.md** | Step-by-step testing instructions |
| **RABBITMQ-IMPLEMENTATION-SUMMARY.md** | Project overview and requirements |
| **RabbitMQ-Messaging-Tests.postman_collection.json** | Postman test collection |

## 🎓 Key Concepts

### Exchange Type: Topic
- Flexible routing with pattern matching
- Routing keys use dot notation: `order.created`, `payment.completed`
- Queues bind with wildcards: `order.*`, `payment.*`

### Message Persistence
- Messages marked as persistent (DeliveryMode = 2)
- Survive RabbitMQ restart (Docker volume/K8s emptyDir)
- ⚠️ Use persistent volumes in production

### Acknowledgment Mode
- Auto-ack enabled by default
- Messages removed after delivery
- Future: Manual ack with retry logic

## 🚨 Production Considerations

### High Availability
- [ ] Deploy RabbitMQ cluster (3+ nodes)
- [ ] Use persistent volumes for message durability
- [ ] Configure queue mirroring/quorum queues
- [ ] Add load balancer for RabbitMQ connections

### Security
- [ ] Move credentials to Kubernetes Secrets
- [ ] Enable TLS/SSL for connections
- [ ] Use separate users per service
- [ ] Implement message encryption for sensitive data

### Monitoring
- [ ] Set up Prometheus metrics
- [ ] Create Grafana dashboards
- [ ] Configure alerts for queue depth
- [ ] Monitor consumer lag

### Performance
- [ ] Tune prefetch count
- [ ] Implement connection pooling
- [ ] Add message batching
- [ ] Optimize queue settings

## ⚡ Quick Commands Cheat Sheet

```powershell
# Start everything (Docker)
cd C:\dotnet\capstone\fda\devops\docker && docker-compose -f docker-compose-working.yml up -d

# Stop everything (Docker)
docker-compose -f docker-compose-working.yml down

# Start everything (Kubernetes)
kubectl apply -f C:\dotnet\capstone\fda\devops\kubernetes\local/

# Stop everything (Kubernetes)
kubectl delete namespace capstone-services

# View RabbitMQ UI
start http://localhost:15672

# Run Postman tests
# (Import collection first, then run via Postman UI)

# Check logs for events
docker logs order-api -f | Select-String "Event"

# Restart a service (Docker)
docker restart order-api

# Restart a service (Kubernetes)
kubectl rollout restart -n capstone-services deployment/order-api
```

---

**Branch:** feature/rabbitmq-messaging  
**Last Updated:** January 13, 2025  
**For detailed information, see:** RABBITMQ-IMPLEMENTATION-SUMMARY.md
