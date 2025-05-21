using OrderManagementSystem.Domain.Entities;
using System;

namespace OrderManagementSystem.Domain.Models
{
    public class OrderSummary
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
} 