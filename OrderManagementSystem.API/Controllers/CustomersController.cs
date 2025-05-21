using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;
using OrderManagementSystem.Domain.Interfaces;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            customer.CreatedAt = DateTime.UtcNow;
            var createdCustomer = await _customerRepository.AddAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.CustomerId }, createdCustomer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Customer customer)
        {
            if (id != customer.CustomerId)
                return BadRequest("Customer ID mismatch");

            await _customerRepository.UpdateAsync(customer);
            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _customerRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("segment/{segment}")]
        public async Task<IActionResult> GetCustomersBySegment(CustomerSegment segment)
        {
            var customers = await _customerRepository.GetBySegmentAsync(segment);
            return Ok(customers);
        }

        [HttpPut("{id}/segment")]
        public async Task<IActionResult> UpdateCustomerSegment(Guid id, [FromBody] CustomerSegment newSegment)
        {
            await _customerRepository.UpdateCustomerSegmentAsync(id, newSegment);
            return Ok();
        }
    }
} 