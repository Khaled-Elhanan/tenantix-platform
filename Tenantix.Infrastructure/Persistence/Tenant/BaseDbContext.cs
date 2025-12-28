using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy;

namespace Tenantix.Infrastructure.Persistence.Tenant
{
    public abstract class BaseDbContext
        : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        protected BaseDbContext(
            IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
            DbContextOptions options)
            : base(tenantContextAccessor, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
