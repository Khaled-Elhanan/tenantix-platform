using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Domain.Common
{
    public abstract class AuditableEntity: TenantEntity
    {
        public DateTime? UpdatedAt { get; set; }
    }
}
