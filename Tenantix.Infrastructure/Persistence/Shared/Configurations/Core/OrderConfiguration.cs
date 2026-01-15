using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared.Configurations.Core
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "Core");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .IsRequired();

            builder.Property(o => o.AddressLine)
                   .HasMaxLength(250);

            builder.Property(o => o.City)
                   .HasMaxLength(100);

            builder.Property(o => o.Phone)
                   .HasMaxLength(20);

            builder.HasMany(o => o.OrderItems)
                   .WithOne()
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
