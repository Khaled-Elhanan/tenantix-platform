using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Enums;

namespace Tenantix.Application.Features.Orders.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}
