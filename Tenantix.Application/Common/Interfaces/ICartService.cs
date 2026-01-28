using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Carts.DTOs;

namespace Tenantix.Application.Common.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
        Task AddItemAsync(Guid customerId, AddCartItemRequest item, CancellationToken cancellationToken);
        Task UpdateItemAsync(Guid customerId, UpdateCartItemRequest item, CancellationToken cancellationToken);
        Task ClearAsync(Guid customerId, CancellationToken cancellationToken);
        Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken);
        Task<CartSummaryResponse> GetSummaryAsync(Guid customerId, CancellationToken cancellationToken);



    }
}
