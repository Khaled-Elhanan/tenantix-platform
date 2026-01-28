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
            await _cartService.ClearCartAsync(request.CustomerId, cancellationToken);
            return await ResponseWrapper.SuccessAsync("Cart cleared successfully.");
        }
    }
}
