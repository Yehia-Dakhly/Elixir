using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PersonalRequestsQueryParams
    {
        private const int DefaultPageSize = 1;
        private const int MaxPageSize = 5;
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        private int pagesize = DefaultPageSize;
        public int Pagesize
        {
            get { return pagesize; }
            set { pagesize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}
