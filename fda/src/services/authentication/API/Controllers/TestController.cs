using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Authentication.Models;
using Authentication.API.BusinessServices;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AuthService _authService;

        public TestController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Test endpoint for Customer role only
        /// </summary>
        [HttpGet("customer-only")]
        [Authorize(Policy = "CustomerOnly")]
        public IActionResult CustomerOnlyEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            return Ok(new { 
                message = "Welcome Customer!", 
                email = userEmail, 
                role = userRole,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for Restaurant Staff (Biller, Operator, Worker)
        /// </summary>
        [HttpGet("restaurant-staff")]
        [Authorize(Policy = "RestaurantStaff")]
        public IActionResult RestaurantStaffEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            return Ok(new { 
                message = "Welcome Restaurant Staff Member!", 
                email = userEmail, 
                role = userRole,
                organization = User.FindFirst("organization")?.Value,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for IT Staff (Developer, Tester, NetworkAdmin, DatabaseAdmin)
        /// </summary>
        [HttpGet("it-staff")]
        [Authorize(Policy = "ITStaff")]
        public IActionResult ITStaffEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            return Ok(new { 
                message = "Welcome IT Staff Member!", 
                email = userEmail, 
                role = userRole,
                department = User.FindFirst("department")?.Value,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for Management Level (Biller, Operator)
        /// </summary>
        [HttpGet("management")]
        [Authorize(Policy = "ManagementLevel")]
        public IActionResult ManagementEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            return Ok(new { 
                message = "Welcome Manager!", 
                email = userEmail, 
                role = userRole,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for payment processing (Biller only)
        /// </summary>
        [HttpGet("payment-processing")]
        [Authorize(Policy = "CanReceivePayments")]
        public IActionResult PaymentProcessingEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Payment Processing Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for order confirmation (Operator only)
        /// </summary>
        [HttpGet("order-confirmation")]
        [Authorize(Policy = "CanConfirmOrders")]
        public IActionResult OrderConfirmationEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Order Confirmation Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for food preparation (Worker only)
        /// </summary>
        [HttpGet("food-preparation")]
        [Authorize(Policy = "CanPrepareFood")]
        public IActionResult FoodPreparationEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Food Preparation Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for delivery confirmation (DeliveryAgent only)
        /// </summary>
        [HttpGet("delivery-confirmation")]
        [Authorize(Policy = "CanConfirmDelivery")]
        public IActionResult DeliveryConfirmationEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Delivery Confirmation Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for full system access (Developer/Tester only)
        /// </summary>
        [HttpGet("full-system-access")]
        [Authorize(Policy = "CanAccessAllEndpoints")]
        public IActionResult FullSystemAccessEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Full System Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for healthcheck access (NetworkAdmin only)
        /// </summary>
        [HttpGet("healthcheck-access")]
        [Authorize(Policy = "CanAccessHealthcheck")]
        public IActionResult HealthcheckAccessEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Healthcheck Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for database access (DatabaseAdmin only)
        /// </summary>
        [HttpGet("database-access")]
        [Authorize(Policy = "CanAccessDatabase")]
        public IActionResult DatabaseAccessEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims.Where(c => c.Type == "permission").Select(c => c.Value);
            
            return Ok(new { 
                message = "Database Access Granted!", 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Test endpoint for Delivery Agent only
        /// </summary>
        [HttpGet("delivery-agent")]
        [Authorize(Policy = "DeliveryAgentOnly")]
        public IActionResult DeliveryAgentEndpoint()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            return Ok(new { 
                message = "Welcome Delivery Agent! You can pickup, transport and confirm deliveries.", 
                email = userEmail, 
                role = userRole,
                vehicle = User.FindFirst("vehicle_type")?.Value,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Get current user's permissions
        /// </summary>
        [HttpGet("my-permissions")]
        [Authorize]
        public IActionResult GetMyPermissions()
        {
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var permissions = User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
            
            return Ok(new { 
                email = userEmail, 
                role = userRole,
                permissions = permissions,
                organization = User.FindFirst("organization")?.Value,
                timestamp = DateTime.UtcNow 
            });
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        [HttpGet("check-permission/{permission}")]
        [Authorize]
        public async Task<IActionResult> CheckPermission(string permission)
        {
            var userEmail = User.FindFirst("email")?.Value;
            
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User email not found in token");
            }

            var hasPermission = await _authService.CheckPermissionAsync(userEmail, permission);
            
            return Ok(new { 
                email = userEmail,
                permission = permission,
                hasPermission = hasPermission,
                timestamp = DateTime.UtcNow 
            });
        }
    }
}