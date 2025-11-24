using Order.Models;
using Order.DataAccess;
using MongoDB.Driver;

namespace Order.API
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration["MONGO_DATABASE"] ?? "OrderDb";
            var database = mongoClient.GetDatabase(databaseName);
            _orderRepository = new OrderRepository(database);
        }

        // Order Management
        public Task<string> CreateOrderAsync(Models.Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Insert(order);
            return Task.FromResult(order.Id ?? string.Empty);
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

        public Task AcceptOrderAsync(string orderId)
        {
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Accepted);
            return Task.CompletedTask;
        }

        public Task DeclineOrderAsync(string orderId)
        {
            _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Declined);
            return Task.CompletedTask;
        }

        public Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            _orderRepository.UpdateOrderStatus(orderId, status);
            return Task.CompletedTask;
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
