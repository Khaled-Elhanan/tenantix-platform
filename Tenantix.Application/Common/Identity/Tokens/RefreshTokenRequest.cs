namespace Tenantix.Application.Common.Identity.Tokens;

public class RefreshTokenRequest
{
    public string CurrentJwt {get; set;}
    public string CurrentRefreshToken {get; set;}
    public DateTime RefreshTokenExpiryDate {get; set;}
}         