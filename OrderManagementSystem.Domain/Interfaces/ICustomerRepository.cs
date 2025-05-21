using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enum;

namespace OrderManagementSystem.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        Task<Customer> GetByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetBySegmentAsync(CustomerSegment segment);
        Task UpdateCustomerSegmentAsync(Guid customerId, CustomerSegment newSegment);
    }
} 