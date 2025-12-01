# RabbitMQ Message-Oriented Middleware Implementation

## Overview

This document describes the implementation of RabbitMQ-based asynchronous messaging across the Food Delivery Application microservices. This enables loose coupling, better scalability, and improved reliability through event-driven communication.

## Architecture

### Message Flow

```
┌──────────────────────────────────────────────────────────────┐
│              RabbitMQ Message Broker                         │
│          (Exchange: food-delivery-exchange)                  │
└──────────────────────────────────────────────────────────────┘
         │                   │                   │
         │                   │                   │
┌────────▼────────┐  ┌───────▼────────┐  ┌──────▼──────┐
│  Order Queue    │  │ Payment Queue  │  │ Cart Queue  │
└────────┬────────┘  └───────┬────────┘  └──────┬──────┘
         │                   │                   │
         │                   │                   │
         ▲                   ▲                   ▼
         │                   │                   │
┌────────┴────────┐  ┌───────┴────────┐  ┌──────┴──────────┐
│  Order Service  │  │ Payment Service│  │  Cart Service   │
│                 │  │                │  │                 │
│ - Publishes:    │  │ - Publishes:   │  │ - Subscribes:   │
│  • OrderCreated │  │  • PaymentCompleted │ • PaymentCompleted│
│  • OrderAccepted│  │  • PaymentFailed│  │ - Clears cart   │
│  • OrderDeclined│  │                │  │   after payment │
│  • OrderStatusChanged │             │  │                 │
│  • OrderReady   │  │                │  │                 │
│  • OrderDelivered│ │                │  │                 │
└─────────────────┘  └────────────────┘  └─────────────────┘
         │                   │
         │                   │
         └────► Publish ─────┘
                Events
```

## Messaging Library

### Purpose
Centralized messaging infrastructure shared across all microservices.

### Components

#### 1. **Events** (`Events/`)
Domain events that represent business actions:

- **OrderCreatedEvent**: Published when a new order is placed
- **OrderAcceptedEvent**: Published when restaurant accepts an order
- **OrderDeclinedEvent**: Published when restaurant declines an order  
- **OrderStatusChangedEvent**: Published when order status changes
- **OrderReadyForPickupEvent**: Published when order is ready
- **OrderDeliveredEvent**: Published when order is delivered
- **PaymentInitiatedEvent**: Published when payment process starts
- **PaymentCompletedEvent**: Published when payment succeeds
- **PaymentFailedEvent**: Published when payment fails
- **CartClearEvent**: Published to clear customer's cart

#### 2. **Messaging Infrastructure**

**IMessagePublisher**
```csharp
public interface IMessagePublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string? routingKey = null) where TEvent : class;
}
```

**IMessageConsumer**
```csharp
public interface IMessageConsumer
{
    void StartConsuming<TEvent>(string queueName, Func<TEvent, Task> handler) where TEvent : class;
    void StopConsuming();
}
```

**RabbitMQPublisher**
- Publishes events to RabbitMQ exchange
- Uses Topic Exchange for flexible routing
- Automatic reconnection on failure
- Persistent messages for reliability

**RabbitMQConsumer**
- Consumes messages from queues
- Automatic acknowledgment/negative acknowledgment
- Prefetch count = 1 for fair distribution
- Async message processing

**RabbitMQConfiguration**
- Centralized configuration settings
- Queue name constants
- Connection parameters

## Service Implementation

### Order Service

**Role**: Event Publisher

**Published Events**:
1. **OrderCreatedEvent** - When CreateOrder() is called
2. **OrderAcceptedEvent** - When AcceptOrder() is called  
3. **OrderDeclinedEvent** - When DeclineOrder() is called
4. **OrderStatusChangedEvent** - When UpdateOrderStatus() is called
5. **OrderReadyForPickupEvent** - When status changes to ReadyForPickup
6. **OrderDeliveredEvent** - When status changes to Delivered

**Configuration**:
```yaml
environment:
  - RABBITMQ_HOST=rabbitmq
  - RABBITMQ_PORT=5672
  - RABBITMQ_USER=admin
  - RABBITMQ_PASSWORD=AdminPass2024
  - RABBITMQ_EXCHANGE=food-delivery-exchange
```

**Example Event Publishing**:
```csharp
public async Task<string> CreateOrderAsync(Models.Order order)
{
    order.CreatedAt = DateTime.UtcNow;
    order.UpdatedAt = DateTime.UtcNow;
    _orderRepository.Insert(order);
    
    // Publish OrderCreatedEvent
    var orderCreatedEvent = new OrderCreatedEvent
    {
        OrderId = order.Id ?? string.Empty,
        CustomerId = order.CustomerId,
        RestaurantId = order.RestaurantId,
        RestaurantName = order.RestaurantName ?? string.Empty,
        TotalAmount = order.TotalAmount,
        Items = order.Items.Select(i => new OrderItemDto { ... }).ToList()
    };
    
    await _messagePublisher.PublishAsync(orderCreatedEvent);
    
    return order.Id ?? string.Empty;
}
```

### Cart Service

**Role**: Event Consumer

**Subscribed Events**:
1. **PaymentCompletedEvent** - Clears cart after successful payment

**Implementation**:
- Background service `CartMessageConsumerService` runs continuously
- Listens on `cart-service-queue`
- Automatically clears customer's cart when payment completes

**Configuration**:
```yaml
environment:
  - RABBITMQ_HOST=rabbitmq
  - RABBITMQ_PORT=5672
  - RABBITMQ_USER=admin
  - RABBITMQ_PASSWORD=AdminPass2024
  - RABBITMQ_EXCHANGE=food-delivery-exchange
```

**Message Handler**:
```csharp
private async Task HandlePaymentCompletedEvent(PaymentCompletedEvent @event)
{
    _logger.LogInformation($"Received PaymentCompletedEvent for Order: {@event.OrderId}");
    
    using var scope = _serviceProvider.CreateScope();
    var cartService = scope.ServiceProvider.GetRequiredService<CartService>();
    
    await cartService.ClearCartByUserIdAsync(@event.CustomerId);
    
    _logger.LogInformation($"Successfully cleared cart for customer: {@event.CustomerId}");
}
```

### Payment Service (Future Implementation)

**Role**: Event Publisher & Consumer

**To Subscribe**:
- OrderCreatedEvent - Initiate payment process

**To Publish**:
- PaymentInitiatedEvent - When payment starts
- PaymentCompletedEvent - When payment succeeds
- PaymentFailedEvent - When payment fails

### CRM Service (Future Implementation)

**Role**: Event Consumer

**To Subscribe**:
- OrderCreatedEvent - Send order confirmation to customer
- OrderAcceptedEvent - Notify customer of acceptance
- OrderDeclinedEvent - Notify customer of decline
- OrderReadyForPickupEvent - Notify delivery agent
- OrderDeliveredEvent - Send delivery confirmation
- PaymentCompletedEvent - Send payment receipt

## RabbitMQ Container Configuration

### Docker Compose

```yaml
services:
  rabbitmq:
    image: rabbitmq:3.13-management
    container_name: capstone-rabbitmq
    ports:
      - "5672:5672"   # AMQP port
      - "15672:15672" # Management UI
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=AdminPass2024
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - capstone-network
    restart: unless-stopped
    healthcheck:
      test: rabbitmq-diagnostics -q ping
      interval: 10s
      timeout: 5s
      retries: 5
```

### Access Management UI

**URL**: http://localhost:15672  
**Username**: admin  
**Password**: AdminPass2024

## Event Flow Examples

### Example 1: Order Placement to Cart Clearing

```
1. Customer places order
   └─> Order Service: CreateOrder()
       └─> Publishes: OrderCreatedEvent

2. Customer makes payment
   └─> Payment Service: ProcessPayment()
       └─> Publishes: PaymentCompletedEvent

3. Cart Service receives PaymentCompletedEvent
   └─> Cart Background Service: HandlePaymentCompletedEvent()
       └─> Clears customer's cart automatically
```

### Example 2: Order Status Updates

```
1. Restaurant operator accepts order
   └─> Order Service: AcceptOrder()
       └─> Publishes: OrderAcceptedEvent
       
2. Kitchen prepares food
   └─> Order Service: UpdateOrderStatus(Preparing)
       └─> Publishes: OrderStatusChangedEvent

3. Food is ready
   └─> Order Service: UpdateOrderStatus(ReadyForPickup)
       └─> Publishes: OrderStatusChangedEvent
       └─> Publishes: OrderReadyForPickupEvent

4. Delivery agent picks up
   └─> Order Service: UpdateOrderStatus(OutForDelivery)
       └─> Publishes: OrderStatusChangedEvent

5. Order delivered
   └─> Order Service: UpdateOrderStatus(Delivered)
       └─> Publishes: OrderStatusChangedEvent
       └─> Publishes: OrderDeliveredEvent
```

## Benefits

### 1. **Loose Coupling**
- Services don't need direct HTTP calls to each other
- Changes in one service don't break others
- Services can be developed and deployed independently

### 2. **Asynchronous Processing**
- Non-blocking operations improve performance
- Services can process events at their own pace
- Better handling of traffic spikes

### 3. **Reliability**
- Messages are persistent and survive crashes
- Automatic retry on failure
- Dead letter queues for failed messages

### 4. **Scalability**
- Multiple consumers can process messages in parallel
- Easy to add new consumers for load distribution
- Services can scale independently

### 5. **Auditability**
- All events are logged
- Easy to trace event flow
- Historical data for analytics

## Configuration

### Environment Variables

All services using RabbitMQ require these environment variables:

```bash
RABBITMQ_HOST=rabbitmq          # or localhost for local dev
RABBITMQ_PORT=5672
RABBITMQ_USER=admin
RABBITMQ_PASSWORD=AdminPass2024
RABBITMQ_EXCHANGE=food-delivery-exchange
```

### Queue Names

Defined in `RabbitMQConfiguration.Queues`:
- `order-service-queue`
- `payment-service-queue`
- `cart-service-queue`
- `crm-service-queue`
- `catalog-service-queue`

## Monitoring

### RabbitMQ Management UI

Access at http://localhost:15672

**Features**:
- View queues, exchanges, connections
- Monitor message rates
- See consumer activity
- Manually publish/consume messages for testing
- View dead letter queues

### Logs

Each service logs:
- Event publishing: "Published {EventType} for order: {OrderId}"
- Event consumption: "Received {EventType} for order: {OrderId}"
- Processing results: "Successfully processed {EventType}"
- Errors: "Error processing {EventType}: {ErrorMessage}"

## Testing

### Manual Testing

1. **Start RabbitMQ**:
   ```bash
   docker-compose up rabbitmq
   ```

2. **Access Management UI**:
   - Navigate to http://localhost:15672
   - Login with admin/AdminPass2024

3. **Create Test Order**:
   ```bash
   curl -X POST http://localhost:8085/api/order \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     -d '{
       "customerId": "test-customer",
       "restaurantId": "test-restaurant",
       "items": [...],
       "totalAmount": 100.00
     }'
   ```

4. **Verify in RabbitMQ UI**:
   - Check "Queues" tab
   - Should see message in respective queue
   - Click on queue to see message details

5. **Check Consumer Logs**:
   ```bash
   docker logs capstone-cart
   # Look for: "Received PaymentCompletedEvent..."
   ```

### Automated Testing

(Future Implementation)
- Unit tests for event publishers
- Integration tests for event consumers
- End-to-end tests for complete flows

## Troubleshooting

### Problem: Messages not being published

**Check**:
1. RabbitMQ container is running: `docker ps | grep rabbitmq`
2. Service has correct RabbitMQ connection settings
3. Check service logs for connection errors
4. Verify exchange exists in RabbitMQ UI

### Problem: Messages not being consumed

**Check**:
1. Background service is registered: `builder.Services.AddHostedService<...>()`
2. Queue is bound to exchange correctly
3. Check consumer logs for errors
4. Verify message format matches event class

### Problem: Messages stuck in queue

**Check**:
1. Consumer is running
2. No unhandled exceptions in consumer
3. Check dead letter queue for failed messages
4. Manually requeue messages from UI

## Future Enhancements

### 1. **Payment Service Integration**
- Subscribe to OrderCreatedEvent
- Publish Payment events

### 2. **CRM Service Integration**
- Subscribe to all order events
- Send notifications (SMS, email, push)

### 3. **Retry Policies**
- Implement exponential backoff
- Configure max retry attempts
- Dead letter queue handling

### 4. **Event Sourcing**
- Store all events for audit trail
- Rebuild state from events
- Temporal queries

### 5. **SAGA Pattern**
- Implement distributed transactions
- Compensating transactions on failure
- Order orchestration

### 6. **Monitoring & Alerting**
- Integrate with Application Insights
- Set up alerts for queue depth
- Track message processing times

## References

- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Event-Driven Architecture](https://martinfowler.com/articles/201701-event-driven.html)
- [SAGA Pattern](https://microservices.io/patterns/data/saga.html)
- [RabbitMQ .NET Client](https://www.rabbitmq.com/dotnet-api-guide.html)
