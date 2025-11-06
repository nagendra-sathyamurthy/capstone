using catalog.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace catalog.DataAccess
{
    public class MenuItemRepository : MongoRepository<MenuItem>
    {
        public MenuItemRepository(IMongoDatabase database) 
            : base(database, "MenuItems")
        {
        }

        public IEnumerable<MenuItem> GetByCategory(string category)
        {
            return _collection.Find(item => item.Category == category).ToList();
        }

        public IEnumerable<MenuItem> GetByCuisine(string cuisine)
        {
            return _collection.Find(item => item.Cuisine == cuisine).ToList();
        }

        public IEnumerable<MenuItem> GetAvailableItems()
        {
            return _collection.Find(item => item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> GetVegetarianItems()
        {
            return _collection.Find(item => item.IsVegetarian == true && item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> GetVeganItems()
        {
            return _collection.Find(item => item.IsVegan == true && item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> GetGlutenFreeItems()
        {
            return _collection.Find(item => item.IsGlutenFree == true && item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> GetItemsBySpiceLevel(int spiceLevel)
        {
            return _collection.Find(item => item.SpiceLevel == spiceLevel && item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> GetItemsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _collection.Find(item => 
                item.PricePerUOM >= minPrice && 
                item.PricePerUOM <= maxPrice && 
                item.IsAvailable == true).ToList();
        }

        public IEnumerable<MenuItem> SearchByName(string searchTerm)
        {
            var filter = Builders<MenuItem>.Filter.Regex(item => item.Name, 
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
            return _collection.Find(filter).ToList();
        }

        public IEnumerable<MenuItem> GetItemsByPreparationTime(int maxMinutes)
        {
            return _collection.Find(item => 
                item.PreparationTimeMinutes <= maxMinutes && 
                item.IsAvailable == true).ToList();
        }

        public void UpdateAvailability(string id, bool isAvailable)
        {
            var filter = Builders<MenuItem>.Filter.Eq(item => item.Id, id);
            var update = Builders<MenuItem>.Update
                .Set(item => item.IsAvailable, isAvailable)
                .Set(item => item.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public new void Update(string id, MenuItem entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            base.Update(id, entity);
        }
    }

    // Keep ItemRepository for backward compatibility
    public class ItemRepository : MongoRepository<Item>
    {
        public ItemRepository(IMongoDatabase database) 
            : base(database, "Items")
        {
        }
    }
}