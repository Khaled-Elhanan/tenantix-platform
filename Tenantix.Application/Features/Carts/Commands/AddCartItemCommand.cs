using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Commands
{
    public class AddCartItemCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid CustomerId { get; set; }
        public AddCartItemRequest Item { get; set; } = default!;
    }

    public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, IResponseWrapper>
    {
        private readonly ICartService _cartService;

        public AddCartItemCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
        {
            await _cartService.AddItemAsync(request.CustomerId, request.Item, cancellationToken);
            return await ResponseWrapper.SuccessAsync("Item added to cart.");
        }
    }
}
