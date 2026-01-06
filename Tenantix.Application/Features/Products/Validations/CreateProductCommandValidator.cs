using FluentValidation;
using Tenantix.Application.Features.Products.Commands;

namespace Tenantix.Application.Features.Products.Validations
{
    public class CreateProductCommandValidator
        : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.CreateProduct)
                .NotNull()
                .SetValidator(new CreateProductRequestValidator());
        }
    }
}
