using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenantix.Application.Common.Identity.Tokens;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Shared.Responses;

namespace Tenantix.Application.Common.Interfaces
{
    public interface IStoreAuthService
    {
        Task<TokenResponse> RegisterAsync(StoreRegisterRequest request, CancellationToken cancellationToken);
        Task<TokenResponse> LoginAsync(StoreLoginRequest request, CancellationToken cancellationToken);
        Task ForgotPasswordAsync(string email, CancellationToken cancellationToken);
        Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);

    }
}
