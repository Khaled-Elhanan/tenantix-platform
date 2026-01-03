using Tenantix.Application;
using Tenantix.Infrastructure;
using Tenantix_WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Add services
// =====================

// Controllers
builder.Services.AddControllers();

// Application (MediatR, Application layer stuff)
builder.Services.AddApplication();

// Infrastructure (DbContext, Identity, Finbuckle, Repos, etc.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(
    builder.Services.GetJwtSettings(builder.Configuration)
);

var app = builder.Build();

// =====================
// Initialize databases
// =====================
await app.Services.AddDatabaseInitializerAsync();

// =====================
// Middleware pipeline
// =====================

// Infrastructure middleware
// (Multi-Tenant, Exception handling, Swagger, Auth, etc.)
app.UseInfrastructure();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
