using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Tenantix.Domain.Entities;
using Tenantix.Infrastructure.MultiTenancy.Models;

namespace Tenantix.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(IMultiTenantContextAccessor<ApplicationTenantInfo> tenantInfoContextAccessor , 
            
            DbContextOptions<ApplicationDbContext>options ):
            base(tenantInfoContextAccessor, options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore<ApplicationTenantInfo>();
        }
        public DbSet<Tenant> Tenants => Set<Tenant>();
           
           
        
    }
}
