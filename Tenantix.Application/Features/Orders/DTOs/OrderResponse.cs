using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Orders.DTOs
{
    public class OrderResponse
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = default!;

        public Guid CustomerId { get; set; }

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public string? Notes { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }

        public List<OrderItemResponse> OrderItems { get; set; } = new();
    }
    public class OrderItemResponse
    {
        
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

    }
}
