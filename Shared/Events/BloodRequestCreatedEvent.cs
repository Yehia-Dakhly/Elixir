using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record BloodRequestCreatedEvent
    {
        public string RequesterId { get; set; } = null!;
        public int RequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string BloodCategoryName { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string HospitalName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public int DonationCategoryId { get; set; }
        public int RequiredBloodTypeId { get; set; }
        public bool? IsDonorReported { get; set; } = false;
    }
}
