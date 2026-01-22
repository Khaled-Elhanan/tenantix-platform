using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Entities;

namespace Tenantix.Infrastructure.Persistence.Shared.Configurations.Core
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories","Core");
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Description)
                .HasMaxLength(500).HasMaxLength(150);
            builder.Property(c => c.DisplayOrder).HasMaxLength(500);

            builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            
        }

    }
}
