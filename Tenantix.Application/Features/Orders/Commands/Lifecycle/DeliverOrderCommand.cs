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
    public class DeliverOrderCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid OrderId { get; set; }
    }

    public class DeliverOrderCommandHandler
        : IRequestHandler<DeliverOrderCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderService;

        public DeliverOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(
            DeliverOrderCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _orderService
                .DeliverAsync(request.OrderId, cancellationToken);

            if (!result)
                return await ResponseWrapper
                    .FailAsync("Order not found.");

            return await ResponseWrapper
                .SuccessAsync("Order delivered successfully.");
        }
    }
}
