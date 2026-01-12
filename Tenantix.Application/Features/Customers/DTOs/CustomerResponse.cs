using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.Customers.DTOs
{
    public class CustomerResponse
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
