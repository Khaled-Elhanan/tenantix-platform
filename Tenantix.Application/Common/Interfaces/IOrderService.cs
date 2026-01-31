using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Domain.Enums;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
        Task<PagedResponse<OrderListItemResponse>> GetPagedAsync(int page , int pageSize , CancellationToken cancellationToken);
        Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken);


        Task<bool> ConfirmAsync(Guid id, CancellationToken ct);
        Task<bool> PackAsync(Guid id, CancellationToken ct);
        Task<bool> ShipAsync(Guid id, CancellationToken ct);
        Task<bool> DeliverAsync(Guid id, CancellationToken ct);

    }
}
