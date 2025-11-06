using catalog.Models;
using catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Get all menu items (including unavailable ones - for admin)
        /// </summary>
        [HttpGet("all")]
        [Authorize] // Require authentication for admin view
        public async Task<ActionResult<List<MenuItem>>> GetAllMenuItems()
        {
            var items = await _menuService.GetAllMenuItemsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get all available menu items (public endpoint)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<MenuItem>>> GetAvailableMenuItems()
        {
            var items = await _menuService.GetAvailableMenuItemsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(string id)
        {
            var item = await _menuService.GetMenuItemByIdAsync(id);
            if (item == null) 
                return NotFound($"Menu item with ID {id} not found");
            
            return Ok(item);
        }

        /// <summary>
        /// Create a new menu item
        /// </summary>
        [HttpPost]
        [Authorize] // Require authentication for creating menu items
        public async Task<ActionResult<MenuItem>> CreateMenuItem([FromBody] MenuItem menuItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _menuService.CreateMenuItemAsync(menuItem);
            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
        }

        /// <summary>
        /// Create multiple menu items at once
        /// </summary>
        [HttpPost("bulk")]
        [Authorize]
        public async Task<ActionResult> CreateMenuItems([FromBody] List<MenuItem> menuItems)
        {
            if (menuItems == null || !menuItems.Any())
                return BadRequest("Menu items list cannot be empty");

            await _menuService.CreateMenuItemsAsync(menuItems);
            return Ok($"Successfully created {menuItems.Count} menu items");
        }

        /// <summary>
        /// Update a menu item
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateMenuItem(string id, [FromBody] MenuItem menuItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _menuService.GetMenuItemByIdAsync(id);
            if (existing == null)
                return NotFound($"Menu item with ID {id} not found");

            await _menuService.UpdateMenuItemAsync(id, menuItem);
            return NoContent();
        }

        /// <summary>
        /// Delete a menu item
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteMenuItem(string id)
        {
            var existing = await _menuService.GetMenuItemByIdAsync(id);
            if (existing == null)
                return NotFound($"Menu item with ID {id} not found");

            await _menuService.DeleteMenuItemAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Update menu item availability
        /// </summary>
        [HttpPatch("{id}/availability")]
        [Authorize]
        public async Task<ActionResult> UpdateAvailability(string id, [FromBody] bool isAvailable)
        {
            var existing = await _menuService.GetMenuItemByIdAsync(id);
            if (existing == null)
                return NotFound($"Menu item with ID {id} not found");

            await _menuService.UpdateAvailabilityAsync(id, isAvailable);
            return Ok($"Menu item availability updated to {(isAvailable ? "available" : "unavailable")}");
        }

        // Category-based endpoints
        /// <summary>
        /// Get menu items by category
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<MenuItem>>> GetMenuItemsByCategory(string category)
        {
            var items = await _menuService.GetMenuItemsByCategoryAsync(category);
            return Ok(items);
        }

        /// <summary>
        /// Get available categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _menuService.GetAvailableCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get menu organized by categories
        /// </summary>
        [HttpGet("menu/categories")]
        public async Task<ActionResult<Dictionary<string, List<MenuItem>>>> GetMenuByCategories()
        {
            var menu = await _menuService.GetMenuByCategoriesAsync();
            return Ok(menu);
        }

        // Cuisine-based endpoints
        /// <summary>
        /// Get menu items by cuisine
        /// </summary>
        [HttpGet("cuisine/{cuisine}")]
        public async Task<ActionResult<List<MenuItem>>> GetMenuItemsByCuisine(string cuisine)
        {
            var items = await _menuService.GetMenuItemsByCuisineAsync(cuisine);
            return Ok(items);
        }

        /// <summary>
        /// Get available cuisines
        /// </summary>
        [HttpGet("cuisines")]
        public async Task<ActionResult<List<string>>> GetCuisines()
        {
            var cuisines = await _menuService.GetAvailableCuisinesAsync();
            return Ok(cuisines);
        }

        /// <summary>
        /// Get menu organized by cuisines
        /// </summary>
        [HttpGet("menu/cuisines")]
        public async Task<ActionResult<Dictionary<string, List<MenuItem>>>> GetMenuByCuisines()
        {
            var menu = await _menuService.GetMenuByCuisinesAsync();
            return Ok(menu);
        }

        // Dietary preference endpoints
        /// <summary>
        /// Get vegetarian menu items
        /// </summary>
        [HttpGet("vegetarian")]
        public async Task<ActionResult<List<MenuItem>>> GetVegetarianMenuItems()
        {
            var items = await _menuService.GetVegetarianMenuItemsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get vegan menu items
        /// </summary>
        [HttpGet("vegan")]
        public async Task<ActionResult<List<MenuItem>>> GetVeganMenuItems()
        {
            var items = await _menuService.GetVeganMenuItemsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get gluten-free menu items
        /// </summary>
        [HttpGet("gluten-free")]
        public async Task<ActionResult<List<MenuItem>>> GetGlutenFreeMenuItems()
        {
            var items = await _menuService.GetGlutenFreeMenuItemsAsync();
            return Ok(items);
        }

        // Search and filter endpoints
        /// <summary>
        /// Search menu items by name
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<MenuItem>>> SearchMenuItems([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty");

            var items = await _menuService.SearchMenuItemsByNameAsync(searchTerm);
            return Ok(items);
        }

        /// <summary>
        /// Get menu items by price range
        /// </summary>
        [HttpGet("price-range")]
        public async Task<ActionResult<List<MenuItem>>> GetMenuItemsByPriceRange(
            [FromQuery] decimal minPrice, 
            [FromQuery] decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                return BadRequest("Invalid price range");

            var items = await _menuService.GetMenuItemsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(items);
        }

        /// <summary>
        /// Get menu items by spice level
        /// </summary>
        [HttpGet("spice-level/{level}")]
        public async Task<ActionResult<List<MenuItem>>> GetMenuItemsBySpiceLevel(int level)
        {
            if (level < 1 || level > 5)
                return BadRequest("Spice level must be between 1 and 5");

            var items = await _menuService.GetMenuItemsBySpiceLevelAsync(level);
            return Ok(items);
        }

        /// <summary>
        /// Get quick preparation menu items
        /// </summary>
        [HttpGet("quick")]
        public async Task<ActionResult<List<MenuItem>>> GetQuickMenuItems([FromQuery] int maxMinutes = 30)
        {
            if (maxMinutes <= 0)
                return BadRequest("Maximum preparation time must be greater than 0");

            var items = await _menuService.GetQuickMenuItemsAsync(maxMinutes);
            return Ok(items);
        }

        /// <summary>
        /// Advanced filter endpoint
        /// </summary>
        [HttpGet("filter")]
        public async Task<ActionResult<List<MenuItem>>> GetFilteredMenuItems(
            [FromQuery] string? category = null,
            [FromQuery] string? cuisine = null,
            [FromQuery] bool? isVegetarian = null,
            [FromQuery] bool? isVegan = null,
            [FromQuery] bool? isGlutenFree = null,
            [FromQuery] int? maxSpiceLevel = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? maxPreparationTime = null)
        {
            var items = await _menuService.GetFilteredMenuItemsAsync(
                category, cuisine, isVegetarian, isVegan, isGlutenFree,
                maxSpiceLevel, minPrice, maxPrice, maxPreparationTime);
            
            return Ok(items);
        }
    }
}