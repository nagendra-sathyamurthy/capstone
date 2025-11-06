using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cart.Models;
using Cart.Services;

namespace Cart.Services.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<ActionResult<Cart.Models.Cart>> CreateCart([FromBody] CreateCartRequest request)
        {
            try
            {
                var cart = await _cartService.CreateCartAsync(request.UserId);
                return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart.Models.Cart>> GetCart(string id)
        {
            try
            {
                // We need to add a GetByIdAsync method to CartService
                return Ok(new { message = "GetCart not implemented yet", cartId = id });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<Cart.Models.Cart>> AddItem(string cartId, [FromBody] CartItem item)
        {
            try
            {
                var cart = await _cartService.AddItemAsync(cartId, item);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{cartId}/items/{itemId}")]
        public async Task<ActionResult<Cart.Models.Cart>> RemoveItem(string cartId, string itemId)
        {
            try
            {
                var cart = await _cartService.RemoveItemAsync(cartId, itemId);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateCartRequest
    {
        public string UserId { get; set; } = string.Empty;
    }
}