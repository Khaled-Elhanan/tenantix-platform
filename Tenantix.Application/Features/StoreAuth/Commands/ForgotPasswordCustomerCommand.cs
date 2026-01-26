using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Application.Pipelines;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Features.StoreAuth.Commands
{
    public class ForgotPasswordCustomerCommand : IRequest<ResponseWrapper>, IValidateMe
    {
        public StoreForgotPasswordRequest Request { get; set; } = default!;
    }

    public class ForgotPasswordCustomerCommandHandler : IRequestHandler<ForgotPasswordCustomerCommand, ResponseWrapper>
    {
        private readonly IStoreAuthService _auth;

        public ForgotPasswordCustomerCommandHandler(IStoreAuthService auth)
        {
            _auth = auth;
        }

        public async Task<ResponseWrapper> Handle(ForgotPasswordCustomerCommand request, CancellationToken cancellationToken)
        {
            await _auth.ForgotPasswordAsync(request.Request.Email, cancellationToken);

            // ✅ always return success (do not disclose if email exists)
            return await ResponseWrapper.SuccessAsync("If the email exists, a reset link will be sent.");
        }
    }
}
