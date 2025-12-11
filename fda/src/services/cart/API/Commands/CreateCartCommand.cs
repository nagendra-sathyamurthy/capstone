using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for creating a new cart
    /// </summary>
    public class CreateCartCommand : ICommand<Cart.Models.Cart>
    {
        private readonly string _userId;
        private readonly CartRepository _cartRepository;

        public CreateCartCommand(string userId, CartRepository cartRepository)
        {
            _userId = userId;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(_userId));
            }

            // Check if cart already exists
            var existingCart = await _cartRepository.GetByIdAsync(_userId);
            if (existingCart != null)
            {
                throw new InvalidOperationException($"Cart already exists for user {_userId}");
            }

            // Create new cart with userId as cartId
            var cart = new Cart.Models.Cart
            {
                Id = _userId,
                UserId = _userId,
                Items = new List<Cart.Models.CartItem>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            return cart;
        }
    }
}
