using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Orders.DTOs
{
    public class OrderListItemResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = default!;
        public DateTime CreateAt { get; set; }
    }
}
