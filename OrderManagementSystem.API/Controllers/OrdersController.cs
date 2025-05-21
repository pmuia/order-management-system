using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Models;

namespace OrderManagementSystem.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderRepository orderRepository,
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (orders, totalCount) = await _orderRepository.GetPagedOrdersAsync(pageNumber, pageSize);
                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }

        [HttpGet("summaries")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrderSummaries()
        {
            try
            {
                var summaries = await _orderRepository.GetOrderSummariesAsync();
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order summaries");
                return StatusCode(500, "An error occurred while retrieving order summaries");
            }
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                // If user is not admin/manager, verify they own the order
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (order.CustomerId.ToString() != customerId)
                    {
                        return Forbid();
                    }
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, "An error occurred while retrieving the order");
            }
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            try
            {
                // If user is not admin/manager, ensure they can only create orders for themselves
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    var customerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    order.CustomerId = Guid.Parse(customerId);
                }

                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred while creating the order");
            }
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateOrder(Guid id, Order order)
        {
            try
            {
                if (id != order.OrderId)
                {
                    return BadRequest();
                }

                await _orderService.UpdateOrderAsync(order);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", id);
                return StatusCode(500, "An error occurred while updating the order");
            }
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                await _orderRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return StatusCode(500, "An error occurred while deleting the order");
            }
        }

        [HttpPut("batch/status")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateOrderStatuses(
            [FromBody] IEnumerable<Guid> orderIds,
            [FromQuery] OrderStatus newStatus)
        {
            try
            {
                await _orderRepository.UpdateOrderStatusesAsync(orderIds, newStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order statuses");
                return StatusCode(500, "An error occurred while updating order statuses");
            }
        }

        [HttpGet("analytics")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<OrderAnalytics>> GetOrderAnalytics(
            [FromQuery] string startDate = null,
            [FromQuery] string endDate = null)
        {
            try
            {
                DateTime? startDateTime = null;
                DateTime? endDateTime = null;

                if (!string.IsNullOrEmpty(startDate))
                {
                    if (DateTime.TryParse(startDate, out DateTime parsedStartDate))
                    {
                        startDateTime = DateTime.SpecifyKind(parsedStartDate.Date, DateTimeKind.Utc);
                    }
                    else
                    {
                        return BadRequest("Invalid start date format. Please use YYYY-MM-DD format.");
                    }
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    if (DateTime.TryParse(endDate, out DateTime parsedEndDate))
                    {
                        endDateTime = DateTime.SpecifyKind(parsedEndDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                    }
                    else
                    {
                        return BadRequest("Invalid end date format. Please use YYYY-MM-DD format.");
                    }
                }

                var analytics = await _orderService.GetOrderAnalyticsAsync(startDateTime, endDateTime);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order analytics");
                return StatusCode(500, "An error occurred while retrieving order analytics");
            }
        }
    }
} 