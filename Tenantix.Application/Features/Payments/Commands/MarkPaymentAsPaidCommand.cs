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
    public class MarkPaymentAsPaidCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid PaymentId { get; set; }
    }
    public class MarkPaymentAsPaidCommandHandler
     : IRequestHandler<MarkPaymentAsPaidCommand, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public MarkPaymentAsPaidCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            MarkPaymentAsPaidCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.MarkAsPaidAsync(
                request.PaymentId,
                cancellationToken);

            if (!result)
                return await ResponseWrapper.FailAsync("Payment not found.");

            return await ResponseWrapper.SuccessAsync("Payment marked as paid successfully.");
        }
    }
}
