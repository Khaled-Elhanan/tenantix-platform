using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery :IRequest<IResponseWrapper>
    {
        public Guid Id { get; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }

    }
    public class GetCategoryByIdQueryHandler
        : IRequestHandler<GetCategoryByIdQuery, IResponseWrapper>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
                return await ResponseWrapper.FailAsync("Category not found.");

            return await ResponseWrapper<CategoryResponse>
                .SuccessAsync(category);
        }
    }
}
