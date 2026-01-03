using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Tenantix.Application.Common.Constants.Authorization;

namespace Tenantix.Infrastructure.Identity.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Check if the user has the required permission claim
            if (context.User.Claims.Any(c => c.Type == ClaimConstants.Permissions && c.Value == requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
