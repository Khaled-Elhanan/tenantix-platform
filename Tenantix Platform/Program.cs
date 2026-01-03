using Tenantix.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddOpenApiDocumentation(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));


var app = builder.Build();

await app.Services.AddDatabaseInitializerAsync();


// NSwag OpenAPI documentation is configured in UseInfrastructure()

app.UseInfrastructure();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
