using catalog.Models;
using catalog.DataAccess;
using MongoDB.Driver;

namespace catalog.API
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly MenuItemRepository _menuItemRepository;

        public OrderService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CatalogDb");
            _orderRepository = new OrderRepository(database);
            _menuItemRepository = new MenuItemRepository(database);
        }

        // Order Management
        public Task<string> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Insert(order);
            return Task.FromResult(order.Id ?? string.Empty);
        }

        public Task<List<Order>> GetOrdersByRestaurantAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetByRestaurantId(restaurantId).ToList());
        }

        public Task<List<Order>> GetPendingOrdersAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetPendingOrders(restaurantId).ToList());
        }

        public Task<List<Order>> GetReadyForPickupOrdersAsync(string restaurantId)
        {
            return Task.FromResult(_orderRepository.GetReadyForPickup(restaurantId).ToList());
        }

        public Task<Order?> GetOrderByIdAsync(string id)
        {
            return Task.FromResult(_orderRepository.GetById(id));
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

            order.DeliveryAgentId = deliveryAgentId;
            order.HandoverOTP = otp;
            order.HandoverOTPGeneratedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            
            _orderRepository.Update(orderId, order);
            
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

        // Inventory Management
        public Task<List<MenuItem>> GetKitchenInventoryAsync(string restaurantId)
        {
            var items = _menuItemRepository.GetByRestaurantId(restaurantId);
            return Task.FromResult(items.ToList());
        }

        public Task UpdateMenuItemAvailabilityAsync(string menuItemId, InventoryUpdateRequest request)
        {
            var item = _menuItemRepository.GetById(menuItemId);
            if (item == null)
                throw new Exception("Menu item not found");

            var filter = MongoDB.Driver.Builders<MenuItem>.Filter.Eq(i => i.Id, menuItemId);
            var updateBuilder = MongoDB.Driver.Builders<MenuItem>.Update;
            var updates = new List<MongoDB.Driver.UpdateDefinition<MenuItem>>();

            if (request.QuantityAvailable.HasValue)
                updates.Add(updateBuilder.Set(i => i.QuantityAvailable, request.QuantityAvailable.Value));

            if (request.IsAvailable.HasValue)
                updates.Add(updateBuilder.Set(i => i.IsAvailable, request.IsAvailable.Value));

            if (!string.IsNullOrEmpty(request.AvailableFromTime))
                updates.Add(updateBuilder.Set(i => i.AvailableFromTime, request.AvailableFromTime));

            if (!string.IsNullOrEmpty(request.AvailableToTime))
                updates.Add(updateBuilder.Set(i => i.AvailableToTime, request.AvailableToTime));

            updates.Add(updateBuilder.Set(i => i.UpdatedAt, DateTime.UtcNow));

            if (updates.Any())
            {
                // Update the existing menu item
                if (request.QuantityAvailable.HasValue)
                    item.QuantityAvailable = request.QuantityAvailable;
                if (request.IsAvailable.HasValue)
                    item.IsAvailable = request.IsAvailable.Value;
                if (!string.IsNullOrEmpty(request.AvailableFromTime))
                    item.AvailableFromTime = request.AvailableFromTime;
                if (!string.IsNullOrEmpty(request.AvailableToTime))
                    item.AvailableToTime = request.AvailableToTime;
                
                item.UpdatedAt = DateTime.UtcNow;
                _menuItemRepository.Update(menuItemId, item);
            }

            return Task.CompletedTask;
        }

        public Task<List<MenuItem>> GetLowStockItemsAsync(string restaurantId)
        {
            var items = _menuItemRepository.GetByRestaurantId(restaurantId)
                .Where(i => i.QuantityAvailable.HasValue && 
                           i.MinimumQuantity.HasValue && 
                           i.QuantityAvailable.Value <= i.MinimumQuantity.Value)
                .ToList();
            
            return Task.FromResult(items);
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
