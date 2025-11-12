using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using catalog.API;

namespace catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly MenuService _menuService;

        public HealthController(MenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Basic health check endpoint
        /// </summary>
        [HttpGet]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                service = "Catalog Service",
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "2.0.0",
                description = "Food Delivery Menu Catalog Service"
            });
        }

        /// <summary>
        /// Detailed health check with database connectivity
        /// </summary>
        [HttpGet("detailed")]
        [Authorize]
        public async Task<ActionResult> GetDetailedHealth()
        {
            try
            {
                // Test database connectivity by getting menu count
                var menuItems = await _menuService.GetAllMenuItemsAsync();
                var availableItems = await _menuService.GetAvailableMenuItemsAsync();

                return Ok(new
                {
                    service = "Catalog Service",
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    version = "2.0.0",
                    description = "Food Delivery Menu Catalog Service",
                    database = new
                    {
                        status = "Connected",
                        totalMenuItems = menuItems.Count,
                        availableMenuItems = availableItems.Count
                    },
                    features = new[]
                    {
                        "Menu Management",
                        "Category Filtering",
                        "Cuisine Filtering",
                        "Dietary Preferences",
                        "Price Range Filtering",
                        "Search Functionality",
                        "Spice Level Filtering",
                        "Preparation Time Filtering"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    service = "Catalog Service",
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    version = "2.0.0",
                    error = ex.Message,
                    database = new
                    {
                        status = "Disconnected"
                    }
                });
            }
        }
    }
}