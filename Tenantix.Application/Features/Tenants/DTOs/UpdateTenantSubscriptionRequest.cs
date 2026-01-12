namespace Tenantix.Application.Features.Tenants.DTOs;

public class UpdateTenantSubscriptionRequest
{
    public string TenantId { get; set; }
    public DateTime  NewExpiryDate { get; set; }

}