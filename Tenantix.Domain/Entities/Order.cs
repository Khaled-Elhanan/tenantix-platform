using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Common;
using Tenantix.Domain.Enums;

namespace Tenantix.Domain.Entities
{
    public class Order : AuditableEntity
    {
        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        // MVP Address
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    }
}
