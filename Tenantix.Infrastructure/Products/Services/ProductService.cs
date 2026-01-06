using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Products;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Domain.Entities;
using Mapster;
namespace Tenantix.Infrastructure.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var product = request.Adapt<Product>();
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken); 
            return product.Id;
        }
    }
}
