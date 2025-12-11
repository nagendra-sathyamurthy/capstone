using Cart.Models;
using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for clearing all items from a cart
    /// </summary>
    public class ClearCartCommand : ICommand<Cart.Models.Cart>
    {
        private readonly string _cartId;
        private readonly CartRepository _cartRepository;

        public ClearCartCommand(string cartId, CartRepository cartRepository)
        {
            _cartId = cartId;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_cartId))
            {
                throw new ArgumentException("Cart ID cannot be null or empty", nameof(_cartId));
            }

            var cart = await _cartRepository.GetByIdAsync(_cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {_cartId} not found");
            }

            // Clear all items
            cart.Items = new List<CartItem>();
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            return cart;
        }
    }
}
