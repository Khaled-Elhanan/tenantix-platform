using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Products;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IProductService
    {
        Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    }
}
