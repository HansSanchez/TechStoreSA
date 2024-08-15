using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechStoreSA.Shared
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
