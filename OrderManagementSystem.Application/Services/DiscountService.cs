using System;
using System.Threading.Tasks;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;
using OrderManagementSystem.Domain.Interfaces;

namespace OrderManagementSystem.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly ICustomerRepository _customerRepository;

        public DiscountService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<decimal> CalculateDiscountAsync(Order order)
        {
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            if (customer == null)
                return 0;

            var segmentDiscount = await GetCustomerSegmentDiscountAsync(customer.Segment);
            var loyaltyDiscount = await GetLoyaltyDiscountAsync(customer);
            var bulkDiscount = await GetBulkOrderDiscountAsync(order);

            // Apply the highest discount
            return Math.Max(Math.Max(segmentDiscount, loyaltyDiscount), bulkDiscount);
        }

        public Task<decimal> GetCustomerSegmentDiscountAsync(CustomerSegment segment)
        {
            return Task.FromResult(segment switch
            {
                CustomerSegment.Platinum => 0.15m, // 15% discount
                CustomerSegment.Gold => 0.10m,     // 10% discount
                CustomerSegment.Silver => 0.05m,   // 5% discount
                _ => 0m                            // No discount for regular customers
            });
        }

        public Task<decimal> GetLoyaltyDiscountAsync(Customer customer)
        {
            if (customer.LastOrderDate == null)
                return Task.FromResult(0m);

            var monthsSinceLastOrder = (DateTime.UtcNow - customer.LastOrderDate.Value).TotalDays / 30;
            
            // Apply loyalty discount based on customer's total spent and order frequency
            if (customer.TotalSpent > 10000 && monthsSinceLastOrder < 3)
                return Task.FromResult(0.20m); // 20% discount for loyal high-spending customers
            else if (customer.TotalSpent > 5000 && monthsSinceLastOrder < 6)
                return Task.FromResult(0.10m); // 10% discount for regular customers

            return Task.FromResult(0m);
        }

        public Task<decimal> GetBulkOrderDiscountAsync(Order order)
        {
            var totalItems = order.OrderItems.Sum(item => item.Quantity);
            
            return Task.FromResult(totalItems switch
            {
                var n when n >= 20 => 0.25m, // 25% discount for bulk orders
                var n when n >= 10 => 0.15m, // 15% discount for medium orders
                var n when n >= 5 => 0.05m,  // 5% discount for small bulk orders
                _ => 0m                      // No bulk discount
            });
        }
    }
} 