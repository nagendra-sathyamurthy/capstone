using catalog.Models;
using catalog.DataAccess;
using catalog.API.Commands;
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
        public async Task<string> RegisterRestaurantAsync(Restaurant restaurant)
        {
            // Use CreateRestaurantCommand for restaurant creation logic
            var command = new CreateRestaurantCommand(restaurant, _restaurantRepository);
            var createdRestaurant = await command.ExecuteAsync();
            return createdRestaurant.Id ?? string.Empty;
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

        public async Task<List<Restaurant>> GetRestaurantsByOwnerIdAsync(string ownerId)
        {
            // Use GetRestaurantsByOwnerCommand for filtering by owner
            var command = new GetRestaurantsByOwnerCommand(ownerId, _restaurantRepository);
            return await command.ExecuteAsync();
        }

        public async Task UpdateRestaurantAsync(string id, Restaurant restaurant)
        {
            // Use UpdateRestaurantCommand for restaurant updates
            var command = new UpdateRestaurantCommand(id, restaurant, _restaurantRepository);
            await command.ExecuteAsync();
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
