using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Customers.Commands;

namespace Tenantix.Application.Features.Customers.Validations
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.Customer)
            .NotNull()
            .SetValidator(new CreateCustomerRequestValidator());
        }
    }
}
