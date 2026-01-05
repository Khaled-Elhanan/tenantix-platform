using Finbuckle.MultiTenant.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.MultiTenancy.Models
{
    public class ApplicationTenantInfo : ITenantInfo
    {
        public string? Id { get; set; }
        public string? Identifier { get; set; }
        public string? Name { get; set; }

        public string? ConnectionString { get; set; }


        public string? OwnerEmail { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidUpTo { get; set; }
    }
}
