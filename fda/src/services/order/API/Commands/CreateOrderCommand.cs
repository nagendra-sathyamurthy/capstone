using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for creating a new order
    /// </summary>
    public class CreateOrderCommand : ICommand<Order.Models.Order>
    {
        private readonly Order.Models.Order _order;
        private readonly OrderRepository _orderRepository;

        public CreateOrderCommand(Order.Models.Order order, OrderRepository orderRepository)
        {
            _order = order;
            _orderRepository = orderRepository;
        }

        public async Task<Order.Models.Order> ExecuteAsync()
        {
            if (_order == null)
            {
                throw new ArgumentNullException(nameof(_order), "Order cannot be null");
            }

            // Validate order
            ValidateOrder();

            // Set initial order properties
            _order.CreatedAt = DateTime.UtcNow;
            _order.UpdatedAt = DateTime.UtcNow;
            _order.Status = OrderStatus.Pending;
            _order.OrderNumber = GenerateOrderNumber();

            // Calculate total if not provided
            if (_order.TotalAmount == 0 && _order.Items != null && _order.Items.Any())
            {
                _order.TotalAmount = _order.Items.Sum(item => item.Price * item.Quantity);
            }

            await _orderRepository.AddAsync(_order);
            return _order;
        }

        private void ValidateOrder()
        {
            if (string.IsNullOrEmpty(_order.CustomerId))
            {
                throw new ArgumentException("Customer ID is required");
            }

            if (string.IsNullOrEmpty(_order.RestaurantId))
            {
                throw new ArgumentException("Restaurant ID is required");
            }

            if (_order.Items == null || !_order.Items.Any())
            {
                throw new ArgumentException("Order must contain at least one item");
            }

            if (_order.DeliveryAddress == null || string.IsNullOrEmpty(_order.DeliveryAddress.Street))
            {
                throw new ArgumentException("Delivery address is required");
            }

            // Validate each order item
            foreach (var item in _order.Items)
            {
                if (string.IsNullOrEmpty(item.ItemId))
                {
                    throw new ArgumentException("Item ID is required for all order items");
                }

                if (item.Quantity <= 0)
                {
                    throw new ArgumentException($"Quantity for item {item.ItemName} must be greater than zero");
                }

                if (item.Price <= 0)
                {
                    throw new ArgumentException($"Price for item {item.ItemName} must be greater than zero");
                }
            }
        }

        private string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-RANDOM6
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(100000, 999999);
            return $"ORD-{datePart}-{randomPart}";
        }
    }
}
