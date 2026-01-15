using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Orders.DTOs
{
    public class CreateOrderRequest
    {
     public Guid  CustomerId { get; set; }   
    public string ? Notes { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public List<CreateOrderItemRequest> OrderItems { get; set; } = new();

    }
   public class CreateOrderItemRequest
    {
        public Guid  ProductId { get; set; }
        public int Quantity { get; set; }   
    }
}
