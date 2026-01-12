using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Constants.Authorization.Store;

namespace Tenantix.Infrastructure.Identity.Auth
{
    public class ShouldHavePermissionAttribute :AuthorizeAttribute
    {
        public ShouldHavePermissionAttribute(string action, string feature)
        {
            Policy = StorePermissions.NameFor(action, feature); 
        }

        public ShouldHavePermissionAttribute(string permissionName)
        {
            Policy = permissionName;
        }
    }
}
