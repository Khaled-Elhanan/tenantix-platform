using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Queries
{
    public class GetCartByCustomerIdQuery : IRequest<IResponseWrapper>  , IValidateMe
    {
        public Guid CustomerId { get; }

        public GetCartByCustomerIdQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }

    public class GetCartByCustomerIdQueryHandler
        : IRequestHandler<GetCartByCustomerIdQuery, IResponseWrapper>
    {
        private readonly ICartService _cartService;

        public GetCartByCustomerIdQueryHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper> Handle(GetCartByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

            if (cart is null)
                return await ResponseWrapper.FailAsync("Cart not found.");

            return await ResponseWrapper<CartResponse>.SuccessAsync(cart);
        }
    }
}
