using FluentValidation;
using Tenantix.Application.Features.Products.Commands;

namespace Tenantix.Application.Features.Products.Validations
{
    public class CreateTenantCommandValidator
        : AbstractValidator<CreateProductCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.CreateProduct)
                .NotNull()
                .SetValidator(new CreateProductRequestValidator());
        }
    }
}
