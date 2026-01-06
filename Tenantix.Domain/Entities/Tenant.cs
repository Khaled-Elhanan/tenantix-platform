using Tenantix.Domain.Common;

namespace Tenantix.Domain.Entities
{
    public class Tenant :BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime ValidUpTo { get; set; }

    }
}
