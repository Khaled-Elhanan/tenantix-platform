using Microsoft.AspNetCore.Authorization;
using Tenantix.Application.Common.Constants.Tenancy;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Finbuckle.MultiTenant.Abstractions;

namespace Tenantix.Infrastructure.Identity.Auth;


public sealed class TenantTypeAuthorizationHandler
    : AuthorizationHandler<TenantTypeRequirement>
{
    private readonly IMultiTenantContextAccessor<ApplicationTenantInfo> _tenantAccessor;

    public TenantTypeAuthorizationHandler(
        IMultiTenantContextAccessor<ApplicationTenantInfo> tenantAccessor)
    {
        _tenantAccessor = tenantAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantTypeRequirement requirement)
    {
        var tenantType = _tenantAccessor.MultiTenantContext?.TenantInfo?.TenantType;

        if (!string.IsNullOrWhiteSpace(tenantType) &&
            tenantType.Equals(requirement.RequiredTenantType, StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

