using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Infrastructure.MultiTenancy.Seeders
{
    public interface ITenantDbSeeder
    {
        Task InitializeDatabaseAsync(CancellationToken cancellationToken);
    }
}
