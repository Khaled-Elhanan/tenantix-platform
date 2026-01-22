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
    public class UpdateCategoryCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public Guid Id { get; set; }
        public UpdateCategoryRequest Category { get; set; } = default!;
    }
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResponseWrapper>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var ok = await _categoryService.UpdateAsync(request.Id, request.Category, cancellationToken);
            if(ok)
            {
                return await ResponseWrapper.SuccessAsync("Category updated successfully.");
            }
            else
            {
                return await ResponseWrapper.FailAsync("Category not found.");
            }

        }
    }
}
