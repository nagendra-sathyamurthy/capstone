using Order.Models;
using Order.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/order/delivery")]
    [Authorize(Policy = "DeliveryAgentOnly")]
    public class OrderDeliveryController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly DeliveryService _deliveryService;

        public OrderDeliveryController(OrderService orderService, DeliveryService deliveryService)
        {
            _orderService = orderService;
            _deliveryService = deliveryService;
        }

        /// <summary>
        /// Get available orders for delivery
        /// </summary>
        [HttpGet("available-orders")]
        public async Task<ActionResult<List<Models.Order>>> GetAvailableOrders()
        {
            try
            {
                var orders = await _deliveryService.GetAvailableOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get delivery agent's assigned deliveries
        /// </summary>
        [HttpGet("my-deliveries")]
        public async Task<ActionResult<List<Models.Order>>> GetMyDeliveries()
        {
            try
            {
                // Get delivery agent ID from claims
                var deliveryAgentId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                
                if (string.IsNullOrEmpty(deliveryAgentId))
                    return Unauthorized(new { error = "Delivery agent ID not found in token" });

                var orders = await _deliveryService.GetDeliveryAgentOrdersAsync(deliveryAgentId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get order details
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<Models.Order>> GetOrderDetails(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                
                if (order == null)
                    return NotFound(new { error = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Accept a delivery assignment
        /// </summary>
        [HttpPost("order/{orderId}/accept")]
        public async Task<ActionResult> AcceptDelivery(string orderId)
        {
            try
            {
                var deliveryAgentId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                
                if (string.IsNullOrEmpty(deliveryAgentId))
                    return Unauthorized(new { error = "Delivery agent ID not found in token" });

                await _deliveryService.AcceptDeliveryAsync(orderId, deliveryAgentId);
                return Ok(new { message = "Delivery accepted successfully", orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Mark order as picked up from restaurant
        /// </summary>
        [HttpPost("order/{orderId}/picked-up")]
        public async Task<ActionResult> MarkPickedUp(string orderId)
        {
            try
            {
                var deliveryAgentId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                
                if (string.IsNullOrEmpty(deliveryAgentId))
                    return Unauthorized(new { error = "Delivery agent ID not found in token" });

                await _deliveryService.MarkPickedUpAsync(orderId, deliveryAgentId);
                return Ok(new { message = "Order marked as picked up", orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Mark order as delivered to customer
        /// </summary>
        [HttpPost("order/{orderId}/delivered")]
        public async Task<ActionResult> MarkDelivered(string orderId)
        {
            try
            {
                var deliveryAgentId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
                
                if (string.IsNullOrEmpty(deliveryAgentId))
                    return Unauthorized(new { error = "Delivery agent ID not found in token" });

                await _deliveryService.MarkDeliveredAsync(orderId, deliveryAgentId);
                return Ok(new { message = "Order marked as delivered", orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
