using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.MultiTenancy
{
    public class TenancyConstants
    {
        public const string TenantIdName = "tenant";
        public const string DefaultPassword = "CodeForces1#";
        public const string FirstName = "Khaled";
        public const string LastName = "Abd-Elhanan";

        public static class Root
        {
            public const string Id = "root";
            public const string name = "Root";
            public const string password = "OnceHuman@gamil.com";
        }
        
    }

}
