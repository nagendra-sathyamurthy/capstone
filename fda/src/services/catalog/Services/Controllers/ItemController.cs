using catalog.Models;
using catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Services.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _service;

        public ItemController(ItemService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all items (requires authentication)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Item>>> Get() => await _service.GetAllAsync();

        /// <summary>
        /// Get item by ID (requires authentication)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        /// <summary>
        /// Create new item (requires authentication)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create(Item item)
        {
            await _service.CreateAsync(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        /// <summary>
        /// Create multiple items (requires authentication)
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult> CreateMany(List<Item> items)
        {
            await _service.CreateManyAsync(items);
            return Ok();
        }

        /// <summary>
        /// Update item (requires authentication)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Item item)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.UpdateAsync(id, item);
            return NoContent();
        }

        /// <summary>
        /// Delete item (requires authentication)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}