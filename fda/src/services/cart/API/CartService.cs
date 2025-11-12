using Cart.Models;
using Cart.DataAccess;
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
            var cart = new Cart.Models.Cart { Id = Guid.NewGuid().ToString(), UserId = userId };
            await _cartRepository.AddAsync(cart);
            return cart;
        }

        public async Task<Cart.Models.Cart> AddItemAsync(string cartId, Cart.Models.CartItem item)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");
            var existing = cart.Items.FirstOrDefault(i => i.ItemId == item.ItemId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }
            await _cartRepository.UpdateAsync(cartId, cart);
            return cart;
        }

        public async Task<Cart.Models.Cart> RemoveItemAsync(string cartId, string itemId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");
            cart.Items = cart.Items.Where(i => i.ItemId != itemId).ToList();
            await _cartRepository.UpdateAsync(cartId, cart);
            return cart;
        }
    }
}
