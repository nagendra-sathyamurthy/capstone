using catalog.Models;
using catalog.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace catalog.API.Controllers
{
    [ApiController]
    //[Authorize(Policy = "RestaurantOwnerOnly")] // Temporarily disabled for testing
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantService _restaurantService;
        private readonly MenuService _menuService;

        public RestaurantController(RestaurantService restaurantService, MenuService menuService)
        {
            _restaurantService = restaurantService;
            _menuService = menuService;
        }

        /// <summary>
        /// Register a new restaurant (Restaurant Owner)
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<Restaurant>> RegisterRestaurant([FromBody] Restaurant restaurant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var restaurantId = await _restaurantService.RegisterRestaurantAsync(restaurant);
            restaurant.Id = restaurantId;
            
            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurantId }, restaurant);
        }

        /// <summary>
        /// Get all restaurants (Admin view)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<List<Restaurant>>> GetAllRestaurants()
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(restaurants);
        }

        /// <summary>
        /// Get all active restaurants (Customer view)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Restaurant>>> GetActiveRestaurants()
        {
            var restaurants = await _restaurantService.GetActiveRestaurantsAsync();
            return Ok(restaurants);
        }

        /// <summary>
        /// Get restaurant by ID
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(string id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
                return NotFound($"Restaurant with ID {id} not found");
            
            return Ok(restaurant);
        }

        /// <summary>
        /// Get restaurants owned by a specific owner
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<List<Restaurant>>> GetRestaurantsByOwner(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return BadRequest("Owner ID cannot be empty");

            var restaurants = await _restaurantService.GetRestaurantsByOwnerIdAsync(ownerId);
            return Ok(restaurants);
        }

        /// <summary>
        /// Update restaurant details (Restaurant Owner)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRestaurant(string id, [FromBody] Restaurant restaurant)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            restaurant.Id = id;
            await _restaurantService.UpdateRestaurantAsync(id, restaurant);
            return NoContent();
        }

        /// <summary>
        /// Update restaurant status (active/inactive) (Restaurant Owner)
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateRestaurantStatus(string id, [FromBody] bool isActive)
        {
            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            await _restaurantService.UpdateRestaurantStatusAsync(id, isActive);
            return NoContent();
        }

        /// <summary>
        /// Update restaurant contact information (Restaurant Owner)
        /// </summary>
        [HttpPatch("{id}/contact")]
        public async Task<ActionResult> UpdateContactInfo(string id, [FromBody] ContactInformation contactInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            await _restaurantService.UpdateContactInfoAsync(id, contactInfo);
            return NoContent();
        }

        /// <summary>
        /// Update restaurant address (Restaurant Owner)
        /// </summary>
        [HttpPatch("{id}/address")]
        public async Task<ActionResult> UpdateAddress(string id, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            await _restaurantService.UpdateAddressAsync(id, address);
            return NoContent();
        }

        /// <summary>
        /// Update restaurant business hours (Restaurant Owner)
        /// </summary>
        [HttpPatch("{id}/hours")]
        public async Task<ActionResult> UpdateBusinessHours(string id, [FromBody] BusinessHours timings)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            await _restaurantService.UpdateBusinessHoursAsync(id, timings);
            return NoContent();
        }

        /// <summary>
        /// Delete restaurant (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRestaurant(string id)
        {
            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing == null)
                return NotFound($"Restaurant with ID {id} not found");

            await _restaurantService.DeleteRestaurantAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Get restaurants by city
        /// </summary>
        [AllowAnonymous]
        [HttpGet("city/{city}")]
        public async Task<ActionResult<List<Restaurant>>> GetRestaurantsByCity(string city)
        {
            var restaurants = await _restaurantService.GetRestaurantsByCityAsync(city);
            return Ok(restaurants);
        }

        /// <summary>
        /// Get restaurants by cuisine type
        /// </summary>
        [AllowAnonymous]
        [HttpGet("cuisine/{cuisineType}")]
        public async Task<ActionResult<List<Restaurant>>> GetRestaurantsByCuisine(string cuisineType)
        {
            var restaurants = await _restaurantService.GetRestaurantsByCuisineAsync(cuisineType);
            return Ok(restaurants);
        }

        /// <summary>
        /// Get menu for a specific restaurant (Customer view)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{restaurantId}/menu")]
        public async Task<ActionResult<List<MenuItem>>> GetRestaurantMenu(string restaurantId)
        {
            try
            {
                var menuItems = await _menuService.GetAvailableMenuItemsByRestaurantIdAsync(restaurantId);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
