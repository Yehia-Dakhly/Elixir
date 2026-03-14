using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class AccountDTo
    {
        public string FullName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string BloodType { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}
