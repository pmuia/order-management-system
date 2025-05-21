namespace OrderManagementSystem.Domain.Common
{
	public abstract class AuditableEntity
	{
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
		public required string CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public byte RecordStatus { get; set; }
	}
}
