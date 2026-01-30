using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Common;

namespace Tenantix.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public string Name { get; set; }=default!;
        public decimal Price { get; set; }
        public int Stock { get; set;  }
        public string SKU { get; set; }=default!;

        // Optimistic concurrency token (prevents overselling / lost updates)
        public byte[] RowVersion { get; set; } = default!;

        public Guid? CategoryId { get; set; }

        public Category? Category { get; set; }

      
      
    }
}
