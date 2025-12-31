using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.MultiTenancy
{
        public class TenantDbContext(DbContextOptions<TenantDbContext> options) :
            EFCoreStoreDbContext<ApplicationTenantInfo>(options)
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<ApplicationTenantInfo>()
                    .ToTable("Tenants", "MultiTenancy");
            }
        }
}
