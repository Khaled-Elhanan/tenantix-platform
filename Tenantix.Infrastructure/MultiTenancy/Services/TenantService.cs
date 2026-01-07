using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Tenantix.Application.Features.Tenancy;
using Tenantix.Shared.Exceptions;
using Mapster;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Application.Common.Constants.MultiTenancy;
namespace Tenantix.Infrastructure.MultiTenancy.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<ApplicationTenantInfo> _tenantStore;
        private readonly ApplicationDbSeeder _dbSeeder;
        private readonly IServiceProvider _serviceProvider;


        public TenantService(IMultiTenantStore<ApplicationTenantInfo> tenantStore, ApplicationDbSeeder dbSeeder, IServiceProvider serviceProvider)
        {
            _tenantStore = tenantStore;
            _dbSeeder = dbSeeder;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> ActivateTenantAsync(string id)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(id);
            tenantInDb.IsActive = true;
            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }

        public async Task<string> CreateTenantAsync(
        CreateTenantRequest createTenant,
        CancellationToken cancellationToken)
        {
            // Check duplicate identifier
            if (!string.IsNullOrWhiteSpace(createTenant.Identifier))
            {
                var existingTenant = await _tenantStore.TryGetAsync(createTenant.Identifier);
                if (existingTenant != null)
                {
                    throw new ConflictException(
                        new List<string> { $"A tenant with identifier '{createTenant.Identifier}' already exists." },
                        HttpStatusCode.Conflict);
                }
            }

            // ?? Handle connection string properly
            var connectionString =
                string.IsNullOrWhiteSpace(createTenant.ConnectionString) ||
                createTenant.ConnectionString == "string"
                    ? _serviceProvider
                        .GetRequiredService<IConfiguration>()
                        .GetConnectionString("DefaultConnection")
                    : createTenant.ConnectionString;

            var newTenant = new ApplicationTenantInfo
            {
                Id = string.IsNullOrWhiteSpace(createTenant.Identifier)
                    ? Guid.NewGuid().ToString()
                    : createTenant.Identifier,

                Identifier = createTenant.Identifier,
                IsActive = createTenant.IsActive,
                Name = createTenant.Name,
                ConnectionString = connectionString,
                OwnerEmail = createTenant.AdminEmail,
                TenantType = TenancyConstants.TenantTypes.Store,
                ValidUpTo = createTenant.ValidUpTo == default ? DateTime.UtcNow.AddYears(1) : createTenant.ValidUpTo
            };

            await _tenantStore.TryAddAsync(newTenant);

            // IMPORTANT: run seeder INSIDE tenant context
            using var scope = _serviceProvider.CreateScope();

            var tenantContextSetter =
                scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();

            tenantContextSetter.MultiTenantContext =
                new MultiTenantContext<ApplicationTenantInfo>
                {
                    TenantInfo = newTenant
                };

            await scope.ServiceProvider
                .GetRequiredService<ApplicationDbSeeder>()
                .InitializeDatabaseAsync(cancellationToken);

            return newTenant.Identifier;
        }




        public async Task<string> DeactivateTenantAsync(string tenantId)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(tenantId);
            tenantInDb.IsActive = false;
            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;

        }
        public async Task<TenantResponse> GetTenantByIdAsync(string tenantId)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(tenantId);



            // Using Mapster
            return tenantInDb.Adapt<TenantResponse>();


        }


        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            var tenantsInDb = await _tenantStore.GetAllAsync();
            return tenantsInDb.Adapt<List<TenantResponse>>();
        }

        public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
        {
            var tenantInDb = await _tenantStore.TryGetAsync(updateTenantSubscription.TenantId);
            tenantInDb.ValidUpTo = updateTenantSubscription.NewExpiryDate;
            await _tenantStore.TryUpdateAsync(tenantInDb);
            return tenantInDb.Identifier;
        }

    }
}
