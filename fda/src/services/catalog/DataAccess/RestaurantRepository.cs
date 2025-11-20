using catalog.Models;
using MongoDB.Driver;

namespace catalog.DataAccess
{
    public class RestaurantRepository : MongoRepository<Restaurant>
    {
        public RestaurantRepository(IMongoDatabase database) 
            : base(database, "Restaurants")
        {
        }

        public IEnumerable<Restaurant> GetByOwnerId(string ownerId)
        {
            return _collection.Find(restaurant => restaurant.OwnerId == ownerId).ToList();
        }

        public IEnumerable<Restaurant> GetActiveRestaurants()
        {
            return _collection.Find(restaurant => restaurant.IsActive == true).ToList();
        }

        public IEnumerable<Restaurant> GetByCity(string city)
        {
            return _collection.Find(restaurant => restaurant.Address.City == city && restaurant.IsActive == true).ToList();
        }

        public IEnumerable<Restaurant> GetByCuisineType(string cuisineType)
        {
            return _collection.Find(restaurant => restaurant.CuisineType == cuisineType && restaurant.IsActive == true).ToList();
        }

        public void UpdateStatus(string id, bool isActive)
        {
            var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, id);
            var update = Builders<Restaurant>.Update
                .Set(r => r.IsActive, isActive)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void UpdateContactInfo(string id, ContactInformation contactInfo)
        {
            var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, id);
            var update = Builders<Restaurant>.Update
                .Set(r => r.ContactInfo, contactInfo)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void UpdateAddress(string id, Address address)
        {
            var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, id);
            var update = Builders<Restaurant>.Update
                .Set(r => r.Address, address)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public void UpdateBusinessHours(string id, BusinessHours timings)
        {
            var filter = Builders<Restaurant>.Filter.Eq(r => r.Id, id);
            var update = Builders<Restaurant>.Update
                .Set(r => r.Timings, timings)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            
            _collection.UpdateOne(filter, update);
        }

        public new void Update(string id, Restaurant entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            base.Update(id, entity);
        }
    }
}
