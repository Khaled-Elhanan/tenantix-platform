using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Domain.Enums;

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



    }

}
