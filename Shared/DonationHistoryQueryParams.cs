using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DonationHistoryQueryParams
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 15;
        public int PageNumber { get; set; } = 1;
        private int pageSize = DefaultPageSize;
        public DonationHistorySorting DonationHistorySorting { get; set; }
        public int PageSize 
        { 
            get 
            {
                return pageSize;
            }
            set
            {
                pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }
    }
}
