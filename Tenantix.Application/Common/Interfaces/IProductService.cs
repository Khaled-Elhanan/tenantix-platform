using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IProductService
    {
        Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(int page , int pageSize, CancellationToken cancellationToken);

        Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id,UpdateProductRequest request,CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id,CancellationToken cancellationToken);

    }
}
