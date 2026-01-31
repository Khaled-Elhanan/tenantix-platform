using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands.Lifecycle
{
    public class ShipOrderCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid OrderId { get; set; }
    }

    public class ShipOrderCommandHandler
        : IRequestHandler<ShipOrderCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderService;

        public ShipOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(
            ShipOrderCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _orderService
                .ShipAsync(request.OrderId, cancellationToken);

            if (!result)
                return await ResponseWrapper
                    .FailAsync("Order not found.");

            return await ResponseWrapper
                .SuccessAsync("Order shipped successfully.");
        }
    }
}

