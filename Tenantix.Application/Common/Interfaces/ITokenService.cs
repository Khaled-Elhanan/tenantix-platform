using Tenantix.Application.Common.Identity.Tokens;

namespace Tenantix.Application.Common.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> LoginAsync(TokenRequest request);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}