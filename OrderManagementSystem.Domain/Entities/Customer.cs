using OrderManagementSystem.Domain.Common;
using OrderManagementSystem.Domain.Enum;

namespace OrderManagementSystem.Domain.Entities
{
	public class Customer : AuditableEntity
	{
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CustomerSegment Segment { get; set; }
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
} 