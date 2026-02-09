using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Payments.DTOs
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = default!;
        public string Provider { get; set; } = default!;
        public string? ExternalReference { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
