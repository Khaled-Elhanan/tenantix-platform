
using Finbuckle.MultiTenant;
using Tenantix.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Tenantix.Application;
using Tenantix.Application.Common.Constants.Authorization;
using Tenantix.Infrastructure.Identity.Auth;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Shared.Responses;
using NSwag;
using NSwag.Generation.Processors.Security;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Infrastructure.Identity.Security;
using Tenantix.Infrastructure.Mappings;
using Mapster;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Infrastructure.MultiTenancy.Persistence;
using Tenantix.Infrastructure.MultiTenancy.Seeders;
using Tenantix.Infrastructure.MultiTenancy.Services;
using Tenantix.Application.Common.Constants.MultiTenancy;
using Tenantix.Infrastructure.Products.Services;
using Tenantix.Infrastructure.Customers.Services;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Common.Constants.Authorization.Common;
using Tenantix.Infrastructure.Orders;

namespace Tenantix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration config)
    {
        // Configure Mapster
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // Scans the assembly and gets the IRegister, adding the registration to the TypeAdapterConfig
        typeAdapterConfig.Scan(typeof(MapsterConfig).Assembly);
        // register the mapper as IMapper
        services.AddMapster();

        return services
            .AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure()))
            .AddMultiTenant<ApplicationTenantInfo>()
            .WithClaimStrategy(ClaimConstants.Tenant)
            .WithHeaderStrategy(ClaimConstants.Tenant)
            
            .WithEFCoreStore<TenantDbContext, ApplicationTenantInfo>()
            .Services
            .AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure()))
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddTransient<ITokenService, TokenService>()
            .AddTransient<ITenantService, TenantService>()  
            .AddTransient<IProductService, ProductService>()  
            .AddTransient<ICustomerService,CustomerService>()
            .AddTransient<IOrderService, OrderService>()
            .AddPermissions()
            .AddOpenApiDocumentation(config)
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

    internal static IServiceCollection AddPermissions(this IServiceCollection service)
    {
    
        return service
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddScoped<IAuthorizationHandler, TenantTypeAuthorizationHandler>();

    }

    public static JwtSettings GetJwtSettings(this IServiceCollection services , IConfiguration configuration)
    {
        var jwtSettingsConfig = configuration.GetSection(nameof(JwtSettings));
        services.Configure<JwtSettings>(jwtSettingsConfig);
        return jwtSettingsConfig.Get<JwtSettings>() ?? new JwtSettings();
    }   


    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services , JwtSettings jwtSettings)
    {
       var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);
        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(bearer =>
        {
            bearer.RequireHttpsMetadata = false;
            bearer.SaveToken = true;
            bearer.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
            bearer.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired."));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    }
                    else
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred"));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;

                    }
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                        return context.Response.WriteAsync(result);
                    }
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    }
                    return Task.CompletedTask;
                }

            };

        });
        services
            .AddAuthorization(options =>
            {
                // Store permissions
                AddPermissionPolicies(options, typeof(StorePermissions));

                // Platform permissions
                AddPermissionPolicies(options, typeof(PlatformPermissions));

                // Tenant type separation policies
                options.AddPolicy(TenancyConstants.TenantPolicies.StoreTenantOnly,
                    policy => policy.Requirements.Add(
                        new TenantTypeRequirement(TenancyConstants.TenantTypes.Store)));

                options.AddPolicy(TenancyConstants.TenantPolicies.PlatformTenantOnly,
                    policy => policy.Requirements.Add(
                        new TenantTypeRequirement(TenancyConstants.TenantTypes.Platform)));
            });
        return services;
    }

    private static void AddPermissionPolicies(AuthorizationOptions options, Type permissionsRootType)
    {
        foreach (var prop in permissionsRootType.GetNestedTypes()
                     .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is string name && !string.IsNullOrWhiteSpace(name))
            {
                options.AddPolicy(name,
                    policy => policy.RequireClaim(ClaimConstants.Permissions, name));
            }
        }

        // Support classes exposing a static All property
        var allProp = permissionsRootType.GetProperty("All", BindingFlags.Public | BindingFlags.Static);
        if (allProp?.GetValue(null) is IEnumerable<object> perms)
        {
            foreach (var perm in perms)
            {
                var nameProp = perm.GetType().GetProperty("Name");
                var name = nameProp?.GetValue(perm)?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    options.AddPolicy(name,
                        policy => policy.RequireClaim(ClaimConstants.Permissions, name));
                }
            }
        }
    }


    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services,
            IConfiguration config)
    {
        var swaggerSettings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();
        if (swaggerSettings is null)
        {
            throw new InvalidOperationException("SwaggerSettings configuration section is missing.");
        }

        services.AddEndpointsApiExplorer();

        _ = services.AddOpenApiDocument(options =>
        {
            // Basic document metadata (title, description, contact, license)
            options.PostProcess = doc =>
            {
                doc.Info.Title = swaggerSettings.Title;
                doc.Info.Description = swaggerSettings.Description;
                doc.Info.Contact = new OpenApiContact
                {
                    Name = swaggerSettings.Title,
                    Email = swaggerSettings.ContactEmail,
                    Url = swaggerSettings.ContactUrl
                };

                doc.Info.License = new OpenApiLicense
                {
                    Name = swaggerSettings.LicenseName,
                    Url = swaggerSettings.LicenseUrl,
                };
            };

            // Add JWT bearer security definition so Swagger shows the "Authorize" button
            const string securitySchemeName = "JWT";

            options.AddSecurity(securitySchemeName, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token (without 'Bearer ' prefix).",
                In = OpenApiSecurityApiKeyLocation.Header,
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            // Make all operations use the JWT security scheme by default
            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(securitySchemeName));
            options.OperationProcessors.Add(new SwaggerGlobalAuthProcessor(securitySchemeName));
            options.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi(options =>
        {
            options.DefaultModelExpandDepth = -1;
            options.DocExpansion = "None";
            options.TagsSorter = "alpha";
        });
        return app;
    }


    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseAuthentication()
            .UseMultiTenant()
            .UseAuthorization()
            .UseOpenApiDocumentation();


    }
}

