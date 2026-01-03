using Microsoft.AspNetCore.Authorization;

namespace Tenantix.Infrastructure.Identity.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
