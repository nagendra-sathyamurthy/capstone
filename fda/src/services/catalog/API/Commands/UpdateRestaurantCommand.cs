using Catalog.Models;
using Catalog.DataAccess;

namespace Catalog.API.Commands
{
    /// <summary>
    /// Command for updating an existing restaurant
    /// </summary>
    public class UpdateRestaurantCommand : ICommand<Restaurant>
    {
        private readonly string _restaurantId;
        private readonly Restaurant _updatedRestaurant;
        private readonly RestaurantRepository _restaurantRepository;

        public UpdateRestaurantCommand(
            string restaurantId,
            Restaurant updatedRestaurant,
            RestaurantRepository restaurantRepository)
        {
            _restaurantId = restaurantId;
            _updatedRestaurant = updatedRestaurant;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<Restaurant> ExecuteAsync()
        {
            // Validate inputs
            if (string.IsNullOrEmpty(_restaurantId))
            {
                throw new ArgumentException("Restaurant ID cannot be null or empty", nameof(_restaurantId));
            }

            if (_updatedRestaurant == null)
            {
                throw new ArgumentException("Updated restaurant data cannot be null", nameof(_updatedRestaurant));
            }

            // Validate required fields
            if (string.IsNullOrEmpty(_updatedRestaurant.Name))
            {
                throw new ArgumentException("Restaurant name is required", nameof(_updatedRestaurant.Name));
            }

            if (string.IsNullOrEmpty(_updatedRestaurant.OwnerId))
            {
                throw new ArgumentException("Owner ID is required", nameof(_updatedRestaurant.OwnerId));
            }

            if (_updatedRestaurant.Location == null || string.IsNullOrEmpty(_updatedRestaurant.Location.Address))
            {
                throw new ArgumentException("Restaurant location with address is required");
            }

            // Get existing restaurant
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(_restaurantId);
            if (existingRestaurant == null)
            {
                throw new InvalidOperationException($"Restaurant with ID {_restaurantId} not found");
            }

            // Check for duplicate name if name is being changed
            if (existingRestaurant.Name != _updatedRestaurant.Name)
            {
                var allRestaurants = await _restaurantRepository.GetAllAsync();
                var duplicate = allRestaurants.FirstOrDefault(r =>
                    r.Name == _updatedRestaurant.Name &&
                    r.OwnerId == _updatedRestaurant.OwnerId &&
                    r.RestaurantId != _restaurantId);

                if (duplicate != null)
                {
                    throw new InvalidOperationException(
                        $"A restaurant with name '{_updatedRestaurant.Name}' already exists for this owner");
                }
            }

            // Update fields (preserve RestaurantId and CreatedAt)
            existingRestaurant.Name = _updatedRestaurant.Name;
            existingRestaurant.OwnerId = _updatedRestaurant.OwnerId;
            existingRestaurant.Location = _updatedRestaurant.Location;
            existingRestaurant.ContactNumber = _updatedRestaurant.ContactNumber;
            existingRestaurant.Cuisine = _updatedRestaurant.Cuisine;
            existingRestaurant.Description = _updatedRestaurant.Description;
            existingRestaurant.ProfileImage = _updatedRestaurant.ProfileImage;
            existingRestaurant.IsActive = _updatedRestaurant.IsActive;
            existingRestaurant.UpdatedAt = DateTime.UtcNow;

            // Update in database
            await _restaurantRepository.UpdateAsync(existingRestaurant);

            return existingRestaurant;
        }
    }
}
