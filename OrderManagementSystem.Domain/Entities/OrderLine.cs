using OrderManagementSystem.Domain.Common;

namespace OrderManagementSystem.Domain.Entities
{
    public class OrderLine : AuditableEntity
	{
        public Guid OrderLineId { get; set; }
        public Guid OrderId { get; set; }        
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
		public virtual Order Order { get; set; }
	}
} 