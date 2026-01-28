using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Queries
{
    public class GetCartSummaryQuery : IRequest<IResponseWrapper>
    {
        public Guid CustomerId { get; }

        public GetCartSummaryQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }

    public class GetCartSummaryQueryHandler
        : IRequestHandler<GetCartSummaryQuery, IResponseWrapper>
    {
        private readonly ICartService _cartService;

        public GetCartSummaryQueryHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = await _cartService.GetSummaryAsync(request.CustomerId, cancellationToken);
            return await ResponseWrapper<CartSummaryResponse>.SuccessAsync(summary);
        }
    }
}
