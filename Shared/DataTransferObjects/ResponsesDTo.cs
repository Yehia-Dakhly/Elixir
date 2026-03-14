using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class ResponsesDTo
    {
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string BloodType { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string Gender { get; set; } = null!;

        public Guid DonorUserId { get; set; }
        public int BloodRequestId { get; set; }
        public DateTime ResponseAt { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
