using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Domain.Entities;


namespace Tenantix.IntegrationTests;

public class TestAppFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test"); 

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            if (!db.Set<Product>().Any())
            {
                db.Set<Product>().AddRange(
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "A1",
                        Price = 10m,
                        Stock = 5,
                        SKU = "SKU-A1",
                        TenantId = "a",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "B1",
                        Price = 10m,
                        Stock = 3,
                        SKU = "SKU-B1",
                        TenantId = "b",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );

                db.SaveChanges();
            }

        });
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
