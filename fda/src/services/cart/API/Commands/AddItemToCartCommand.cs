using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for adding an item to a cart
    /// </summary>
    public class AddItemToCartCommand : ICommand<Cart.Models.Cart>
    {
        private readonly string _cartId;
        private readonly Cart.Models.CartItem _item;
        private readonly CartRepository _cartRepository;

        public AddItemToCartCommand(string cartId, Cart.Models.CartItem item, CartRepository cartRepository)
        {
            _cartId = cartId;
            _item = item;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_cartId))
            {
                throw new ArgumentException("Cart ID cannot be null or empty", nameof(_cartId));
            }

            if (_item == null)
            {
                throw new ArgumentNullException(nameof(_item), "Cart item cannot be null");
            }

            if (_item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(_item.Quantity));
            }

            var cart = await _cartRepository.GetByIdAsync(_cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {_cartId} not found");
            }

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == _item.ItemId);
            if (existingItem != null)
            {
                // Update quantity of existing item
                existingItem.Quantity += _item.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new item to cart
                _item.AddedAt = DateTime.UtcNow;
                _item.UpdatedAt = DateTime.UtcNow;
                cart.Items.Add(_item);
            }

            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(_cartId, cart);
            return cart;
        }
    }
}
