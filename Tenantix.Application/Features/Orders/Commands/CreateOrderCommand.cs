using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands
{
    public class CreateOrderCommand  :IRequest<IResponseWrapper>,IValidateMe
    {
        public CreateOrderRequest CreateOrder { get; set; } = default!;
    }     
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, IResponseWrapper>
    {
        private readonly IOrderService _orderService;
        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IResponseWrapper> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderId = await _orderService.CreateAsync(request.CreateOrder, cancellationToken);

            return await ResponseWrapper<Guid>.SuccessAsync(orderId, "Order created successfully.");
        }
    }
}
