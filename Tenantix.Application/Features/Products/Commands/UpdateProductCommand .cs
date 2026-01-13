using Tenantix.Application.Pipelines;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Products.Commands
{
    public class UpdateProductCommand:IRequest<IResponseWrapper> ,IValidateMe
    {
           public Guid Id { get; init; }
        public UpdateProductRequest Product { get; init; } = default!;
    }
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, IResponseWrapper>
    {
        private readonly IProductService _productService;
        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResponseWrapper> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var updated = await _productService.UpdateAsync(request.Id, request.Product, cancellationToken);
            if (!updated)
            {
                return await ResponseWrapper
                    .FailAsync("Product not found .");
            }
            return await ResponseWrapper
                .SuccessAsync("Product updated successfully");
        }
        
    }
}
