using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Payments.Commands
{
    public class RefundPaymentCommand
        : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid PaymentId { get; set; }
    }
    public class RefundPaymentCommandHandler
      : IRequestHandler<RefundPaymentCommand, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public RefundPaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            RefundPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.RefundAsync(
                request.PaymentId,
                cancellationToken);

            if (!result)
                return await ResponseWrapper.FailAsync("Payment not found.");

            return await ResponseWrapper.SuccessAsync("Payment refunded successfully.");
        }
    }
}
