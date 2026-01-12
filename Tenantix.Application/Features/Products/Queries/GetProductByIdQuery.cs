using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Application.Features.Products.Queries;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Products.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<IResponseWrapper>;

    public class GetProductByIdQueryHandler
          : IRequestHandler<GetProductByIdQuery, IResponseWrapper>
    {
        private readonly IProductService _productService;

        public GetProductByIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResponseWrapper> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (product is null)
            {
                return await ResponseWrapper<ProductResponse>
                    .FailAsync("Product not found");
            }

            return await ResponseWrapper<ProductResponse>
                .SuccessAsync(product);
        }
    }
}