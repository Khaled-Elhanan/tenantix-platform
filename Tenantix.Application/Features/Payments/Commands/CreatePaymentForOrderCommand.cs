using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Domain.Enums;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Payments.Commands
{
    public class CreatePaymentForOrderCommand: IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid OrderId { get; set; }
        public PaymentProvider Provider { get; set; }
    }

    public class CreatePaymentForOrderCommandHandler
        : IRequestHandler<CreatePaymentForOrderCommand, IResponseWrapper>
    {

        private readonly IPaymentService _paymentService;

        public CreatePaymentForOrderCommandHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IResponseWrapper> Handle(CreatePaymentForOrderCommand request, CancellationToken cancellationToken)
        {
            var paymentId = await _paymentService.CreateAsync(request.OrderId, request.Provider, cancellationToken);
            return await ResponseWrapper<Guid>.SuccessAsync(paymentId, "Payment created successfully.");
        }
    }

    }

