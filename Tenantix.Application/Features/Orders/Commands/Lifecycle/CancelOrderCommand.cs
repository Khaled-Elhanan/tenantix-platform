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
    public class CancelOrderCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid OrderId { get; set; }
    }

    public class CancelOrderCommandHandler
        : IRequestHandler<CancelOrderCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderService;

        public CancelOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _orderService
                .CancelAsync(request.OrderId, cancellationToken);

            if (!result)
                return await ResponseWrapper
                    .FailAsync("Order not found or cannot be cancelled.");

            return await ResponseWrapper
                .SuccessAsync("Order cancelled successfully.");
        }
    }
}
