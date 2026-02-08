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
    public class InitiatePaymentCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid PaymentId { get; set; }
    }
    public class InitiatePaymentCommandHandler
        : IRequestHandler<InitiatePaymentCommand, IResponseWrapper>
    {
        private readonly IPaymentService _paymentService;

        public InitiatePaymentCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(
            InitiatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            var paymentUrl = await _paymentService.InitiateAsync(
                request.PaymentId,
                cancellationToken);

            return await ResponseWrapper<string>.SuccessAsync(
                paymentUrl,
                "Payment initiated successfully.");
        }
    }
}
