using catalog.Models;
using catalog.DataAccess;
using MongoDB.Driver;

namespace catalog.API
{
    public class RestaurantService
    {
        private readonly RestaurantRepository _restaurantRepository;

        public RestaurantService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CatalogDb");
            _restaurantRepository = new RestaurantRepository(database);
        }

        // Restaurant Registration and Management
        public Task<string> RegisterRestaurantAsync(Restaurant restaurant)
        {
            restaurant.CreatedAt = DateTime.UtcNow;
            restaurant.UpdatedAt = DateTime.UtcNow;
            _restaurantRepository.Insert(restaurant);
            return Task.FromResult(restaurant.Id ?? string.Empty);
        }

        public Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            return Task.FromResult(_restaurantRepository.GetAll().ToList());
        }

        public Task<List<Restaurant>> GetActiveRestaurantsAsync()
        {
            return Task.FromResult(_restaurantRepository.GetActiveRestaurants().ToList());
        }

        public Task<Restaurant?> GetRestaurantByIdAsync(string id)
        {
            return Task.FromResult(_restaurantRepository.GetById(id));
        }

        public Task<List<Restaurant>> GetRestaurantsByOwnerIdAsync(string ownerId)
        {
            return Task.FromResult(_restaurantRepository.GetByOwnerId(ownerId).ToList());
        }

        public Task UpdateRestaurantAsync(string id, Restaurant restaurant)
        {
            restaurant.UpdatedAt = DateTime.UtcNow;
            _restaurantRepository.Update(id, restaurant);
            return Task.CompletedTask;
        }

        public Task DeleteRestaurantAsync(string id)
        {
            _restaurantRepository.Delete(id);
            return Task.CompletedTask;
        }

        // Status Management
        public Task UpdateRestaurantStatusAsync(string id, bool isActive)
        {
            _restaurantRepository.UpdateStatus(id, isActive);
            return Task.CompletedTask;
        }

        // Contact and Address Updates
        public Task UpdateContactInfoAsync(string id, ContactInformation contactInfo)
        {
            _restaurantRepository.UpdateContactInfo(id, contactInfo);
            return Task.CompletedTask;
        }

        public Task UpdateAddressAsync(string id, Address address)
        {
            _restaurantRepository.UpdateAddress(id, address);
            return Task.CompletedTask;
        }

        public Task UpdateBusinessHoursAsync(string id, BusinessHours timings)
        {
            _restaurantRepository.UpdateBusinessHours(id, timings);
            return Task.CompletedTask;
        }

        // Search and Filter
        public Task<List<Restaurant>> GetRestaurantsByCityAsync(string city)
        {
            return Task.FromResult(_restaurantRepository.GetByCity(city).ToList());
        }

        public Task<List<Restaurant>> GetRestaurantsByCuisineAsync(string cuisineType)
        {
            return Task.FromResult(_restaurantRepository.GetByCuisineType(cuisineType).ToList());
        }
    }
}
