using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands.Lifecycle
{
    namespace Tenantix.Application.Features.Orders.Commands
    {
        public class ConfirmOrderCommand : IRequest<IResponseWrapper>, IValidateMe
        {
            public Guid OrderId { get; set; }
        }

        public class ConfirmOrderCommandHandler
            : IRequestHandler<ConfirmOrderCommand, IResponseWrapper>
        {
            private readonly IOrderService _orderService;

            public ConfirmOrderCommandHandler(IOrderService orderService)
            {
                _orderService = orderService;
            }

            public async Task<IResponseWrapper> Handle(
                ConfirmOrderCommand request,
                CancellationToken cancellationToken)
            {
                var result = await _orderService
                    .ConfirmAsync(request.OrderId, cancellationToken);

                if (!result)
                    return await ResponseWrapper
                        .FailAsync("Order not found.");

                return await ResponseWrapper
                    .SuccessAsync("Order confirmed successfully.");
            }
        }
    }
}
