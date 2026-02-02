using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Orders.Commands;

namespace Tenantix.Application.Features.Orders.Validators
{
    public class CheckoutFromCartCommandValidator : AbstractValidator<CheckoutFromCartCommand>
    {
        public CheckoutFromCartCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.CheckoutRequest).NotNull();
            RuleFor(x => x.CheckoutRequest.Phone).MaximumLength(30);
            RuleFor(x => x.CheckoutRequest.City).MaximumLength(100);
            RuleFor(x => x.CheckoutRequest.AddressLine).MaximumLength(250);
        }
    }
}
