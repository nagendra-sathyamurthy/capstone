using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for cancelling an order with reason
    /// </summary>
    public class CancelOrderCommand : ICommand<Order.Models.Order>
    {
        private readonly string _orderId;
        private readonly string _cancellationReason;
        private readonly string _cancelledBy;
        private readonly OrderRepository _orderRepository;

        public CancelOrderCommand(
            string orderId,
            string cancellationReason,
            string cancelledBy,
            OrderRepository orderRepository)
        {
            _orderId = orderId;
            _cancellationReason = cancellationReason;
            _cancelledBy = cancelledBy;
            _orderRepository = orderRepository;
        }

        public async Task<Order.Models.Order> ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_orderId))
            {
                throw new ArgumentException("Order ID cannot be null or empty", nameof(_orderId));
            }

            if (string.IsNullOrEmpty(_cancellationReason))
            {
                throw new ArgumentException("Cancellation reason is required", nameof(_cancellationReason));
            }

            if (string.IsNullOrEmpty(_cancelledBy))
            {
                throw new ArgumentException("CancelledBy information is required", nameof(_cancelledBy));
            }

            // Get existing order
            var order = await _orderRepository.GetByIdAsync(_orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {_orderId} not found");
            }

            // Validate order can be cancelled - cannot cancel if already delivered or cancelled
            var nonCancellableStatuses = new[] 
            { 
                OrderStatus.Delivered, 
                OrderStatus.Cancelled 
            };

            if (nonCancellableStatuses.Contains(order.Status))
            {
                throw new InvalidOperationException(
                    $"Cannot cancel order with status {order.Status}");
            }

            // Orders that are PickedUp or OutForDelivery may have restrictions
            // (in production, you might want to require special authorization)
            if (order.Status == OrderStatus.PickedUp || order.Status == OrderStatus.OutForDelivery)
            {
                // Log warning or require manager approval in real implementation
                Console.WriteLine($"Warning: Cancelling order {_orderId} that is already in transit");
            }

            // Update order status
            var previousStatus = order.Status;
            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // Add cancellation information to status history
            if (order.StatusHistory == null)
            {
                order.StatusHistory = new List<OrderStatusHistory>();
            }

            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = OrderStatus.Cancelled,
                Timestamp = order.CancelledAt.Value,
                Notes = $"Cancelled by {_cancelledBy}. Reason: {_cancellationReason}. Previous status: {previousStatus}"
            });

            // Update in database
            await _orderRepository.UpdateAsync(order);

            return order;
        }
    }
}
