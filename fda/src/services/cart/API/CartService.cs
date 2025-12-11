using Cart.Models;
using Cart.DataAccess;
using Cart.API.Commands;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq;
namespace Cart.API
{
    public class CartService
    {
        private readonly CartRepository _cartRepository;

        public CartService(IMongoDatabase database)
        {
            _cartRepository = new CartRepository(database);
        }

        public async Task<Cart.Models.Cart> CreateCartAsync(string userId)
        {
            // Use CreateCartCommand for cart creation logic
            var command = new CreateCartCommand(userId, _cartRepository);
            return await command.ExecuteAsync();
        }

        public async Task<Cart.Models.Cart?> GetCartByIdAsync(string cartId)
        {
            // Use GetCartByIdCommand for cart retrieval logic
            var command = new GetCartByIdCommand(cartId, _cartRepository);
            return await command.ExecuteAsync();
        }

        public async Task<Cart.Models.Cart> AddItemAsync(string cartId, Cart.Models.CartItem item)
        {
            // Use AddItemToCartCommand for adding items logic
            var command = new AddItemToCartCommand(cartId, item, _cartRepository);
            return await command.ExecuteAsync();
        }

        public async Task<Cart.Models.Cart> RemoveItemAsync(string cartId, string itemId)
        {
            // Use RemoveItemFromCartCommand for removing items logic
            var command = new RemoveItemFromCartCommand(cartId, itemId, _cartRepository);
            return await command.ExecuteAsync();
        }

        public async Task<Cart.Models.Cart> ClearCartAsync(string cartId)
        {
            // Use ClearCartCommand to remove all items
            var command = new ClearCartCommand(cartId, _cartRepository);
            return await command.ExecuteAsync();
        }

        public async Task<Cart.Models.Cart> UpdateItemQuantityAsync(string cartId, string itemId, int newQuantity)
        {
            // Use UpdateItemQuantityCommand to update item quantity
            var command = new UpdateItemQuantityCommand(cartId, itemId, newQuantity, _cartRepository);
            return await command.ExecuteAsync();
        }
    }
}
