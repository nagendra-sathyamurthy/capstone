using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for retrieving orders by customer
    /// </summary>
    public class GetOrdersByCustomerCommand : ICommand<List<Order.Models.Order>>
    {
        private readonly string _customerId;
        private readonly OrderRepository _orderRepository;
        private readonly OrderStatus? _statusFilter;
        private readonly int? _limit;

        public GetOrdersByCustomerCommand(
            string customerId,
            OrderRepository orderRepository,
            OrderStatus? statusFilter = null,
            int? limit = null)
        {
            _customerId = customerId;
            _orderRepository = orderRepository;
            _statusFilter = statusFilter;
            _limit = limit;
        }

        public async Task<List<Order.Models.Order>> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty", nameof(_customerId));
            }

            var allOrders = await _orderRepository.GetAllAsync();

            // Filter by customer
            var customerOrders = allOrders.Where(o => o.CustomerId == _customerId);

            // Apply status filter if specified
            if (_statusFilter.HasValue)
            {
                customerOrders = customerOrders.Where(o => o.Status == _statusFilter.Value);
            }

            // Sort by creation date (most recent first)
            customerOrders = customerOrders.OrderByDescending(o => o.CreatedAt);

            // Apply limit if specified
            if (_limit.HasValue && _limit.Value > 0)
            {
                customerOrders = customerOrders.Take(_limit.Value);
            }

            return customerOrders.ToList();
        }
    }
}
