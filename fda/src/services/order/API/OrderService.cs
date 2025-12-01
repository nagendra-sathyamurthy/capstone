using Order.Models;
using Order.DataAccess;
using MongoDB.Driver;
using Shared.Messaging;
using Shared.Messaging.Events;

namespace Order.API
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly IMessagePublisher _messagePublisher;

        public OrderService(IMongoClient mongoClient, IConfiguration configuration, IMessagePublisher messagePublisher)
        {
            var databaseName = configuration["MONGO_DATABASE"] ?? "OrderDb";
            var database = mongoClient.GetDatabase(databaseName);
            _orderRepository = new OrderRepository(database);
            _messagePublisher = messagePublisher;
        }

        // Order Management
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
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    SpecialInstructions = i.SpecialInstructions
                }).ToList()
            };
            
            await _messagePublisher.PublishAsync(orderCreatedEvent);
            Console.WriteLine($"Published OrderCreatedEvent for order: {order.Id}");
            
            return order.Id ?? string.Empty;
        }

        public Task<List<Models.Order>> GetOrdersByRestaurantAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetByRestaurantId(restaurantId).ToList());
        }

        public Task<List<Models.Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return Task.FromResult(_orderRepository.GetByCustomerId(customerId).ToList());
        }

        public Task<List<Models.Order>> GetPendingOrdersAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetPendingOrders(restaurantId).ToList());
        }

        public Task<List<Models.Order>> GetReadyForPickupOrdersAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetReadyForPickup(restaurantId).ToList());
        }

        public Task<Models.Order?> GetOrderByIdAsync(string id)
        {
            var order = _orderRepository.GetById(id);
            return Task.FromResult(order);
        }

        public async Task AcceptOrderAsync(string orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found");
                
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Accepted);
            
            // Publish OrderAcceptedEvent
            var orderAcceptedEvent = new OrderAcceptedEvent
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                AcceptedBy = "Restaurant Staff", // TODO: Get from claims
                EstimatedPreparationTime = DateTime.UtcNow.AddMinutes(30)
            };
            
            await _messagePublisher.PublishAsync(orderAcceptedEvent);
            Console.WriteLine($"Published OrderAcceptedEvent for order: {orderId}");
        }

        public async Task DeclineOrderAsync(string orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found");
                
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Declined);
            
            // Publish OrderDeclinedEvent
            var orderDeclinedEvent = new OrderDeclinedEvent
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                DeclinedBy = "Restaurant Staff", // TODO: Get from claims
                Reason = "Restaurant is busy"
            };
            
            await _messagePublisher.PublishAsync(orderDeclinedEvent);
            Console.WriteLine($"Published OrderDeclinedEvent for order: {orderId}");
        }

        public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found");
                
            var oldStatus = order.Status;
            _orderRepository.UpdateOrderStatus(orderId, status);
            
            // Publish OrderStatusChangedEvent
            var statusChangedEvent = new OrderStatusChangedEvent
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                RestaurantId = order.RestaurantId,
                OldStatus = oldStatus.ToString(),
                NewStatus = status.ToString(),
                UpdatedBy = "System"
            };
            
            await _messagePublisher.PublishAsync(statusChangedEvent);
            Console.WriteLine($"Published OrderStatusChangedEvent for order: {orderId} - {oldStatus} -> {status}");
            
            // If status is ReadyForPickup, publish specific event
            if (status == OrderStatus.ReadyForPickup)
            {
                var readyEvent = new OrderReadyForPickupEvent
                {
                    OrderId = orderId,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                    RestaurantName = order.RestaurantName ?? string.Empty,
                    PackagedBy = order.Packaging?.PackagedBy
                };
                
                await _messagePublisher.PublishAsync(readyEvent);
                Console.WriteLine($"Published OrderReadyForPickupEvent for order: {orderId}");
            }
            
            // If status is Delivered, publish specific event
            if (status == OrderStatus.Delivered)
            {
                var deliveredEvent = new OrderDeliveredEvent
                {
                    OrderId = orderId,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                    DeliveryAgentId = order.DeliveryAgentId,
                    DeliveryTime = DateTime.UtcNow
                };
                
                await _messagePublisher.PublishAsync(deliveredEvent);
                Console.WriteLine($"Published OrderDeliveredEvent for order: {orderId}");
            }
        }

        // Packaging Management
        public Task UpdatePackagingAsync(string orderId, PackagingDetails packaging)
        {
            packaging.PackagedAt = DateTime.UtcNow;
            _orderRepository.UpdatePackaging(orderId, packaging);
            return Task.CompletedTask;
        }

        // Handover Management
        public Task<string> GenerateHandoverOTPAsync(string orderId, string deliveryAgentId)
        {
            var otp = GenerateOTP();
            
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found");

            var filter = MongoDB.Driver.Builders<Models.Order>.Filter.Eq(o => o.Id, orderId);
            var update = MongoDB.Driver.Builders<Models.Order>.Update
                .Set(o => o.DeliveryAgentId, deliveryAgentId)
                .Set(o => o.HandoverOTP, otp)
                .Set(o => o.HandoverOTPGeneratedAt, DateTime.UtcNow)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _orderRepository._collection.UpdateOne(filter, update);
            
            return Task.FromResult(otp);
        }

        public Task<bool> VerifyHandoverOTPAsync(HandoverRequest request)
        {
            var order = _orderRepository.GetById(request.OrderId);
            
            if (order == null)
                return Task.FromResult(false);

            if (order.HandoverOTP != request.OTP)
                return Task.FromResult(false);

            if (order.DeliveryAgentId != request.DeliveryAgentId)
                return Task.FromResult(false);

            // Check OTP expiry (5 minutes)
            if (order.HandoverOTPGeneratedAt.HasValue)
            {
                var expiryTime = order.HandoverOTPGeneratedAt.Value.AddMinutes(5);
                if (DateTime.UtcNow > expiryTime)
                    return Task.FromResult(false);
            }

            // Complete handover
            _orderRepository.CompleteHandover(request.OrderId, request.OperatorId);
            
            return Task.FromResult(true);
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
