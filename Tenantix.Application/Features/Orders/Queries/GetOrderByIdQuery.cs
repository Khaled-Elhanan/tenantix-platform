using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Shared.Responses;
using Tenantix.Application.Features.Orders.DTOs;

public class GetOrderByIdQuery : IRequest<IResponseWrapper>
{
    public Guid Id { get; set; }
}

public class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, IResponseWrapper>
{
    private readonly IOrderService _orderService;

    public GetOrderByIdQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            return await ResponseWrapper.FailAsync("Order not found.");

        return await ResponseWrapper<OrderResponse>
            .SuccessAsync(order);
    }
}
