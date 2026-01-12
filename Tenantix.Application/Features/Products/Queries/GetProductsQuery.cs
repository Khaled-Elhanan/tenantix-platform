using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Products.DTOs;
using Tenantix.Shared.Models;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<IResponseWrapper>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public class GetProductsQueryHandler
        : IRequestHandler<GetProductsQuery, IResponseWrapper>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResponseWrapper> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _productService.GetPagedAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

            return await ResponseWrapper<PagedResponse<ProductListItemResponse>>
                .SuccessAsync(data: result);
        }
    }
}
