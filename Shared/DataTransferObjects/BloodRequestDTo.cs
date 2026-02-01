using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class BloodRequestDTo
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string HospitalName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public int BagsCount { get; set; }
        public int ResponsesCount { get; set; }
        public int CollectedBags { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }

        public string RequesterName { get; set; } = null!;
        public string DonationCategoryAr { get; set; } = null!;
        public string RequiredBloodType { get; set; } = null!;
        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;
        public DateTime Deadline { get; set; }
    }
}
