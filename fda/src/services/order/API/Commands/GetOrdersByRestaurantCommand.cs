using Order.Models;
using Order.DataAccess;

namespace Order.API.Commands
{
    /// <summary>
    /// Command for retrieving orders by restaurant
    /// </summary>
    public class GetOrdersByRestaurantCommand : ICommand<List<Order.Models.Order>>
    {
        private readonly string _restaurantId;
        private readonly OrderRepository _orderRepository;
        private readonly OrderStatus? _statusFilter;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateTo;
        private readonly int? _limit;

        public GetOrdersByRestaurantCommand(
            string restaurantId,
            OrderRepository orderRepository,
            OrderStatus? statusFilter = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? limit = null)
        {
            _restaurantId = restaurantId;
            _orderRepository = orderRepository;
            _statusFilter = statusFilter;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _limit = limit;
        }

        public async Task<List<Order.Models.Order>> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_restaurantId))
            {
                throw new ArgumentException("Restaurant ID cannot be null or empty", nameof(_restaurantId));
            }

            var allOrders = await _orderRepository.GetAllAsync();

            // Filter by restaurant
            var restaurantOrders = allOrders.Where(o => o.RestaurantId == _restaurantId);

            // Apply status filter if specified
            if (_statusFilter.HasValue)
            {
                restaurantOrders = restaurantOrders.Where(o => o.Status == _statusFilter.Value);
            }

            // Apply date range filter if specified
            if (_dateFrom.HasValue)
            {
                restaurantOrders = restaurantOrders.Where(o => o.CreatedAt >= _dateFrom.Value);
            }

            if (_dateTo.HasValue)
            {
                restaurantOrders = restaurantOrders.Where(o => o.CreatedAt <= _dateTo.Value);
            }

            // Sort by creation date (most recent first)
            restaurantOrders = restaurantOrders.OrderByDescending(o => o.CreatedAt);

            // Apply limit if specified
            if (_limit.HasValue && _limit.Value > 0)
            {
                restaurantOrders = restaurantOrders.Take(_limit.Value);
            }

            return restaurantOrders.ToList();
        }
    }
}
