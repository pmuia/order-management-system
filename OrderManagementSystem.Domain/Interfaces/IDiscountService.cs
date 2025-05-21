using System.Threading.Tasks;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;

namespace OrderManagementSystem.Domain.Interfaces
{
    public interface IDiscountService
    {
        Task<decimal> CalculateDiscountAsync(Order order);
        Task<decimal> GetCustomerSegmentDiscountAsync(CustomerSegment segment);
        Task<decimal> GetLoyaltyDiscountAsync(Customer customer);
        Task<decimal> GetBulkOrderDiscountAsync(Order order);
    }
} 