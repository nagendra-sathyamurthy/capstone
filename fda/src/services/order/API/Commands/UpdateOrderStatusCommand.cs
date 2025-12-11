using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for updating order status
    /// </summary>
    public class UpdateOrderStatusCommand : ICommand<Order.Models.Order>
    {
        private readonly string _orderId;
        private readonly OrderStatus _newStatus;
        private readonly OrderRepository _orderRepository;
        private readonly string? _notes;

        public UpdateOrderStatusCommand(
            string orderId,
            OrderStatus newStatus,
            OrderRepository orderRepository,
            string? notes = null)
        {
            _orderId = orderId;
            _newStatus = newStatus;
            _orderRepository = orderRepository;
            _notes = notes;
        }

        public async Task<Order.Models.Order> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_orderId))
            {
                throw new ArgumentException("Order ID cannot be null or empty", nameof(_orderId));
            }

            var order = await _orderRepository.GetByIdAsync(_orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {_orderId} not found");
            }

            // Validate status transition
            ValidateStatusTransition(order.Status, _newStatus);

            // Update status
            order.Status = _newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Add status history entry
            if (order.StatusHistory == null)
            {
                order.StatusHistory = new List<OrderStatusHistory>();
            }

            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = _newStatus,
                Timestamp = DateTime.UtcNow,
                Notes = _notes
            });

            // Set specific timestamps based on status
            UpdateStatusTimestamps(order, _newStatus);

            await _orderRepository.UpdateAsync(_orderId, order);
            return order;
        }

        private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid transitions
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
                { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Preparing, OrderStatus.Cancelled } },
                { OrderStatus.Preparing, new List<OrderStatus> { OrderStatus.ReadyForPickup, OrderStatus.Cancelled } },
                { OrderStatus.ReadyForPickup, new List<OrderStatus> { OrderStatus.OutForDelivery, OrderStatus.Cancelled } },
                { OrderStatus.OutForDelivery, new List<OrderStatus> { OrderStatus.Delivered, OrderStatus.Cancelled } },
                { OrderStatus.Delivered, new List<OrderStatus>() }, // Terminal state
                { OrderStatus.Cancelled, new List<OrderStatus>() }  // Terminal state
            };

            if (!validTransitions.ContainsKey(currentStatus))
            {
                throw new InvalidOperationException($"Invalid current status: {currentStatus}");
            }

            if (!validTransitions[currentStatus].Contains(newStatus))
            {
                throw new InvalidOperationException(
                    $"Invalid status transition from {currentStatus} to {newStatus}");
            }
        }

        private void UpdateStatusTimestamps(Order.Models.Order order, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Preparing:
                    order.PreparingAt = DateTime.UtcNow;
                    break;
                case OrderStatus.ReadyForPickup:
                    order.ReadyAt = DateTime.UtcNow;
                    break;
                case OrderStatus.OutForDelivery:
                    order.PickedUpAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
