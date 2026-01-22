using MediatR;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<IResponseWrapper>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCategoryCommandHandler
        : IRequestHandler<DeleteCategoryCommand, IResponseWrapper>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IResponseWrapper> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var ok = await _categoryService.DeleteAsync(request.Id, cancellationToken);

            if (!ok)
                return await ResponseWrapper.FailAsync("Category not found.");

            return await ResponseWrapper.SuccessAsync("Category deleted successfully.");
        }
    }
}
