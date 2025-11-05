using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Crm.Models;
using Crm.Services;
using Microsoft.AspNetCore.Authorization;
namespace Crm.Services.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetById(string id)
        {
            var customer = _service.GetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }


        [HttpPost]
        public ActionResult<Customer> Create(Customer customer)
        {
            try
            {
                _service.Create(customer);
                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public ActionResult CreateMany(List<Customer> customers)
        {
            _service.CreateMany(customers);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Customer customer)
        {
            _service.Update(id, customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
