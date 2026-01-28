using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Commands
{
    public class ClearCartCommand : IRequest<IResponseWrapper> , IValidateMe
    {
        public Guid CustomerId { get; set; }
    }
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, IResponseWrapper>
    {
        private readonly ICartService _cartService;
        public ClearCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IResponseWrapper> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.ClearAsync(request.CustomerId, cancellationToken);
            return await ResponseWrapper.SuccessAsync("Cart cleared successfully.");
        }
    }
}
