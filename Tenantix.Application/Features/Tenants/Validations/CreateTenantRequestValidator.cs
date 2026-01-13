using FluentValidation;
using Tenantix.Application.Features.Tenants.DTOs;

public class CreateTenantRequestValidator
    : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9-]+$")
            .WithMessage("Identifier can contain letters, numbers and hyphens only");
    }
}
