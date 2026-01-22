using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Categories.Commands;

namespace Tenantix.Application.Features.Categories.Validations
{
    public class CreateCategoryCommandValidator :AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.CreateCategory).NotNull();
            RuleFor(x => x.CreateCategory.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.CreateCategory.Description).MaximumLength(500);
        }
    }
}
