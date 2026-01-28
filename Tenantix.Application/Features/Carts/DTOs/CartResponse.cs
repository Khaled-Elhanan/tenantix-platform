using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Carts.DTOs
{
    public class CartResponse
    {
        public Guid CustomerId { get; set; }
        public List<CartItemResponse> Items { get; set; } = new();
    }
}
