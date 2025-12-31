using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenantix.Infrastructure.Identity.Models;

namespace Tenantix.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleConfig
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", "Identity");

        builder.Property(x => x.Description)
            .HasMaxLength(250);
    }
}
