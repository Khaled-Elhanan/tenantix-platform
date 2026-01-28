using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Carts.DTOs
{
    public class CartSummaryResponse
    {
        public Guid CustomerId { get; set; }
        public int TotalItems { get; set; }     // sum of Quantity
        public decimal SubTotal { get; set; }       // sum (Product.Price * Quantity)
    }
}
