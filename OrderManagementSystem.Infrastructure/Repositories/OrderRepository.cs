using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Models;
using OrderManagementSystem.Infrastructure.Data;

namespace OrderManagementSystem.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string OrderCacheKey = "Order_{0}";
        private const string OrdersListCacheKey = "Orders_List_{0}";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public OrderRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            var cacheKey = string.Format(OrderCacheKey, id);
            
            if (!_cache.TryGetValue(cacheKey, out Order order))
            {
                order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order != null)
                {
                    _cache.Set(cacheKey, order, CacheDuration);
                }
            }
            
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var cacheKey = string.Format(OrdersListCacheKey, "all");
            
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Order> orders))
            {
                orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Customer)
                    .ToListAsync();

                _cache.Set(cacheKey, orders, CacheDuration);
            }
            
            return orders;
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderSummary>> GetOrderSummariesAsync()
        {
            return await _context.Orders
                .Select(o => new OrderSummary
                {
                    Id = o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();
        }

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedOrdersAsync(
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer);

            var totalCount = await query.CountAsync();
            
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task UpdateOrderStatusesAsync(IEnumerable<Guid> orderIds, OrderStatus newStatus)
        {
            var orders = await _context.Orders
                .Where(o => orderIds.Contains(o.OrderId))
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Status = newStatus;
                if (newStatus == OrderStatus.Delivered)
                    order.FulfillmentDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            
            // Invalidate cache
            foreach (var orderId in orderIds)
            {
                _cache.Remove(string.Format(OrderCacheKey, orderId));
            }
            _cache.Remove(string.Format(OrdersListCacheKey, "all"));
        }

        public async Task<Order> AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            _cache.Remove(string.Format(OrdersListCacheKey, "all"));
            
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            _cache.Remove(string.Format(OrderCacheKey, order.OrderId));
            _cache.Remove(string.Format(OrdersListCacheKey, "all"));
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                
                // Invalidate cache
                _cache.Remove(string.Format(OrderCacheKey, id));
                _cache.Remove(string.Format(OrdersListCacheKey, "all"));
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .AverageAsync(o => o.DiscountedAmount);
        }

        public async Task<TimeSpan> GetAverageFulfillmentTimeAsync()
        {
            var deliveredOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered && o.FulfillmentDate.HasValue)
                .ToListAsync();

            if (!deliveredOrders.Any())
                return TimeSpan.Zero;

            var totalFulfillmentTime = deliveredOrders.Sum(o => 
                (o.FulfillmentDate.Value - o.OrderDate).TotalMilliseconds);

            return TimeSpan.FromMilliseconds(totalFulfillmentTime / deliveredOrders.Count);
        }
    }
} 