using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Payments.DTOs;
using Tenantix.Domain.Enums;
using Tenantix.Shared.Models;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<Guid> CreateAsync(
            Guid orderId,
            PaymentProvider provider,
            CancellationToken cancellationToken);     

        Task<bool> MarkAsPaidAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<bool> MarkAsFailedAsync(Guid paymentId, CancellationToken cancellationToken);

        Task<string> InitiateAsync(Guid paymentId, CancellationToken cancellationToken);
        Task<bool> RefundAsync(Guid paymentId, CancellationToken cancellationToken);

        Task<List<PaymentResponse>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);

        Task<PagedResponse<PaymentResponse>> GetPagedAsync(
            int page ,
            int pageSize ,
            CancellationToken cancellationToken);



    }

}
