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
        public string OrderNumber { get; set; }

        public string? Notes { get; set; }

        // MVP Address
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public void Confirm()
        {
            if(Status!=OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed.");   
            Status= OrderStatus.Confirmed;
        }
        public void Pack()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed orders can be packed.");
            Status = OrderStatus.Packed;
        }
        public void Ship()
        {
            if (Status != OrderStatus.Packed)
                throw new InvalidOperationException("Only packed orders can be shipped.");
            Status = OrderStatus.Shipped;
        }
        public void Deliver()
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOperationException("Only shipped orders can be delivered.");
            Status = OrderStatus.Delivered;
        }
        public void Cancel()
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel shipped or delivered orders.");

            if (Status == OrderStatus.Cancelled)
                return;

            Status = OrderStatus.Cancelled;
        }

    }
}
