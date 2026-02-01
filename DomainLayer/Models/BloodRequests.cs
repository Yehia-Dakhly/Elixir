using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class BloodRequests : BaseEntity<int>
    {
        public string PatientName { get; set; } = null!;
        public string HospitalName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int BagsCount { get; set; }
        public int CollectedBags { get; set; }
        public int ResponsesCount { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public Guid RequesterId { get; set; }
        public BloodDonationUser Requester { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public int DonationCategoryId { get; set; }
        public DonationCategories DonationCategory { get; set; } = null!;
        public int RequiredBloodTypeId { get; set; }
        public BloodTypes RequiredBloodType { get; set; } = null!;
        public ICollection<DonationResponses> DonationResponses { get; set; } = new HashSet<DonationResponses>();
        public ICollection<NotificationBase> NotificationChild { get; set; } = new HashSet<NotificationBase>();
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public DateTime Deadline {  get; set; }
        public bool IsComplete => CollectedBags >= BagsCount;
    }
}
