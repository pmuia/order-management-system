using OrderManagementSystem.Domain.Common;

namespace OrderManagementSystem.Domain.Entities
{
	public class Order : AuditableEntity
	{
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }        
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderLine> OrderItems { get; set; } = new List<OrderLine>();
        public DateTime? FulfillmentDate { get; set; }
        public required string TrackingNumber { get; set; }
		public virtual Customer Customer { get; set; }
	}

    public enum OrderStatus
    {
        Created,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
} 