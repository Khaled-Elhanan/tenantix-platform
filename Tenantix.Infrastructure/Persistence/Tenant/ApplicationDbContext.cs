using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Tenantix.Infrastructure.MultiTenancy;

namespace Tenantix.Infrastructure.Persistence.Tenant
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(
            IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
            DbContextOptions<ApplicationDbContext> options)
            : base(tenantContextAccessor, options)
        {
        }
    }
}
