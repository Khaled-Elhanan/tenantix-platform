using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenantix.Application.Common.Constants.MultiTenancy;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Infrastructure.MultiTenancy.Persistence;
using Tenantix.Application.Common.Constants.MultiTenancy;
namespace Tenantix.Infrastructure.MultiTenancy.Seeders
{
    public class TenantDbSeeder : ITenantDbSeeder
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public TenantDbSeeder(
            TenantDbContext tenantDbContext,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _tenantDbContext = tenantDbContext;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            await InitializeTenantDbAsync(cancellationToken);

            var tenants = await _tenantDbContext.TenantInfo
                .Where(t => t.IsActive && t.ValidUpTo > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var tenant in tenants)
            {
                if (tenant.TenantType == TenancyConstants.TenantTypes.Store)
                {
                    await InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
                }
               
            }
        }

        private async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
        {
            await _tenantDbContext.Database.MigrateAsync(cancellationToken);

            var rootTenant = await _tenantDbContext.TenantInfo.FindAsync(
                new object[] { TenancyConstants.Root.Id },
                cancellationToken);

            if (rootTenant is null)
            {
                var defaultConnectionString =
                    _configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection is missing");

                rootTenant = new ApplicationTenantInfo
                {
                    Id = TenancyConstants.Root.Id,
                    Identifier = TenancyConstants.Root.Identifier,
                    Name = TenancyConstants.Root.Name,
                    OwnerEmail = TenancyConstants.Root.AdminEmail,
                    CompanyName = "System",
                    ConnectionString = defaultConnectionString,
                    IsActive = true,
                    TenantType = TenancyConstants.TenantTypes.Platform,
                    ValidUpTo = DateTime.UtcNow.AddYears(
                        TenancyConstants.DefaultTenantValidityInYears)
                };

                await _tenantDbContext.TenantInfo.AddAsync(rootTenant, cancellationToken);
                await _tenantDbContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var updated = false;

                if (string.IsNullOrWhiteSpace(rootTenant.ConnectionString))
                {
                    rootTenant.ConnectionString =
                        _configuration.GetConnectionString("DefaultConnection");
                    updated = true;
                }

                if (string.IsNullOrWhiteSpace(rootTenant.TenantType))
                {
                    rootTenant.TenantType = TenancyConstants.TenantTypes.Platform;
                    updated = true;
                }

                if (updated)
                {
                    await _tenantDbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }

        private async Task InitializeApplicationDbForTenantAsync(
            ApplicationTenantInfo currentTenant,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(currentTenant.ConnectionString))
            {
                currentTenant.ConnectionString =
                    _configuration.GetConnectionString("DefaultConnection");

                await _tenantDbContext.SaveChangesAsync(cancellationToken);
            }

            using var scope = _serviceProvider.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<IMultiTenantContextSetter>()
                .MultiTenantContext = new MultiTenantContext<ApplicationTenantInfo>
                {
                    TenantInfo = currentTenant
                };


                    await scope.ServiceProvider
                    .GetRequiredService<ApplicationDbSeeder>()
                    .InitializeDatabaseAsync(cancellationToken);
                                                                                                         
        }
    }
}
