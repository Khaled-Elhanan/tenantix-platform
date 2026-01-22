using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Models;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IResponseWrapper> , IValidateMe
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }                                                              
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IResponseWrapper>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            return await ResponseWrapper <PagedResponse<CategoryResponse>>.SuccessAsync(result);
        }
    }
}
