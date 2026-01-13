using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Customers.DTOs;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface ICustomerService
    {
        Task<Guid> CreateAsync(CreateCustomerRequest  request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest  request, CancellationToken cancellationToken);

        
        Task<PagedResponse<CustomerListItemResponse>> GetPagedAsync(int page,int pageSize,CancellationToken cancellationToken);
        Task<CustomerResponse?> GetByIdAsync(Guid id,CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    }
}
