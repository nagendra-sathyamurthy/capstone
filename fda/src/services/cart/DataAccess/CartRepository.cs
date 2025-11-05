using Cart.Models;
using Cart.DataAccess;
using MongoDB.Driver;

namespace Cart.DataAccess
{
    public class CartRepository : Cart.DataAccess.MongoRepository<Cart.Models.Cart>
    {
        public CartRepository(IMongoDatabase database)
            : base(database, "Carts")
        {
        }
    }
}
