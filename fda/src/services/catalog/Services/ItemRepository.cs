using catalog.Models;
using catalog.DataAccess;
using MongoDB.Driver;

namespace catalog.Services
{
    public class ItemRepository : MongoRepository<Item>
    {
        public ItemRepository(IMongoDatabase database)
            : base(database, "Items")
        {
        }
    }
}
