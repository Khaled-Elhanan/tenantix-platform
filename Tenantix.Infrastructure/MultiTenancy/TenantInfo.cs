using Finbuckle.MultiTenant.Abstractions;

namespace Tenantix.Infrastructure.MultiTenancy
{
    public class TenantInfo : ITenantInfo
    {
        public string Id { get; set; } = default!;
        public string Identifier { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime? ValidUpTo { get; set; }
    }
}

