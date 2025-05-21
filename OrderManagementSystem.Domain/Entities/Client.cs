using System;
using OrderManagementSystem.Domain.Common;
using OrderManagementSystem.Domain.Enum;

namespace OrderManagementSystem.Domain.Entities
{
	public class Client : AuditableEntity
	{
		public Guid ClientId { get; set; }
		public string ApiKey { get; set; }
		public string AppSecret { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? LastAccessAt { get; set; }
		public Roles Role { get; set; }
		public int AccessTokenLifetimeInMins { get; set; } = 60;
		public int AuthorizationCodeLifetimeInMins { get; set; } = 60;
	}
}
