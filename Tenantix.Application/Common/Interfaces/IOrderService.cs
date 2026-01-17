using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<OrderListItemResponse>> GetPagedAsync(int page , int pageSize , CancellationToken cancellationToken);
        Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
