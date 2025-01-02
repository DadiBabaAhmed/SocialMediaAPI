using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryObject
    {
        public string? Title { get; set; } = null;
        public string? Content { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        //public string? Search { get; set; }
        public string? SortBy { get; set; } = null;
        public bool IsSortAscending { get; set; } = false;

    }
}