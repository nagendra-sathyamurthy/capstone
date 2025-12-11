using Cart.DataAccess;

namespace Cart.API.Commands
{
    /// <summary>
    /// Command for retrieving a cart by ID
    /// </summary>
    public class GetCartByIdCommand : ICommand<Cart.Models.Cart?>
    {
        private readonly string _cartId;
        private readonly CartRepository _cartRepository;

        public GetCartByIdCommand(string cartId, CartRepository cartRepository)
        {
            _cartId = cartId;
            _cartRepository = cartRepository;
        }

        public async Task<Cart.Models.Cart?> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_cartId))
            {
                throw new ArgumentException("Cart ID cannot be null or empty", nameof(_cartId));
            }

            return await _cartRepository.GetByIdAsync(_cartId);
        }
    }
}
