using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Features.Carts.Commands;

namespace Tenantix.Application.Features.Carts.Validations
{
    public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
    {
        public AddCartItemCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();

            RuleFor(x => x.Item).NotNull();

            RuleFor(x => x.Item.ProductId).NotEmpty();

            RuleFor(x => x.Item.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
    }
