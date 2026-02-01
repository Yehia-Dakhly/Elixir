using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class DonationResponseDTo
    {
        public Guid DonorUserId { get; set; }
        public string FullName { get; set; } = null!;
        public short Age { get; set; }
        public string BloodType { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public DateTime ResponseAt { get; set; }
        public string ResponseStatus { get; set; } = null!;
        public Gender Gender { get; set; }
    }
}
