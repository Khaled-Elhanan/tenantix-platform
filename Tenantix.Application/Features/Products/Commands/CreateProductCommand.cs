using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<IResponseWrapper>
    {
        public CreateProductRequest CreateProduct { get; set; } = default!;
    }

    public class CreateProductCommandHandler
        : IRequestHandler<CreateProductCommand, IResponseWrapper>
    {
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IResponseWrapper> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var productId = await _productService.CreateAsync(
                request.CreateProduct,
                cancellationToken);

            return await ResponseWrapper<Guid>
                .SuccessAsync(productId, "Product created successfully.");
        }
    }
}
