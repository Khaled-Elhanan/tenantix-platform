using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Common;

namespace Tenantix.Domain.Entities
{
    public class CartItem : TenantEntity
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }  =  null!;

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
