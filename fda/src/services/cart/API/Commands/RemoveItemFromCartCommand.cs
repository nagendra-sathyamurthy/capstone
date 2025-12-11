using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for removing an item from a cart
    /// </summary>
    public class RemoveItemFromCartCommand : ICommand<Cart.Models.Cart>
    {
        private readonly string _cartId;
        private readonly string _itemId;
        private readonly CartRepository _cartRepository;

        public RemoveItemFromCartCommand(string cartId, string itemId, CartRepository cartRepository)
        {
            _cartId = cartId;
            _itemId = itemId;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_cartId))
            {
                throw new ArgumentException("Cart ID cannot be null or empty", nameof(_cartId));
            }

            if (string.IsNullOrEmpty(_itemId))
            {
                throw new ArgumentException("Item ID cannot be null or empty", nameof(_itemId));
            }

            var cart = await _cartRepository.GetByIdAsync(_cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {_cartId} not found");
            }

            var itemToRemove = cart.Items.FirstOrDefault(i => i.ItemId == _itemId);
            if (itemToRemove == null)
            {
                throw new InvalidOperationException($"Item with ID {_itemId} not found in cart");
            }

            cart.Items = cart.Items.Where(i => i.ItemId != _itemId).ToList();
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(_cartId, cart);
            return cart;
        }
    }
}
