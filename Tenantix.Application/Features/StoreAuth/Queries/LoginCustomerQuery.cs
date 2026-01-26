using MediatR;
using Tenantix.Application.Common.Identity.Tokens;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.StoreAuth.Queries
{
    public class LoginCustomerQuery : IRequest<IResponseWrapper>
    {
        public StoreLoginRequest Login { get; set; } = default!;
    }

    public class LoginCustomerQueryHandler
        : IRequestHandler<LoginCustomerQuery, IResponseWrapper>
    {
        private readonly IStoreAuthService _authService;

        public LoginCustomerQueryHandler(IStoreAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IResponseWrapper> Handle(
            LoginCustomerQuery request,
            CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(
                request.Login,
                cancellationToken);

            return await ResponseWrapper<TokenResponse>
                .SuccessAsync(token);
        }
    }
}
