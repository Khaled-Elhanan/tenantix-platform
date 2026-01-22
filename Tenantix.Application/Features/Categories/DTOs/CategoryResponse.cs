using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Categories.DTOs
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
