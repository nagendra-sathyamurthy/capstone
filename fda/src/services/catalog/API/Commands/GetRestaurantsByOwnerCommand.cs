using catalog.Models;
using catalog.DataAccess;

namespace catalog.API.Commands
{
    /// <summary>
    /// Command for retrieving restaurants by owner
    /// </summary>
    public class GetRestaurantsByOwnerCommand : ICommand<List<Restaurant>>
    {
        private readonly string _ownerId;
        private readonly RestaurantRepository _restaurantRepository;

        public GetRestaurantsByOwnerCommand(string ownerId, RestaurantRepository restaurantRepository)
        {
            _ownerId = ownerId;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<List<Restaurant>> ExecuteAsync()
        {
            if (string.IsNullOrEmpty(_ownerId))
            {
                throw new ArgumentException("Owner ID cannot be null or empty", nameof(_ownerId));
            }

            var allRestaurants = await _restaurantRepository.GetAllAsync();
            return allRestaurants.Where(r => r.OwnerId == _ownerId).ToList();
        }
    }
}
