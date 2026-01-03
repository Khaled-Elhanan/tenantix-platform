using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Common.Constants.Authorization
{
    public class StoreActions
    {
        public const string Read = nameof(Read);
        public const string Create = nameof(Create);
        public const string Write = nameof(Write);
        public const string Delete = nameof(Delete);
        public const string Update = nameof(Update);


        public const string Manage = nameof(Manage);
        public const string Upgrade = nameof(Upgrade);


        public const string RefreshToken = nameof(RefreshToken);
       
    }
}
