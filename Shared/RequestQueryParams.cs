
namespace Shared
{
    public class RequestQueryParams
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;
        public int? GovernorateId { get; set; }
        public int? CityId { get; set; }
        public RequestSortingOptions SortingOption { get; set; }
        public string? Search { get; set; }
        public bool SuitableRequests { get; set; }
        public int PageNumber { get; set; } = 1;
        private int pagesize = DefaultPageSize;
        public int Pagesize 
        {
            get { return pagesize; } 
            set { pagesize = value > MaxPageSize ? MaxPageSize : value; } 
        }
    }
}
