using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Customers.DTOs
{
    public class CustomerListItemResponse
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string? Phone { get; init; }
    }
}
