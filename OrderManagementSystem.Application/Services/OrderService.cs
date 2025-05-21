using System;
using System.Threading.Tasks;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Models;

namespace OrderManagementSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDiscountService _discountService;
        private readonly ILoggingService _logger;

        public OrderService(
            IOrderRepository orderRepository, 
            IDiscountService discountService,
            ILoggingService logger)
        {
            _orderRepository = orderRepository;
            _discountService = discountService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Created;
            
            // Calculate total amount
            order.TotalAmount = order.OrderItems.Sum(item => item.TotalPrice);
            
            // Apply discount
            var discount = await _discountService.CalculateDiscountAsync(order);
            order.DiscountedAmount = order.TotalAmount * (1 - discount);

            return await _orderRepository.AddAsync(order);
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(order.OrderId);
            if (existingOrder == null)
                throw new ArgumentException("Order not found", nameof(order.OrderId));

            // Recalculate total amount and discount
            order.TotalAmount = order.OrderItems.Sum(item => item.TotalPrice);
            var discount = await _discountService.CalculateDiscountAsync(order);
            order.DiscountedAmount = order.TotalAmount * (1 - discount);

            await _orderRepository.UpdateAsync(order);
            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found", nameof(orderId));

            if (!IsValidStatusTransition(order.Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");

            order.Status = newStatus;
            
            if (newStatus == OrderStatus.Delivered)
                order.FulfillmentDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return order;
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (OrderStatus.Created, OrderStatus.Processing) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.Created, OrderStatus.Cancelled) => true,
                (OrderStatus.Processing, OrderStatus.Cancelled) => true,
                _ => false
            };
        }

        public async Task<OrderAnalytics> GetOrderAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Retrieving order analytics for date range: {StartDate} to {EndDate}", 
                    startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"));

                var query = _orderRepository.GetOrdersByDateRangeAsync(
                    startDate ?? DateTime.MinValue,
                    endDate ?? DateTime.MaxValue);

                var orders = await query;
                
                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found in the specified date range");
                    return new OrderAnalytics
                    {
                        AverageOrderValue = 0,
                        AverageFulfillmentTime = TimeSpan.Zero
                    };
                }

                var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered && o.FulfillmentDate.HasValue).ToList();

                if (!deliveredOrders.Any())
                {
                    _logger.LogWarning("No delivered orders with fulfillment dates found in the specified date range");
                    return new OrderAnalytics
                    {
                        AverageOrderValue = 0,
                        AverageFulfillmentTime = TimeSpan.Zero
                    };
                }

                try
                {
                    var averageOrderValue = deliveredOrders.Average(o => o.DiscountedAmount);
                    var averageFulfillmentTime = TimeSpan.FromMilliseconds(
                        deliveredOrders.Average(o => (o.FulfillmentDate!.Value - o.OrderDate).TotalMilliseconds));

                    _logger.LogInformation("Successfully calculated analytics: Average Order Value: {Value}, Average Fulfillment Time: {Time}",
                        averageOrderValue, averageFulfillmentTime);

                    return new OrderAnalytics
                    {
                        AverageOrderValue = averageOrderValue,
                        AverageFulfillmentTime = averageFulfillmentTime
                    };
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Error calculating averages from order data");
                    throw new InvalidOperationException("Error calculating averages from order data", ex);
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error retrieving order analytics");
                throw new Exception("Error retrieving order analytics", ex);
            }
        }
    }

    public class OrderAnalytics
    {
        public decimal AverageOrderValue { get; set; }
        public TimeSpan AverageFulfillmentTime { get; set; }
    }
} 