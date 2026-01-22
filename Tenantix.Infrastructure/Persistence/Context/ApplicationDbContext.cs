using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
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
        
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<Category> Categories => Set<Category>();



    }
}
