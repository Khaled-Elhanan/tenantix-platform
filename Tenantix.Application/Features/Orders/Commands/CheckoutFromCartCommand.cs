using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands
{
    public class CheckoutFromCartCommand  :IRequest<IResponseWrapper>,IValidateMe
    {
        public Guid CustomerId { get; set; }
        public CheckoutRequest CheckoutRequest { get; set; }

    }
    public class CheckoutFromCartCommandHandler
        : IRequestHandler<CheckoutFromCartCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderSerivce;

        private readonly IOrderService _orderService;

        public CheckoutFromCartCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(CheckoutFromCartCommand request, CancellationToken cancellationToken)
        {
            var orderId = await _orderService.CheckoutFromCartAsync(
                request.CustomerId,
                request.CheckoutRequest,
                cancellationToken);

            return await ResponseWrapper<Guid>.SuccessAsync(orderId, "Checkout completed successfully.");
        }
    }
}
