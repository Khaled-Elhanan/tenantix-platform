using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Application
{
    public class JwtSettings
    {
        public string Secret {  get; set; }

        public int TokenExpiryTimeInMinutes { get; set; }
        public int RefreshTokenExpiryTimeInDays { get; set; }
    }
}
