using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for assigning a delivery worker to an order
    /// </summary>
    public class AssignDeliveryWorkerCommand : ICommand<Order.Models.Order>
    {
        private readonly string _orderId;
        private readonly string _deliveryWorkerId;
        private readonly OrderRepository _orderRepository;

        public AssignDeliveryWorkerCommand(
            string orderId,
            string deliveryWorkerId,
            OrderRepository orderRepository)
        {
            _orderId = orderId;
            _deliveryWorkerId = deliveryWorkerId;
            _orderRepository = orderRepository;
        }

        public async Task<Order.Models.Order> ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_orderId))
            {
                throw new ArgumentException("Order ID cannot be null or empty", nameof(_orderId));
            }

            if (string.IsNullOrEmpty(_deliveryWorkerId))
            {
                throw new ArgumentException("Delivery Worker ID cannot be null or empty", nameof(_deliveryWorkerId));
            }

            // Get existing order
            var order = await _orderRepository.GetByIdAsync(_orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {_orderId} not found");
            }

            // Validate order status - can only assign delivery worker to certain statuses
            var validStatuses = new[] 
            { 
                OrderStatus.Ready, 
                OrderStatus.PickedUp 
            };

            if (!validStatuses.Contains(order.Status))
            {
                throw new InvalidOperationException(
                    $"Cannot assign delivery worker to order with status {order.Status}. " +
                    $"Order must be in Ready or PickedUp status");
            }

            // Check if delivery worker is already assigned
            if (!string.IsNullOrEmpty(order.DeliveryWorkerId))
            {
                throw new InvalidOperationException(
                    $"Order already has delivery worker {order.DeliveryWorkerId} assigned. " +
                    $"Cannot reassign to {_deliveryWorkerId}");
            }

            // Assign delivery worker
            order.DeliveryWorkerId = _deliveryWorkerId;
            order.UpdatedAt = DateTime.UtcNow;

            // If order is Ready, automatically move it to PickedUp status
            if (order.Status == OrderStatus.Ready)
            {
                order.Status = OrderStatus.PickedUp;
                order.PickedUpAt = DateTime.UtcNow;

                // Add to status history
                if (order.StatusHistory == null)
                {
                    order.StatusHistory = new List<OrderStatusHistory>();
                }

                order.StatusHistory.Add(new OrderStatusHistory
                {
                    Status = OrderStatus.PickedUp,
                    Timestamp = order.PickedUpAt.Value,
                    Notes = $"Delivery worker {_deliveryWorkerId} assigned and order picked up"
                });
            }

            // Update in database
            await _orderRepository.UpdateAsync(order);

            return order;
        }
    }
}
