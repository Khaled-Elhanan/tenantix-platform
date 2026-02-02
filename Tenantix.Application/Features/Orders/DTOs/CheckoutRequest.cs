namespace Tenantix.Application.Features.Orders.DTOs
{
    public class CheckoutRequest
    {
        public string ? Notes { get; set; }
       
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }

    }
}
