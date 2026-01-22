using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared.Configurations.Core
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products","Core");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired();

         
            builder.HasIndex(x => new { x.TenantId, x.SKU })
                   .IsUnique();

            // Note: Query filter (TenantId + IsActive) is applied globally in BaseDbContext
            // product & category
            builder.Property(p => p.CategoryId);
               
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p=>p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }          
    }
}
