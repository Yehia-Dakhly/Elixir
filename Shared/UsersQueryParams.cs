using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class UsersQueryParams
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;
        public int? GovernorateId { get; set; }
        public int? CityId { get; set; }
        public UsersSortingOptions? SortingOption { get; set; }
        public string? Search { get; set; }
        public bool? IsAvailable { get; set; }
        [Range(1, 8, ErrorMessage = "Blood Types From 1 To 8")]
        public int? BloodType { get; set; }
        public int PageIndex { get; set; } = 1;
        private int pagesize = DefaultPageSize;
        public int Pagesize
        {
            get { return pagesize; }
            set { pagesize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}
