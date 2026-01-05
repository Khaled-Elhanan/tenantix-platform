namespace Tenantix.Domain.Entities
{
    public class Store
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public string DefaultCurrency { get; set; } = "EGP";

        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  




    }
}
