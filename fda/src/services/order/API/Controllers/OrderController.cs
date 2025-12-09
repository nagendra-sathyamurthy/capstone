using Order.Models;
using Order.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order (Customer)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> CreateOrder([FromBody] Models.Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var orderId = await _orderService.CreateOrderAsync(order);
                return Ok(new { orderId, message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get pending orders for restaurant (Operator/Restaurant Staff)
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/pending")]
        [Authorize(Policy = "RestaurantStaff")]
        public async Task<ActionResult<List<Models.Order>>> GetPendingOrders(string restaurantId)
        {
            var orders = await _orderService.GetPendingOrdersAsync(restaurantId);
            return Ok(orders);
        }

        /// <summary>
        /// Get orders ready for pickup (Operator/Delivery Agent)
        /// </summary>
        [HttpGet("restaurant/{restaurantId}/ready")]
        [Authorize(Policy = "RestaurantStaff")]
        public async Task<ActionResult<List<Models.Order>>> GetReadyForPickupOrders(string restaurantId)
        {
            var orders = await _orderService.GetReadyForPickupOrdersAsync(restaurantId);
            return Ok(orders);
        }

        /// <summary>
        /// Get all orders for a restaurant
        /// </summary>
        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Policy = "RestaurantStaff")]
        public async Task<ActionResult<List<Models.Order>>> GetRestaurantOrders(string restaurantId)
        {
            var orders = await _orderService.GetOrdersByRestaurantAsync(restaurantId);
            return Ok(orders);
        }

        /// <summary>
        /// Get orders by customer
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<List<Models.Order>>> GetCustomerOrders(string customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// Get order details
        /// </summary>
        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<ActionResult<Models.Order>> GetOrder(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            
            if (order == null)
                return NotFound(new { error = "Order not found" });

            return Ok(order);
        }

        /// <summary>
        /// Accept an order (Operator)
        /// </summary>
        [HttpPost("{orderId}/accept")]
        [Authorize(Policy = "RestaurantStaff")]
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
        [HttpPost("{orderId}/decline")]
        [Authorize(Policy = "RestaurantStaff")]
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
        /// Update order status
        /// </summary>
        [HttpPatch("{orderId}/status")]
        [Authorize(Policy = "RestaurantStaff")]
        public async Task<ActionResult> UpdateOrderStatus(
            string orderId, 
            [FromBody] OrderStatus status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Handle order packaging (Operator)
        /// </summary>
        [HttpPost("{orderId}/package")]
        [Authorize(Policy = "RestaurantStaff")]
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
        [HttpPost("{orderId}/generate-handover-otp")]
        [Authorize(Policy = "RestaurantStaff")]
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
        [HttpPost("handover")]
        [Authorize(Policy = "RestaurantStaff")]
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
        /// Cancel an order (Customer)
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        [Authorize]
        public async Task<ActionResult> CancelOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order == null)
                    return NotFound(new { error = "Order not found" });

                // Only allow cancellation if order is still pending or accepted
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Accepted)
                {
                    return BadRequest(new { error = "Cannot cancel order in current status" });
                }

                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
                return Ok(new { message = "Order cancelled successfully", orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
