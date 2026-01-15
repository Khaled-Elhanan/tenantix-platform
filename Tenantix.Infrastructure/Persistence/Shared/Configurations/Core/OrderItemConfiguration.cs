using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared.Configurations.Core
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "Core");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.UnitPrice)
                   .HasPrecision(18, 2);

            builder.Property(i => i.LineTotal)
                   .HasPrecision(18, 2);

            builder.Property(i => i.Quantity)
                   .IsRequired();
        }
    }
}
