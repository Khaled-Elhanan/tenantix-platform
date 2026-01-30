using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
        private new ApplicationTenantInfo? TenantInfo { get; }

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
            var currentTenantId = TenantInfo?.Identifier;
            if (string.IsNullOrWhiteSpace(currentTenantId))
                return;

            var entries = ChangeTracker.Entries<TenantEntity>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Entity.TenantId))
                    entry.Entity.TenantId = currentTenantId!;
            }
        }

        private void ApplyAuditFields()
        {
            var utcNow = DateTime.UtcNow;

            // Ensure CreatedAt is always set server-side
            foreach (var entry in ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Added))
            {
                entry.Entity.CreatedAt = utcNow;
            }

            // Update UpdatedAt for modified auditable entities
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>()
                         .Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditFields();
            ApplyTenantIds();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyAuditFields();
            ApplyTenantIds();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            
            ApplyIsActiveFilter(builder);
        }

        private static void ApplyIsActiveFilter(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null) continue;

                if (!typeof(TenantEntity).IsAssignableFrom(clrType))
                    continue;

                // e =>
                var parameter = System.Linq.Expressions.Expression.Parameter(clrType, "e");

                // EF.Property<bool>(e, "IsActive")
                var isActiveProp = System.Linq.Expressions.Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(bool) },
                    parameter,
                    System.Linq.Expressions.Expression.Constant(nameof(TenantEntity.IsActive))
                );

                // EF.Property<bool>(e,"IsActive") == true
                var body = System.Linq.Expressions.Expression.Equal(
                    isActiveProp,
                    System.Linq.Expressions.Expression.Constant(true)
                );

                var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);

                builder.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }
}
