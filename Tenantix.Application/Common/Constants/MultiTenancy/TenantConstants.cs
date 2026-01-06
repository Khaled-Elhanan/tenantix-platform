using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Common.Constants.Tenancy
{
    public static class TenancyConstants
    {

        public const int DefaultTenantValidityInYears = 1;

        public static class TenantTypes
        {
            public const string Root = "Root";
            public const string Store = "Store";
        }

        public static class TenantPolicies
        {
            public const string PlatformTenantOnly = "TenantType.PlatformOnly";
            public const string StoreTenantOnly = "TenantType.StoreOnly";
        }

        public static class Root
        {
            public const string Id = "root";
            public const string Identifier = "root";
            public const string Name = "System";
            public const string AdminEmail = "DeathNote@gamil.com";
            public const string DefaultPassword = "Ghost123";
            public const bool IsActive = true;
        }
    }
}                                                               
