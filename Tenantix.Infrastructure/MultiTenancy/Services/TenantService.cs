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
using Tenantix.Application.Features.Tenants.DTOs;

namespace Tenantix.Infrastructure.MultiTenancy.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMultiTenantStore<ApplicationTenantInfo> _tenantStore;
        private readonly IServiceProvider _serviceProvider;

        public TenantService(
            IMultiTenantStore<ApplicationTenantInfo> tenantStore,
            IServiceProvider serviceProvider)
        {
            _tenantStore = tenantStore;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> ActivateTenantAsync(string tenantIdOrIdentifier)
        {
            var tenantInDb = await FindTenantAsync(tenantIdOrIdentifier);

            if (tenantInDb.ValidUpTo < DateTime.UtcNow)
            {
                throw new ConflictException(
                    new List<string> { "Tenant subscription is expired. Please upgrade the subscription first." },
                    HttpStatusCode.Conflict);
            }

            tenantInDb.IsActive = true;
            await _tenantStore.TryUpdateAsync(tenantInDb);

            return tenantInDb.Identifier!;
        }

        public async Task<string> DeactivateTenantAsync(string tenantIdOrIdentifier)
        {
            var tenantInDb = await FindTenantAsync(tenantIdOrIdentifier);

            tenantInDb.IsActive = false;
            await _tenantStore.TryUpdateAsync(tenantInDb);

            return tenantInDb.Identifier!;
        }

        public async Task<TenantResponse> GetTenantByIdAsync(string tenantIdOrIdentifier)
        {
            var tenantInDb = await FindTenantAsync(tenantIdOrIdentifier);
            return tenantInDb.Adapt<TenantResponse>();
        }

        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            var tenantsInDb = await _tenantStore.GetAllAsync();
            return tenantsInDb.Adapt<List<TenantResponse>>();
        }

        public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscription)
        {
            var tenantInDb = await FindTenantAsync(updateTenantSubscription.TenantId);

            tenantInDb.ValidUpTo = updateTenantSubscription.NewExpiryDate;

            // Optional: auto-reactivate after upgrade
            if (tenantInDb.ValidUpTo >= DateTime.UtcNow)
                tenantInDb.IsActive = true;

            await _tenantStore.TryUpdateAsync(tenantInDb);

            return tenantInDb.Identifier!;
        }

        public async Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken cancellationToken)
        {
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

            var connectionString =
                string.IsNullOrWhiteSpace(createTenant.ConnectionString) ||
                createTenant.ConnectionString == "string"
                    ? _serviceProvider
                        .GetRequiredService<IConfiguration>()
                        .GetConnectionString("DefaultConnection")
                    : createTenant.ConnectionString;

            var generatedId = string.IsNullOrWhiteSpace(createTenant.Identifier)
                ? Guid.NewGuid().ToString()
                : createTenant.Identifier;

            var newTenant = new ApplicationTenantInfo
            {
                Id = generatedId,
                Identifier = generatedId,
                IsActive = createTenant.IsActive,
                Name = createTenant.Name,
                ConnectionString = connectionString,
                OwnerEmail = createTenant.OwnerEmail,
                CompanyName = createTenant.CompanyName,
                TenantType = TenancyConstants.TenantTypes.Store,
                ValidUpTo = createTenant.ValidUpTo == default
                    ? DateTime.UtcNow.AddYears(1)
                    : createTenant.ValidUpTo
            };

            await _tenantStore.TryAddAsync(newTenant);

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

            return newTenant.Identifier!;
        }

        private async Task<ApplicationTenantInfo> FindTenantAsync(string tenantIdOrIdentifier)
        {
            // 1) Try by identifier (Finbuckle default)
            var tenant = await _tenantStore.TryGetAsync(tenantIdOrIdentifier);
            if (tenant != null)
                return tenant;

            // 2) Fallback: search by Id
            var all = await _tenantStore.GetAllAsync();
            tenant = all.FirstOrDefault(t =>
                string.Equals(t.Id, tenantIdOrIdentifier, StringComparison.OrdinalIgnoreCase));

            if (tenant is null)
            {
                throw new NotFoundException(new List<string> { "Tenant not found." });
            }

            return tenant;
        }
    }
}
