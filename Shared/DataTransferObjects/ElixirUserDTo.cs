using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class ElixirUserDTo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int Gender { get; set; }
        public string BloodType { get; set; } = null!;
        public int Age { get; set; }
        public string City { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public int MaxFailedDonationCount { get; set; }
        //public string Location { get; set; } = null!;
    }
}
