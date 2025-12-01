# RabbitMQ Messaging Testing Guide

## Overview
This guide explains how to test the RabbitMQ message-oriented middleware implementation using Postman and the RabbitMQ Management UI.

## Prerequisites

### 1. Running Services
Ensure all required services are running:

**Docker Compose:**
```powershell
cd C:\dotnet\capstone\fda\devops\docker
docker-compose -f docker-compose-working.yml up -d
```

**Kubernetes:**
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

### 2. Verify Services are Running

**Docker:**
```powershell
docker ps | Select-String -Pattern "rabbitmq|order|cart|authentication"
```

**Kubernetes:**
```powershell
kubectl get pods -n capstone-services
kubectl get svc -n capstone-services
```

### 3. Access RabbitMQ Management UI
- **URL:** http://localhost:15672
- **Username:** admin
- **Password:** AdminPass2024

For Kubernetes, use port-forward:
```powershell
kubectl port-forward -n capstone-services svc/rabbitmq-service 15672:15672
```

## Import Postman Collection

### Step 1: Import Collection
1. Open Postman
2. Click "Import" button
3. Select file: `C:\dotnet\capstone\fda\postman-collections\RabbitMQ-Messaging-Tests.postman_collection.json`
4. Click "Import"

### Step 2: Import Environment
1. Click environment dropdown (top right)
2. Click "Import"
3. Select file: `C:\dotnet\capstone\fda\postman-collections\RabbitMQ-Testing-Local.postman_environment.json`
4. Click "Import"
5. Select "RabbitMQ Testing - Local" environment

## Test Scenarios

### Scenario 1: Order Creation Event Flow

**Purpose:** Verify OrderCreatedEvent is published when a new order is created.

**Steps:**
1. **Authenticate**
   - Run: `Authentication > Login as Operator`
   - Verify: Response status 200, auth_token saved

2. **Check RabbitMQ Before Order**
   - Open RabbitMQ Management UI: http://localhost:15672
   - Go to "Queues" tab
   - Note current message counts

3. **Create Order**
   - Run: `Order Events > Create Order (Publishes OrderCreatedEvent)`
   - Verify: Response status 200, order_id saved
   - Check console output for "OrderCreatedEvent published" message

4. **Verify Event in RabbitMQ**
   - Refresh RabbitMQ Management UI "Queues" tab
   - Verify message appeared in `order-service-queue` (if consuming)
   - Go to "Exchanges" tab → "food-delivery-exchange"
   - Check "Message rates" graph for activity

5. **Check Logs**
   ```powershell
   # Docker
   docker logs order-api | Select-String -Pattern "OrderCreatedEvent"
   
   # Kubernetes
   kubectl logs -n capstone-services -l app=order-api | Select-String -Pattern "OrderCreatedEvent"
   ```

**Expected Results:**
- ✅ Order created successfully (HTTP 200)
- ✅ OrderCreatedEvent appears in RabbitMQ
- ✅ Event contains: OrderId, CustomerId, RestaurantId, TotalAmount, CreatedAt
- ✅ Routing key: `order.created`

---

### Scenario 2: Order Status Change Events

**Purpose:** Verify OrderStatusChangedEvent is published for each status transition.

**Steps:**
1. **Accept Order**
   - Run: `Order Events > Accept Order (Publishes OrderAcceptedEvent)`
   - Verify: HTTP 200 response

2. **Update to Preparing**
   - Run: `Order Events > Update Order Status to Preparing`
   - Verify: OrderStatusChangedEvent published
   - Check RabbitMQ for event with `order.status.changed` routing key

3. **Update to ReadyForPickup**
   - Run: `Order Events > Update Order Status to ReadyForPickup`
   - Verify: **TWO events published:**
     - OrderStatusChangedEvent (routing key: `order.status.changed`)
     - OrderReadyForPickupEvent (routing key: `order.ready`)

4. **Update to Delivered**
   - Run: `Order Events > Update Order Status to Delivered`
   - Verify: **TWO events published:**
     - OrderStatusChangedEvent
     - OrderDeliveredEvent (routing key: `order.delivered`)

**Expected Results:**
- ✅ Each status change publishes appropriate events
- ✅ ReadyForPickup publishes 2 events
- ✅ Delivered publishes 2 events
- ✅ Events contain correct OrderId and Status values

---

### Scenario 3: Order Decline Flow

**Purpose:** Verify OrderDeclinedEvent is published when order is declined.

**Steps:**
1. **Create New Order** (for decline test)
   - Run: `Order Events > Create Order`
   - Save the new order_id

2. **Decline Order**
   - Run: `Order Events > Decline Order`
   - Verify: HTTP 200 response

3. **Verify in RabbitMQ**
   - Check for OrderDeclinedEvent
   - Routing key: `order.declined`
   - Contains: OrderId, Reason, DeclinedAt

**Expected Results:**
- ✅ Order declined successfully
- ✅ OrderDeclinedEvent published with reason
- ✅ Event available for customer notification service

---

### Scenario 4: Payment Completion → Cart Clearing

**Purpose:** Verify Cart service consumes PaymentCompletedEvent and clears the cart.

**Prerequisites:** Cart must have items and payment service must be implemented (or simulate event).

**Steps:**
1. **Add Items to Cart** (via Cart API - not in this collection)
   ```json
   POST http://localhost:5000/api/cart
   {
     "userId": "{{user_id}}",
     "items": [...]
   }
   ```

2. **Verify Cart Has Items**
   ```json
   GET http://localhost:5000/api/cart/{{user_id}}
   ```
   - Should return cart with items

3. **Simulate Payment Completion**
   - When Payment service publishes `PaymentCompletedEvent`
   - Event should have routing key: `payment.completed`
   - Cart service should automatically consume it

4. **Verify Cart is Cleared**
   - Run: `Cart Events > Simulate PaymentCompleted`
   - Cart should now be empty

**Expected Results:**
- ✅ PaymentCompletedEvent published by Payment service
- ✅ Cart service consumes event from `cart-service-queue`
- ✅ Cart automatically cleared for the user
- ✅ No manual intervention needed

**Manual Testing (without Payment service):**
```powershell
# Publish test event via RabbitMQ Management UI
# Go to: Exchanges > food-delivery-exchange > Publish message
# Routing key: payment.completed
# Payload:
{
  "OrderId": "your-order-id",
  "PaymentId": "test-payment-123",
  "Amount": 699.97,
  "CompletedAt": "2025-01-13T12:00:00Z"
}
```

---

### Scenario 5: RabbitMQ Health Check

**Purpose:** Verify RabbitMQ infrastructure is properly configured.

**Steps:**
1. **Get Queues Info**
   - Run: `RabbitMQ Management > Get RabbitMQ Queues Info`
   - Verify: Response contains expected queues:
     - `order-service-queue`
     - `cart-service-queue`
     - `payment-service-queue` (when implemented)
     - `crm-service-queue` (when implemented)

2. **Get Exchange Bindings**
   - Run: `RabbitMQ Management > Get Exchange Bindings`
   - Verify: `food-delivery-exchange` has bindings to all queues
   - Check routing key patterns

3. **Verify in Management UI**
   - Go to: http://localhost:15672/#/exchanges/%2F/food-delivery-exchange
   - Check "Bindings" section
   - Verify all service queues are bound

**Expected Results:**
- ✅ All expected queues exist
- ✅ Exchange bindings configured correctly
- ✅ No errors in RabbitMQ logs
- ✅ All queues have active consumers

---

## Monitoring and Debugging

### RabbitMQ Management UI

**View Queues:**
1. Go to "Queues" tab
2. Click queue name to see details
3. Check:
   - Message count (ready + unacknowledged)
   - Consumer count
   - Message rates (publish/deliver/ack)

**View Messages:**
1. Click queue name
2. Scroll to "Get messages" section
3. Click "Get Message(s)" to preview without consuming
4. **WARNING:** "Ack Mode: Automatic" will remove messages!

**View Exchange:**
1. Go to "Exchanges" tab
2. Click "food-delivery-exchange"
3. Check:
   - Bindings (which queues are connected)
   - Message rates (in/out)
   - Publish message manually (for testing)

### Service Logs

**Docker Compose:**
```powershell
# Order service logs
docker logs order-api -f | Select-String -Pattern "RabbitMQ|Event"

# Cart service logs
docker logs cart-api -f | Select-String -Pattern "RabbitMQ|Event"

# RabbitMQ logs
docker logs rabbitmq -f
```

**Kubernetes:**
```powershell
# Order service logs
kubectl logs -n capstone-services -l app=order-api --tail=100 -f

# Cart service logs
kubectl logs -n capstone-services -l app=cart-api --tail=100 -f

# RabbitMQ logs
kubectl logs -n capstone-services -l app=rabbitmq --tail=100 -f
```

### Common Log Patterns to Look For

**Successful Event Publishing:**
```
Publishing event: OrderCreatedEvent for order: abc123
Event published successfully to exchange: food-delivery-exchange
```

**Successful Event Consumption:**
```
Received message from queue: cart-service-queue
Processing PaymentCompletedEvent for order: abc123
Cart cleared for user: user456
```

**Errors:**
```
Failed to publish event: [error details]
Failed to connect to RabbitMQ: [connection error]
Consumer error: [consumption error]
```

---

## Troubleshooting

### Issue: "Connection refused" when running tests

**Cause:** Services not running or not accessible.

**Solutions:**
1. Verify services are running:
   ```powershell
   docker ps  # Docker Compose
   kubectl get pods -n capstone-services  # Kubernetes
   ```

2. Check port forwarding (Kubernetes):
   ```powershell
   kubectl port-forward -n capstone-services svc/order-service 5000:80
   kubectl port-forward -n capstone-services svc/rabbitmq-service 5672:5672 15672:15672
   ```

3. Check firewall/antivirus blocking ports 5000, 5672, 15672

---

### Issue: Events not appearing in RabbitMQ

**Cause:** Exchange or queues not properly configured.

**Diagnosis:**
1. Check RabbitMQ Management UI → Exchanges
2. Verify "food-delivery-exchange" exists (type: topic)
3. Check Bindings tab for queue bindings

**Solutions:**
1. Restart RabbitMQ:
   ```powershell
   # Docker
   docker restart rabbitmq
   
   # Kubernetes
   kubectl delete pod -n capstone-services -l app=rabbitmq
   ```

2. Verify environment variables in service:
   ```powershell
   # Docker
   docker exec order-api printenv | Select-String -Pattern "RABBITMQ"
   
   # Kubernetes
   kubectl exec -n capstone-services deployment/order-api -- printenv | Select-String -Pattern "RABBITMQ"
   ```

---

### Issue: Cart not clearing after payment

**Cause:** Cart service not consuming messages or event handler error.

**Diagnosis:**
1. Check Cart service logs for errors
2. Verify CartMessageConsumerService is running:
   ```
   Hosted service: CartMessageConsumerService started
   ```
3. Check if consumer is connected in RabbitMQ UI:
   - Go to Queues → cart-service-queue
   - Verify "Consumers: 1"

**Solutions:**
1. Check Cart service has RabbitMQ env vars:
   ```powershell
   # Docker
   docker exec cart-api printenv | Select-String -Pattern "RABBITMQ"
   ```

2. Restart Cart service:
   ```powershell
   # Docker
   docker restart cart-api
   
   # Kubernetes
   kubectl rollout restart -n capstone-services deployment/cart-api
   ```

3. Check GetByUserIdAsync implementation in CartRepository

---

### Issue: RabbitMQ Management UI shows "No consumers"

**Cause:** Background services not registered or crashed.

**Solutions:**
1. Verify Program.cs has:
   ```csharp
   builder.Services.AddHostedService<CartMessageConsumerService>();
   ```

2. Check for exceptions in service startup:
   ```powershell
   docker logs cart-api | Select-String -Pattern "Exception|Error"
   ```

3. Verify RabbitMQ connection string is correct:
   - Host: `rabbitmq` (Docker) or `rabbitmq-service` (Kubernetes)
   - Port: 5672
   - Credentials: admin/AdminPass2024

---

## Performance Monitoring

### Message Throughput
Monitor in RabbitMQ Management UI → food-delivery-exchange:
- **Publish rate:** Messages/second being published
- **Deliver rate:** Messages/second being delivered to consumers
- **Ack rate:** Messages/second being acknowledged

### Queue Depth
Monitor in RabbitMQ Management UI → Queues:
- **Ready:** Messages waiting to be consumed
- **Unacknowledged:** Messages delivered but not yet acknowledged
- **Total:** Total messages in queue

**Healthy State:**
- Ready: Low (0-10) - messages consumed quickly
- Unacknowledged: Low (0-5) - messages processed quickly
- Consumer utilization: High - consumers actively processing

**Problem Indicators:**
- Ready: Growing continuously - consumers can't keep up
- Unacknowledged: High - processing taking too long
- No consumers - service crashed or not connected

---

## Advanced Testing Scenarios

### Load Testing Event Publishing

Create a Postman Runner test to create multiple orders:

1. Open Postman Collection Runner
2. Select "Order Events" folder
3. Set iterations: 10-100
4. Add delay: 100ms between requests
5. Run and monitor:
   - RabbitMQ message rates
   - Service CPU/memory usage
   - Response times

### Message Durability Test

1. Create order (publishes event)
2. Stop Cart service:
   ```powershell
   docker stop cart-api
   ```
3. Verify message persists in queue (RabbitMQ UI)
4. Restart Cart service:
   ```powershell
   docker start cart-api
   ```
5. Verify message is consumed after restart

### Dead Letter Queue Test (Future Enhancement)

When implemented, test message rejection scenarios:
1. Publish malformed event
2. Verify message moved to DLQ
3. Inspect DLQ in RabbitMQ UI
4. Reprocess or discard

---

## Test Checklists

### ✅ Pre-Deployment Checklist
- [ ] RabbitMQ container/pod running
- [ ] All service containers/pods running
- [ ] RabbitMQ Management UI accessible
- [ ] Postman collection imported
- [ ] Environment variables configured

### ✅ Functional Testing Checklist
- [ ] OrderCreatedEvent publishes successfully
- [ ] OrderAcceptedEvent publishes successfully
- [ ] OrderDeclinedEvent publishes successfully
- [ ] OrderStatusChangedEvent publishes for all transitions
- [ ] OrderReadyForPickupEvent publishes at correct status
- [ ] OrderDeliveredEvent publishes at correct status
- [ ] PaymentCompletedEvent clears cart
- [ ] All events have correct routing keys
- [ ] All events contain required properties

### ✅ Integration Testing Checklist
- [ ] Order → Payment → Cart clearing flow works end-to-end
- [ ] Multiple concurrent orders process correctly
- [ ] Service restart doesn't lose messages
- [ ] Failed message processing triggers retry
- [ ] RabbitMQ UI shows correct queue bindings
- [ ] All services show active consumers

### ✅ Monitoring Checklist
- [ ] Message publish rates are reasonable
- [ ] Queue depths remain low (<10 messages)
- [ ] No unacknowledged messages accumulating
- [ ] Service logs show successful event processing
- [ ] No connection errors in logs
- [ ] RabbitMQ memory usage stable

---

## Next Steps

1. **Implement Payment Service** - Add PaymentCompletedEvent publishing
2. **Add CRM Service Consumer** - Send customer notifications for order events
3. **Add Dead Letter Queues** - Handle failed message processing
4. **Add Message Retry Logic** - Automatic retry with exponential backoff
5. **Add Circuit Breaker** - Prevent cascading failures
6. **Add Distributed Tracing** - Track messages across services
7. **Add Metrics** - Prometheus/Grafana for monitoring

---

## Resources

- **RabbitMQ Documentation:** https://www.rabbitmq.com/documentation.html
- **RabbitMQ .NET Client Guide:** https://www.rabbitmq.com/dotnet-api-guide.html
- **Implementation Details:** See `fda/docs/RABBITMQ-IMPLEMENTATION.md`
- **Postman Learning:** https://learning.postman.com/

---

## Support

For issues or questions:
1. Check service logs first
2. Verify RabbitMQ Management UI
3. Review this guide's Troubleshooting section
4. Check RABBITMQ-IMPLEMENTATION.md for architecture details
