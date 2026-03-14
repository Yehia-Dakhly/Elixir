using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AdminNotificationQueryParams
    {
        private const int MaxPageSize = 10;
        private const int DefaultPageSize = 5;
        public string? SearchTitle { get; set; }
        public int PageIndex { get; set; } = 1;
        private int pageSize = DefaultPageSize;
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
