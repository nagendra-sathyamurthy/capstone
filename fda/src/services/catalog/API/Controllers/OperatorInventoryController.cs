using catalog.Models;
using catalog.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace catalog.API.Controllers
{
    [ApiController]
    [Route("api/catalog/operator/inventory")]
    [Authorize(Policy = "RestaurantOperatorOnly")]
    public class OperatorInventoryController : ControllerBase
    {
        private readonly MenuService _menuService;

        public OperatorInventoryController(MenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// View all inventory for restaurant
        /// </summary>
        [HttpGet("{restaurantId}")]
        public async Task<ActionResult<List<MenuItem>>> GetInventory(string restaurantId)
        {
            try
            {
                var items = await _menuService.GetMenuItemsByRestaurantIdAsync(restaurantId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message });
            }
        }

        /// <summary>
        /// Get low stock items
        /// </summary>
        [HttpGet("{restaurantId}/low-stock")]
        public async Task<ActionResult<List<MenuItem>>> GetLowStockItems(string restaurantId)
        {
            try
            {
                // For now, return items that are marked as unavailable
                var allItems = await _menuService.GetMenuItemsByRestaurantIdAsync(restaurantId);
                var lowStockItems = allItems.Where(item => !item.IsAvailable).ToList();
                return Ok(lowStockItems);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message });
            }
        }

        /// <summary>
        /// Update food item availability
        /// </summary>
        [HttpPut("{menuItemId}/availability")]
        public async Task<ActionResult> UpdateAvailability(string menuItemId, [FromBody] UpdateAvailabilityRequest request)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                    return NotFound(new { error = true, message = "Menu item not found" });

                menuItem.IsAvailable = request.IsAvailable;
                await _menuService.UpdateMenuItemAsync(menuItemId, menuItem);

                return Ok(new { message = "Menu item availability updated successfully", menuItemId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = true, message = ex.Message });
            }
        }
    }

    // Request models
    public class UpdateAvailabilityRequest
    {
        public bool IsAvailable { get; set; }
    }
}
