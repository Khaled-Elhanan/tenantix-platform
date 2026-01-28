using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Commands
{
    public class RemoveCartItemCommand : IRequest<IResponseWrapper> , IValidateMe
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
    }
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, IResponseWrapper>
    {
        private readonly ICartService _cartService;
        public RemoveCartItemCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IResponseWrapper> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            await _cartService.RemoveItemAsync(request.CustomerId, request.ProductId, cancellationToken);
            return await ResponseWrapper.SuccessAsync("Item removed from cart.");
        }
    }
}
