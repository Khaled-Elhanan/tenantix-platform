using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Common;

namespace Tenantix.Domain.Entities
{
    public class Category :AuditableEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int? DisplayOrder { get; set; }
        }
}
