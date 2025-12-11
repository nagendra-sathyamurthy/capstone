using Cart.Models;
using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for updating the quantity of an item in the cart
    /// </summary>
    public class UpdateItemQuantityCommand : ICommand<Cart.Models.Cart>
    {
        private readonly string _cartId;
        private readonly string _itemId;
        private readonly int _newQuantity;
        private readonly CartRepository _cartRepository;

        public UpdateItemQuantityCommand(
            string cartId,
            string itemId,
            int newQuantity,
            CartRepository cartRepository)
        {
            _cartId = cartId;
            _itemId = itemId;
            _newQuantity = newQuantity;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart> ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_cartId))
            {
                throw new ArgumentException("Cart ID cannot be null or empty", nameof(_cartId));
            }

            if (string.IsNullOrEmpty(_itemId))
            {
                throw new ArgumentException("Item ID cannot be null or empty", nameof(_itemId));
            }

            if (_newQuantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative. Use RemoveItemFromCartCommand to remove items", nameof(_newQuantity));
            }

            // Get cart
            var cart = await _cartRepository.GetByIdAsync(_cartId);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {_cartId} not found");
            }

            // Find item in cart
            var existingItem = cart.Items?.FirstOrDefault(i => i.ItemId == _itemId);
            if (existingItem == null)
            {
                throw new InvalidOperationException($"Item with ID {_itemId} not found in cart");
            }

            // If new quantity is 0, remove the item
            if (_newQuantity == 0)
            {
                cart.Items = cart.Items.Where(i => i.ItemId != _itemId).ToList();
            }
            else
            {
                // Update the quantity
                existingItem.Quantity = _newQuantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            return cart;
        }
    }
}
