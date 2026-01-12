using Tenantix.Application.Common.Constants.Authorization.Common;

namespace Infrastructure.OpenApi;

public sealed class TenantHeaderAttribute : SwaggerHeaderAttribute
{
    public TenantHeaderAttribute()
        : base(
            ClaimConstants.Tenant,
            "Enter your tenant name to access this API.",
            string.Empty,
            true)
    {
    }
}
