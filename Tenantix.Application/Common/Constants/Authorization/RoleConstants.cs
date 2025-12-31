using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application.Common.Constants.Authorization
{
    public class RoleConstants
    {
        public const string Owner = nameof(Owner);
        public const string Admin = nameof(Admin);
        public const string Staff = nameof(Staff);
        public const string Viewer = nameof(Viewer);

        public static readonly IReadOnlyList<string> DefaultRoles =
            new[] {Owner , Admin, Staff , Viewer};   

    }
}
