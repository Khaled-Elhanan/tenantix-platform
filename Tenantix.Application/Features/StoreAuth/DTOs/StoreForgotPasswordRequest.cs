using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Features.StoreAuth.DTOs
{
    public class StoreForgotPasswordRequest
    {
        public string Email { get; set; } = default!;
    }
}
