using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared.Configurations.Core
{
    public class CartConfiguration:IEntityTypeConfiguration<Cart>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(x => x.Id);

            // Multi-tenant safety: ensure one cart per customer per tenant (not globally)
            builder.HasIndex(x => new { x.TenantId, x.CustomerId }).IsUnique();

            builder.HasIndex(x => new { x.TenantId, x.IsActive });

            builder
                .HasMany(x => x.Items)
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
