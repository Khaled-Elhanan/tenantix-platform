using Microsoft.AspNetCore.Authorization;

namespace Tenantix.Infrastructure.Identity.Auth;

public sealed class TenantTypeRequirement : IAuthorizationRequirement
{
    public TenantTypeRequirement(string requiredTenantType)
    {
        RequiredTenantType = requiredTenantType;
    }

    public string RequiredTenantType { get; }
}

