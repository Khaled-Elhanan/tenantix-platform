using Microsoft.AspNetCore.Authorization;
using Tenantix.Application.Common.Constants.MultiTenancy;

namespace Tenantix.Infrastructure.Identity.Auth;

public sealed class StoreTenantOnlyAttribute : AuthorizeAttribute
{
    public StoreTenantOnlyAttribute()
    {
        Policy = TenancyConstants.TenantPolicies.StoreTenantOnly;
    }
}

public sealed class PlatformTenantOnlyAttribute : AuthorizeAttribute
{
    public PlatformTenantOnlyAttribute()
    {
        Policy = TenancyConstants.TenantPolicies.PlatformTenantOnly;
    }
}

