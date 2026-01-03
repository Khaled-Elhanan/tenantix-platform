using System.Security.Claims;
using Tenantix.Application.Common.Constants.Authorization;

namespace Tenantix.Application.Features.Identity

{
    public static class ClaimPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal principal)
           => principal.FindFirstValue(ClaimTypes.Email);
        public static string ? GetUserId(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.NameIdentifier)  ?? throw new UnauthorizedAccessException("UserId claim is missing");

        // this custom claim 
        public static string? GetTenant(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimConstants.Tenant);

        public static string GetFirstName(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Name);
                                                                                                 
        public static string GetLastName(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Surname);

       

    }
}
