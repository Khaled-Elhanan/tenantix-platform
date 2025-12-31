using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Domain.Entities
{
    public class Store
    {
       public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!; 

        public string Currency { get; set; } = "EGY";
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
