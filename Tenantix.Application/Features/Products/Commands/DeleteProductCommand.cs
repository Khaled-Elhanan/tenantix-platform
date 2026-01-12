using Application.Pipelines;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest<IResponseWrapper> , IValidateMe
    {
      public Guid Id { get; init; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, IResponseWrapper>
    {
        private readonly IProductService _productService;
        public DeleteProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResponseWrapper> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(request.Id, cancellationToken);
            if(!deleted)
            {
                return await ResponseWrapper.FailAsync("Product not found");
            }
            return await ResponseWrapper.SuccessAsync("Product deleted successfully");
        }
    }
}
