using MediatR;
using Tenantix.Application.Common.Identity.Tokens;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.StoreAuth.Commands
{
    public class RegisterCustomerCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public StoreRegisterRequest Register { get; set; } = default!;
    }

    public class RegisterCustomerCommandHandler
        : IRequestHandler<RegisterCustomerCommand, IResponseWrapper>
    {
        private readonly IStoreAuthService _authService;

        public RegisterCustomerCommandHandler(IStoreAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IResponseWrapper> Handle(
            RegisterCustomerCommand request,
            CancellationToken cancellationToken)
        {
            var token = await _authService.RegisterAsync(
                request.Register,
                cancellationToken);

            return await ResponseWrapper<TokenResponse>
                .SuccessAsync(data:token , "Customer registered successfully");
        }
    }
}
