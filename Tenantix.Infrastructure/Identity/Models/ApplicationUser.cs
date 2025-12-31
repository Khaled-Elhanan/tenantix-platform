using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string TenantId { get; set; } = default!;

      
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        
        public bool IsActive { get; set; } = true;
    }
}
