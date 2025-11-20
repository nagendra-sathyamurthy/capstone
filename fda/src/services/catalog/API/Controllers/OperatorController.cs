using catalog.Models;
using catalog.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace catalog.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "RestaurantStaff")]
    [Route("api/[controller]")]
    public class OperatorController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OperatorController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get pending orders for restaurant (Operator)
        /// </summary>
        [HttpGet("orders/pending/{restaurantId}")]
        public async Task<ActionResult<List<Order>>> GetPendingOrders(string restaurantId)
        {
            var orders = await _orderService.GetPendingOrdersAsync(restaurantId);
            return Ok(orders);
        }

        /// <summary>
        /// Get orders ready for pickup (Operator)
        /// </summary>
        [HttpGet("orders/ready/{restaurantId}")]
        public async Task<ActionResult<List<Order>>> GetReadyForPickupOrders(string restaurantId)
        {
            var orders = await _orderService.GetReadyForPickupOrdersAsync(restaurantId);
            return Ok(orders);
        }

        /// <summary>
        /// Accept an order (Operator)
        /// </summary>
        [HttpPost("orders/{orderId}/accept")]
        public async Task<ActionResult> AcceptOrder(string orderId)
        {
            try
            {
                await _orderService.AcceptOrderAsync(orderId);
                return Ok(new { message = "Order accepted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Decline an order (Operator)
        /// </summary>
        [HttpPost("orders/{orderId}/decline")]
        public async Task<ActionResult> DeclineOrder(string orderId)
        {
            try
            {
                await _orderService.DeclineOrderAsync(orderId);
                return Ok(new { message = "Order declined" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update food item availability (Operator)
        /// </summary>
        [HttpPatch("inventory/{menuItemId}/availability")]
        public async Task<ActionResult> UpdateFoodItemAvailability(
            string menuItemId, 
            [FromBody] InventoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.MenuItemId = menuItemId;
                await _orderService.UpdateMenuItemAvailabilityAsync(menuItemId, request);
                return Ok(new { message = "Food item availability updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// View kitchen inventory (Operator)
        /// </summary>
        [HttpGet("inventory/{restaurantId}")]
        public async Task<ActionResult<List<MenuItem>>> GetKitchenInventory(string restaurantId)
        {
            var items = await _orderService.GetKitchenInventoryAsync(restaurantId);
            return Ok(items);
        }

        /// <summary>
        /// Get low stock items (Operator)
        /// </summary>
        [HttpGet("inventory/{restaurantId}/low-stock")]
        public async Task<ActionResult<List<MenuItem>>> GetLowStockItems(string restaurantId)
        {
            var items = await _orderService.GetLowStockItemsAsync(restaurantId);
            return Ok(items);
        }

        /// <summary>
        /// Handle order packaging (Operator)
        /// </summary>
        [HttpPost("orders/{orderId}/package")]
        public async Task<ActionResult> PackageOrder(
            string orderId, 
            [FromBody] PackagingDetails packaging)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _orderService.UpdatePackagingAsync(orderId, packaging);
                return Ok(new { message = "Order packaged successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Generate OTP for delivery agent handover (Operator)
        /// </summary>
        [HttpPost("orders/{orderId}/generate-handover-otp")]
        public async Task<ActionResult> GenerateHandoverOTP(
            string orderId, 
            [FromBody] string deliveryAgentId)
        {
            try
            {
                var otp = await _orderService.GenerateHandoverOTPAsync(orderId, deliveryAgentId);
                return Ok(new 
                { 
                    message = "OTP generated successfully",
                    otp = otp,
                    expiresIn = "5 minutes"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Verify OTP and handover order to delivery agent (Operator)
        /// </summary>
        [HttpPost("orders/handover")]
        public async Task<ActionResult> HandoverToDeliveryAgent([FromBody] HandoverRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isValid = await _orderService.VerifyHandoverOTPAsync(request);
                
                if (!isValid)
                {
                    return BadRequest(new { error = "Invalid OTP or handover request" });
                }

                return Ok(new { message = "Order handed over successfully to delivery agent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get order details (Operator)
        /// </summary>
        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<Order>> GetOrder(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            
            if (order == null)
                return NotFound(new { error = "Order not found" });

            return Ok(order);
        }
    }
}
