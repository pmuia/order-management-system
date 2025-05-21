using System;
using System.Threading.Tasks;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Entities;

namespace OrderManagementSystem.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<OrderAnalytics> GetOrderAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
} 