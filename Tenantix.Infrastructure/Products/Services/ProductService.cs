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
        {  if(request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories.AnyAsync(
                x => x.Id == request.CategoryId.Value && x.IsActive,
                cancellationToken);

                if (!categoryExists)
                {
                    throw new ArgumentException("Category not found.");
                }
            }
            var product = request.Adapt<Product>();
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken); 
            return product.Id;
        }

     

        public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await (from product in _context.Products
                               where product.Id == id
                               join category in _context.Categories on product.CategoryId equals category.Id into categoryGroup
                               from category in categoryGroup.DefaultIfEmpty()
                               select new ProductResponse
                               {
                                   Id = product.Id,
                                   Name = product.Name,
                                   SKU = product.SKU,
                                   Price = product.Price,
                                   Stock = product.Stock,
                                   CategoryId = product.CategoryId,
                                   CategoryName = category != null ? category.Name : null
                               })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            return result;
        }

        public async Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var productsQuery = _context.Products.OrderByDescending(x => x.CreatedAt);
            var totalCount = await productsQuery.CountAsync(cancellationToken);
            
            var items = await (from product in productsQuery
                              join category in _context.Categories on product.CategoryId equals category.Id into categoryGroup
                              from category in categoryGroup.DefaultIfEmpty()
                              select new ProductListItemResponse
                              {
                                  Id = product.Id,
                                  Name = product.Name,
                                  Price = product.Price,
                                  Stock = product.Stock,
                                  IsActive = product.IsActive,
                                  CategoryId = product.CategoryId,
                                  CategoryName = category != null ? category.Name : null
                              })
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            
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
            if(request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories.AnyAsync(
                 x => x.Id == request.CategoryId.Value && x.IsActive,
                 cancellationToken);

                if (!categoryExists)
                {
                    throw new ArgumentException("Category not found.");
                }
            }
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
           
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


        public async Task<bool> DeleteAsync(Guid id,  CancellationToken cancellationToken)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (product == null)
            {
                return false;   
            }
            product.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;    
        }
    }
}
