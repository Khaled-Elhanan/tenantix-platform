namespace Tenantix.Application.Features.StoreAuth.DTOs
{
    public class StoreLoginRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
