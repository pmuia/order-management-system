using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Models;

namespace OrderManagementSystem.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<OrderSummary>> GetOrderSummariesAsync();
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedOrdersAsync(int pageNumber, int pageSize);
        Task UpdateOrderStatusesAsync(IEnumerable<Guid> orderIds, OrderStatus newStatus);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetAverageOrderValueAsync();
        Task<TimeSpan> GetAverageFulfillmentTimeAsync();
    }
} 