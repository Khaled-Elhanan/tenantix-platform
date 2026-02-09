using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Payments.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Payments.Queries
{
    public class GetPaymentsByOrderIdQuery : IRequest<IResponseWrapper>
    {
        public Guid OrderId { get; set; }
    }
    public class GetPaymentsByOrderIdQueryHandler
    : IRequestHandler<GetPaymentsByOrderIdQuery, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public GetPaymentsByOrderIdQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            GetPaymentsByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            var payments = await _paymentService.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

            return await ResponseWrapper<List<PaymentResponse>>
                .SuccessAsync(payments);
        }
    }
}
