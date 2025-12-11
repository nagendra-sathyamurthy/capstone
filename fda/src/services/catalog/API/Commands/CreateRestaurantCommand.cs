using catalog.Models;
using catalog.DataAccess;

namespace catalog.API.Commands
{
    /// <summary>
    /// Command for creating a new restaurant
    /// </summary>
    public class CreateRestaurantCommand : ICommand<Restaurant>
    {
        private readonly Restaurant _restaurant;
        private readonly RestaurantRepository _restaurantRepository;

        public CreateRestaurantCommand(Restaurant restaurant, RestaurantRepository restaurantRepository)
        {
            _restaurant = restaurant;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<Restaurant> ExecuteAsync()
        {
            if (_restaurant == null)
            {
                throw new ArgumentNullException(nameof(_restaurant), "Restaurant cannot be null");
            }

            // Validate required fields
            ValidateRestaurant();

            // Set timestamps
            _restaurant.CreatedAt = DateTime.UtcNow;
            _restaurant.UpdatedAt = DateTime.UtcNow;
            _restaurant.IsActive = true;

            // Check if restaurant with same name and owner exists
            await CheckDuplicateRestaurant();

            await _restaurantRepository.AddAsync(_restaurant);
            return _restaurant;
        }

        private void ValidateRestaurant()
        {
            if (string.IsNullOrEmpty(_restaurant.Name))
            {
                throw new ArgumentException("Restaurant name is required");
            }

            if (string.IsNullOrEmpty(_restaurant.OwnerId))
            {
                throw new ArgumentException("Owner ID is required");
            }

            if (_restaurant.Location == null || string.IsNullOrEmpty(_restaurant.Location.Address))
            {
                throw new ArgumentException("Restaurant location is required");
            }
        }

        private async Task CheckDuplicateRestaurant()
        {
            var allRestaurants = await _restaurantRepository.GetAllAsync();
            var duplicate = allRestaurants.FirstOrDefault(r =>
                r.Name == _restaurant.Name &&
                r.OwnerId == _restaurant.OwnerId);

            if (duplicate != null)
            {
                throw new InvalidOperationException($"Restaurant '{_restaurant.Name}' already exists for this owner");
            }
        }
    }
}
