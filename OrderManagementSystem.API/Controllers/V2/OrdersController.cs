using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Models;

namespace OrderManagementSystem.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
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
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null)
        {
            try
            {
                var (orders, totalCount) = await _orderRepository.GetPagedOrdersAsync(pageNumber, pageSize);
                
                // V2 enhancement: Filter by status if provided
                if (status.HasValue)
                {
                    orders = orders.Where(o => o.Status == status.Value);
                }

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }

        [HttpGet("analytics")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<OrderAnalytics>> GetOrderAnalytics(
            [FromQuery] string startDate = null,
            [FromQuery] string endDate = null,
            [FromQuery] OrderStatus? status = null)
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