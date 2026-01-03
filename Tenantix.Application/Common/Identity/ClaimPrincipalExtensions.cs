using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Tenantix.Application.Common.Constants.Authorization;

namespace Tenantix.Application.Common.Identity
{
    public static class ClaimPrincipalExtensions
    {
        public static string? GetEmail(this ClaimsPrincipal principal)
           => principal.FindFirst(ClaimTypes.Email)?.Value;
        public static string GetUserId(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        // this custom claim 
        public static string GetTenant(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimConstants.Tenant)?.Value ?? string.Empty;

        public static string GetFirstName(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        public static string GetLastName(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;

        public static string GetPhoneNumber(this ClaimsPrincipal principal)
            => principal.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty;

    }
}
