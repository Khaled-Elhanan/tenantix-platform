using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Common;

namespace Tenantix.Domain.Entities
{
    public class Cart : TenantEntity
    {
        public Guid CustomerId { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
