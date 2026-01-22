using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Categories.Commands;

namespace Tenantix.Application.Features.Categories.Validations
{
    public class UpdateCategoryCommandValidator  : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Category).NotNull();
            RuleFor(x => x.Category.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Category.Description).MaximumLength(500);

        }
    }
}
