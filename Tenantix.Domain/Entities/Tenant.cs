using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
        public DateTime? ValidUpTo { get; set; }
        public bool IsActive { get; set; }

    }
}
