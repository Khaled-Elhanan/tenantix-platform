
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenantix.Infrastructure.Identity.Models;

namespace Tenantix.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleClaimConfig
    : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable("RoleClaims", "Identity");

        builder.Property(x => x.ClaimType)
            .HasMaxLength(150);

        builder.Property(x => x.ClaimValue)
            .HasMaxLength(250);
    }
}
