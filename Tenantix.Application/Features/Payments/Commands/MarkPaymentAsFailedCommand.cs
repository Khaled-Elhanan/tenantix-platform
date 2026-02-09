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
    public class MarkPaymentAsFailedCommand
        : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid PaymentId { get; set; }
    }
    public class MarkPaymentAsFailedCommandHandler
      : IRequestHandler<MarkPaymentAsFailedCommand, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public MarkPaymentAsFailedCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            MarkPaymentAsFailedCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.MarkAsFailedAsync(
                request.PaymentId,
                cancellationToken);

            if (!result)
                return await ResponseWrapper.FailAsync("Payment not found.");

            return await ResponseWrapper.SuccessAsync("Payment marked as failed successfully.");
        }
    }
}
