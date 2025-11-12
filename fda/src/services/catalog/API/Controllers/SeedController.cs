using catalog.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace catalog.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly MenuService _menuService;

        public SeedController(MenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Seed the database with sample menu items for testing
        /// </summary>
        [HttpPost("sample-menu")]
        public async Task<ActionResult> SeedSampleMenu()
        {
            try
            {
                var sampleItems = SampleMenuData.GetSampleMenuItems();
                await _menuService.CreateMenuItemsAsync(sampleItems);
                
                return Ok(new 
                { 
                    message = "Sample menu items created successfully", 
                    count = sampleItems.Count,
                    categories = sampleItems.Select(i => i.Category).Distinct().ToList(),
                    cuisines = sampleItems.Select(i => i.Cuisine).Distinct().ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error seeding sample data", error = ex.Message });
            }
        }

        /// <summary>
        /// Get seeding status and database statistics
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetSeedingStatus()
        {
            try
            {
                var allItems = await _menuService.GetAllMenuItemsAsync();
                var availableItems = await _menuService.GetAvailableMenuItemsAsync();
                var categories = await _menuService.GetAvailableCategoriesAsync();
                var cuisines = await _menuService.GetAvailableCuisinesAsync();

                return Ok(new
                {
                    totalMenuItems = allItems.Count,
                    availableMenuItems = availableItems.Count,
                    categoriesCount = categories.Count,
                    categories = categories,
                    cuisinesCount = cuisines.Count,
                    cuisines = cuisines,
                    itemsByCategory = availableItems.GroupBy(i => i.Category)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    itemsByCuisine = availableItems.GroupBy(i => i.Cuisine)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    dietaryOptions = new
                    {
                        vegetarian = availableItems.Count(i => i.IsVegetarian),
                        vegan = availableItems.Count(i => i.IsVegan),
                        glutenFree = availableItems.Count(i => i.IsGlutenFree)
                    },
                    priceRange = new
                    {
                        min = availableItems.Any() ? availableItems.Min(i => i.PricePerUOM) : 0,
                        max = availableItems.Any() ? availableItems.Max(i => i.PricePerUOM) : 0,
                        average = availableItems.Any() ? availableItems.Average(i => i.PricePerUOM) : 0
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting seeding status", error = ex.Message });
            }
        }
    }
}