using Tenantix.Application.Common.Interfaces;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Domain.Entities;
using Mapster;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Shared.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var product= await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
            if(product is null)
            {
                return null;
            }
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Stock = product.Stock
            };
        }

        public async Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking().OrderByDescending(x => x.CreatedAt);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip(page - 1).Take(pageSize)
                .Select(x => new ProductListItemResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Stock=x.Stock,
                    IsActive = x.IsActive
                }).ToListAsync(cancellationToken);
            return new PagedResponse<ProductListItemResponse>
            {
                Items = items,
                Page = page,
                TotalCount = totalCount,
                PageSize = pageSize

            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x=>x.Id == id , cancellationToken);
            if (product == null)
            {
                return false;
            }
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
