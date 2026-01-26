using MediatR;
using Tenantix.Application.Pipelines;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.StoreAuth.Commands
{
    public class ResetPasswordCustomerCommand : IRequest<ResponseWrapper>, IValidateMe
    {
        public StoreResetPasswordRequest Request { get; set; } = default!;
    }

    public class ResetPasswordCustomerCommandHandler : IRequestHandler<ResetPasswordCustomerCommand, ResponseWrapper>
    {
        private readonly IStoreAuthService _auth;

        public ResetPasswordCustomerCommandHandler(IStoreAuthService auth)
        {
            _auth = auth;
        }

        public async Task<ResponseWrapper> Handle(ResetPasswordCustomerCommand request, CancellationToken cancellationToken)
        {
            await _auth.ResetPasswordAsync(
                request.Request.Email,
                request.Request.Token,
                request.Request.NewPassword,
                cancellationToken);

            return await ResponseWrapper.SuccessAsync("Password reset successfully.");
        }
    }
}
