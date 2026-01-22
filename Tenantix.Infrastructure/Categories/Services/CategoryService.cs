using Mapster;
using Microsoft.EntityFrameworkCore;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Models;

namespace Tenantix.Infrastructure.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            // prevent duplicate per tenant here 
            var exists = await _context.Categories.
                AnyAsync(x=>x.Name==request.Name && x.IsActive, cancellationToken);
            if(exists)
            {
              throw new InvalidOperationException("Category name already exists.");
            }
            var category = request.Adapt<Category>();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
         
        }
        public async Task<PagedResponse<CategoryResponse>> GetPagedAsync(int page, int pageSize, CancellationToken cancellation)
        {
           var query= _context.Categories.AsNoTracking()
                .Where(x=>x.IsActive).OrderBy(x=>x.DisplayOrder?? int.MaxValue)
                .ThenByDescending(x=>x.CreatedAt);

            var totalCount = await query.CountAsync(cancellation);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x=>new CategoryResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    DisplayOrder = x.DisplayOrder,
                    CreatedAt = x.CreatedAt
                }).ToListAsync(cancellation);

            return new PagedResponse<CategoryResponse>
            {
                Items=items,
                TotalCount= totalCount,
                Page= page,
                PageSize= pageSize
            };
        }

        public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Categories.AsNoTracking()
              .Where(x => x.Id == id && x.IsActive)
              .Select(x => new CategoryResponse
              {
                  Id = x.Id,
                  Name = x.Name,
                  Description = x.Description,
                  DisplayOrder = x.DisplayOrder,
                  CreatedAt = x.CreatedAt
              }).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
           var category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id==id && x.IsActive,cancellationToken);
            if (category is null) return false;
            // prevent duplicates per tenant
            var exists = await _context.Categories.AnyAsync(
                x => x.Id != id && x.Name == request.Name && x.IsActive,
                cancellationToken);
            if (exists)
                throw new InvalidOperationException("Category name already exists.");
            category.Name = request.Name.Trim();
            category.Description = request.Description;
            category.DisplayOrder = request.DisplayOrder;
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x=>x.Id==id && x.IsActive,cancellationToken);
            if (category is null) return false;
            category.IsActive=false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

       

        

       
    }
}
