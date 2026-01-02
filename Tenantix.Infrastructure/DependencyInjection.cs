
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenantix.Infrastructure.MultiTenancy;
using Tenantix.Infrastructure.Persistence.Shared;
using Tenantix.Infrastructure.Persistence.Tenant;
using Tenantix.Infrastructure.Identity.Models;

using Finbuckle.MultiTenant.Strategies;
using Finbuckle.MultiTenant;
using Tenantix.Application.Common.Constants.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;

namespace Tenantix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration config)
    {
        return services
            .AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure()))
            .AddMultiTenant<ApplicationTenantInfo>()
            .WithHeaderStrategy(ClaimConstants.Tenant)
            .WithClaimStrategy(ClaimConstants.Tenant)
            .WithEFCoreStore<TenantDbContext, ApplicationTenantInfo>()
            .Services
            .AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure()))
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddIdentityService();
            

    }

    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider , CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }

    internal static IServiceCollection AddIdentityService(this IServiceCollection service)
    {
        return service
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .Services;
          
    }
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseAuthentication();
         
       

        
    }
}

