using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.Identity.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string TenantId { get; set; } = default!;

    
        public string? Description { get; set; }
    }
}
