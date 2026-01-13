using FluentValidation;
using Tenantix.Application.Features.Tenancy.Commands;

public class CreateTenantCommandValidator
    : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.CreateTenant)
            .NotNull()
            .SetValidator(new CreateTenantRequestValidator());
    }
}
