using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Orders.Commands;

namespace Tenantix.Application.Features.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CreateOrder.CustomerId).NotEmpty();
            RuleFor(x => x.CreateOrder.OrderItems).NotEmpty();
            RuleForEach(x => x.CreateOrder.OrderItems).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).NotEmpty();
                items.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
