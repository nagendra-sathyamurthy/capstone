using Order.Models;
using Order.DataAccess;
using MongoDB.Driver;

namespace Order.API
{
    public class DeliveryService
    {
        private readonly OrderRepository _orderRepository;

        public DeliveryService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration["MONGO_DATABASE"] ?? "OrderDb";
            var database = mongoClient.GetDatabase(databaseName);
            _orderRepository = new OrderRepository(database);
        }

        /// <summary>
        /// Get orders ready for pickup that are not assigned to any delivery agent
        /// </summary>
        public Task<List<Models.Order>> GetAvailableOrdersAsync()
        {
            var orders = _orderRepository._collection
                .Find(o => o.Status == OrderStatus.ReadyForPickup && string.IsNullOrEmpty(o.DeliveryAgentId))
                .ToList();
            
            return Task.FromResult(orders);
        }

        /// <summary>
        /// Get orders assigned to a specific delivery agent
        /// </summary>
        public Task<List<Models.Order>> GetDeliveryAgentOrdersAsync(string deliveryAgentId)
        {
            var orders = _orderRepository._collection
                .Find(o => o.DeliveryAgentId == deliveryAgentId && 
                           (o.Status == OrderStatus.ReadyForPickup || 
                            o.Status == OrderStatus.OutForDelivery))
                .ToList();
            
            return Task.FromResult(orders);
        }

        /// <summary>
        /// Accept a delivery assignment
        /// </summary>
        public Task AcceptDeliveryAsync(string orderId, string deliveryAgentId)
        {
            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Models.Order>.Update
                .Set(o => o.DeliveryAgentId, deliveryAgentId)
                .Set(o => o.Status, OrderStatus.OutForDelivery)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _orderRepository._collection.UpdateOne(filter, update);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Mark order as picked up from restaurant
        /// </summary>
        public Task MarkPickedUpAsync(string orderId, string deliveryAgentId)
        {
            var order = _orderRepository.GetById(orderId);
            
            if (order == null)
                throw new Exception("Order not found");
                
            if (order.DeliveryAgentId != deliveryAgentId)
                throw new Exception("This order is not assigned to you");

            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Models.Order>.Update
                .Set(o => o.Status, OrderStatus.OutForDelivery)
                .Set(o => o.PickedUpAt, DateTime.UtcNow)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _orderRepository._collection.UpdateOne(filter, update);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Mark order as delivered to customer
        /// </summary>
        public Task MarkDeliveredAsync(string orderId, string deliveryAgentId)
        {
            var order = _orderRepository.GetById(orderId);
            
            if (order == null)
                throw new Exception("Order not found");
                
            if (order.DeliveryAgentId != deliveryAgentId)
                throw new Exception("This order is not assigned to you");

            var filter = Builders<Models.Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Models.Order>.Update
                .Set(o => o.Status, OrderStatus.Delivered)
                .Set(o => o.DeliveredAt, DateTime.UtcNow)
                .Set(o => o.UpdatedAt, DateTime.UtcNow);
            
            _orderRepository._collection.UpdateOne(filter, update);
            
            return Task.CompletedTask;
        }
    }
}
