using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenantix.Shared.Models
{
    public class PagedResponse <T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];

        public int Page { get; set; }

        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
