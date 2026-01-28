using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Carts.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Carts.Commands
{
    public class UpdateCartItemCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid CustomerId { get; set; }
        public UpdateCartItemRequest Item { get; set; } = default!;
    }

    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, IResponseWrapper>
    {
        private readonly ICartService _cartService;

        public UpdateCartItemCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            await _cartService.UpdateItemAsync(request.CustomerId, request.Item, cancellationToken);
            return await ResponseWrapper.SuccessAsync("Cart item updated successfully.");
        }
    }
}
