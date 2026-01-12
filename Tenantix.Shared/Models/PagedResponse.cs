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

        public int Page { get; init; }

        public int PageSize { get; init; }
        public int TotalCount { get; init; }

       
    }
}
