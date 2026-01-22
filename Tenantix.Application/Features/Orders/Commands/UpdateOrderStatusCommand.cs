using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Orders.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusCommand :IRequest<IResponseWrapper>
    {
        public Guid OrderId { get; set; }
        public UpdateOrderStatusRequest Request { get; set; } = default!;
    }
    public class UpdateOrderStatusCommandHandler
      : IRequestHandler<UpdateOrderStatusCommand, IResponseWrapper>
    {
          private readonly IOrderService _orderService;
        public UpdateOrderStatusCommandHandler(IOrderService orderService)
        {
           _orderService = orderService;
        }
        public async Task<IResponseWrapper> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
           var ok = await _orderService.UpdateStatusAsync(request.OrderId,
               request.Request.Status, 
               cancellationToken);
            if (ok)
            {
                return await ResponseWrapper<string>.SuccessAsync("Order status updated successfully.");
            }
            else
            {
                return await ResponseWrapper<string>.FailAsync("Failed to update order status.");
            }
        }
    }
}
