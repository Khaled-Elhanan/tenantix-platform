using Mapster;
using Tenantix.Application.Features.Tenancy;
using Tenantix.Infrastructure.MultiTenancy;

namespace Tenantix.Infrastructure.Mappings;

/// <summary>
/// Mapster configuration implementing IRegister for dependency injection support.
/// </summary>
public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Configure mapping from ApplicationTenantInfo to TenantResponse
        // Map OwnerEmail (source entity property) to AdminEmail (destination DTO property)
        config.NewConfig<ApplicationTenantInfo, TenantResponse>()
            .Map(dest => dest.AdminEmail, src => src.OwnerEmail);

        // Note: Other properties with matching names will be mapped automatically.
    }
}
