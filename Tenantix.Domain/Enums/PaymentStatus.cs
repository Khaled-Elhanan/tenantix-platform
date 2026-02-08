using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Domain.Enums
{
    public enum PaymentStatus
    {
        Initialized = 1,
        Pending = 2,
        Paid = 3,
        Failed = 4,
        Refunded = 5
    }
}
