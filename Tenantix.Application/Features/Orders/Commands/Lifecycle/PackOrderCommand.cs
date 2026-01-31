using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands.Lifecycle
{
    public class PackOrderCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid OrderId { get; set; }
    }

    public class PackOrderCommandHandler
        : IRequestHandler<PackOrderCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderService;

        public PackOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(
            PackOrderCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _orderService
                .PackAsync(request.OrderId, cancellationToken);

            if (!result)
                return await ResponseWrapper
                    .FailAsync("Order not found.");

            return await ResponseWrapper
                .SuccessAsync("Order packed successfully.");
        }
    }
}
