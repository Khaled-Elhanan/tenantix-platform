using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.MultiTenancy
{
    public class TenantDbSeeder : ITenantDbSeeder
    {


        public Task IntializeDatabaseAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
