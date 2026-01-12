using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Products.DTOs
{
    public class ProductListItemResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public decimal Price { get; init; }
        public int Stock { get; init; }
        public bool IsActive { get; init; }
    }
}
