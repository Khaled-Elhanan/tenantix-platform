using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared
{
    public class SharedDbContext : EFCoreStoreDbContext<TenantInfo>
    {
        public SharedDbContext(DbContextOptions<SharedDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TenantInfo>()
                .ToTable("Tenants", "MultiTenancy");
        }
    }


}
