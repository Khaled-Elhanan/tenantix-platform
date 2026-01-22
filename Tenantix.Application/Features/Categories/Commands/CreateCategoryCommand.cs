using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.Categories.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<IResponseWrapper> ,IValidateMe
    {
        public CreateCategoryRequest CreateCategory { get; set; } = default!;
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResponseWrapper>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var id = await _categoryService.CreateAsync(request.CreateCategory, cancellationToken);
            return await ResponseWrapper<Guid>
                .SuccessAsync(id, "Category created successfully.");
        }
    }
}
