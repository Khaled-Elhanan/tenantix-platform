using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Carts.Commands;

namespace Tenantix.Application.Features.Carts.Validations
{
    public class ClearCartCommandValidator  :AbstractValidator<ClearCartCommand>
    {
        public ClearCartCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }
}
