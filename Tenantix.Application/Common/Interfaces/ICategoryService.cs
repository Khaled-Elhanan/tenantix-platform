using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface ICategoryService
    {
         Task<Guid>CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
         Task<PagedResponse<CategoryResponse>> GetPagedAsync(int page , int pageSize ,CancellationToken cancellation);
         Task<CategoryResponse?> GetByIdAsync(Guid id  , CancellationToken cancellationToken);

        Task<bool>UpdateAsync(Guid id ,  UpdateCategoryRequest request , CancellationToken cancellationToken);

        Task<bool>DeleteAsync(Guid id , CancellationToken cancellationToken);


    }
}
