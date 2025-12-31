using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy;

namespace Tenantix.Infrastructure.Persistence.Tenant
{
    public abstract class BaseDbContext
        : MultiTenantIdentityDbContext<ApplicationUser, 
            ApplicationRole, string,
            IdentityUserClaim<string>,
            IdentityUserRole<string>,
            IdentityUserLogin<string>,
            ApplicationRoleClaim,
            IdentityUserToken<string>>
    {

        private new ApplicationTenantInfo  ? TenantInfo { get; set; }

        protected BaseDbContext(
            IMultiTenantContextAccessor<ApplicationTenantInfo> tenantContextAccessor,
            DbContextOptions options)
            : base(tenantContextAccessor, options)
        {
            TenantInfo = tenantContextAccessor.MultiTenantContext?.TenantInfo;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

           

            if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
            {
                  optionsBuilder.UseSqlServer(TenantInfo.ConnectionString , 
                      options=>options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
