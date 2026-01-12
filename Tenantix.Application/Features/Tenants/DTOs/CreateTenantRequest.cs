namespace Tenantix.Application.Features.Tenants.DTOs;

public class CreateTenantRequest
{
    public string? Identifier { get; set; }
    public string? Name { get; set; }

    public string? ConnectionString { get; set; }

    public string OwnerEmail { get; set; } 

    public string? CompanyName { get; set; }

        



    public DateTime ValidUpTo { get; set; }
    public bool IsActive { get; set; }
}