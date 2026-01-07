using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tenantix.Domain.Common;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy.Models;

namespace Tenantix.Infrastructure.Persistence.Context
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
        private new ApplicationTenantInfo? TenantInfo { get; set; }

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
                optionsBuilder.UseSqlServer(
                    TenantInfo.ConnectionString,
                    options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            }
        }

      
        private void ApplyTenantIds()
        {
        
            var currentTenantId = TenantInfo?.Id;
            if (string.IsNullOrWhiteSpace(currentTenantId))
            {
                return;
            }

            var entries = ChangeTracker.Entries<TenantEntity>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                // Only set TenantId if the entity hasn't explicitly set it.
                if (string.IsNullOrWhiteSpace(entry.Entity.TenantId))
                {
                    entry.Entity.TenantId = currentTenantId!;
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyTenantIds();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyTenantIds();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
