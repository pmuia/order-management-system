using System;
using OrderManagementSystem.Domain.Common;
using OrderManagementSystem.Domain.Enum;

namespace OrderManagementSystem.Domain.Entities
{
    public class Promotion : AuditableEntity
    {
        public Guid PromotionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PromotionType Type { get; set; }
        public decimal Value { get; set; }
        public CustomerSegment? RequiredSegment { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
} 