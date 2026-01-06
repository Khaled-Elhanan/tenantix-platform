using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Domain.Common
{
    public abstract class TenantEntity  : BaseEntity
    {
        public string TenantId { get; set; }   = default!;
        public bool IsActive { get; set; } = true;
    }
}
