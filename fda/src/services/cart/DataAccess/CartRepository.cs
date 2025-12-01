using Cart.Models;
using Cart.DataAccess;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Cart.DataAccess
{
    public class CartRepository : Cart.DataAccess.MongoRepository<Cart.Models.Cart>
    {
        public CartRepository(IMongoDatabase database)
            : base(database, "Carts")
        {
        }

        public async Task<Cart.Models.Cart?> GetByUserIdAsync(string userId)
        {
            return await _collection.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
