using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class City : BaseEntity<int>
    {
        public string NameEn { get; set; } = null!;
        public string NameAr { get; set; } = null!;
        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; } = null!;

        public ICollection<BloodDonationUser> Users { get; set; } = new HashSet<BloodDonationUser>();
        public ICollection<BloodRequests> BloodRequests { get; set; } = new HashSet<BloodRequests>();
    }
}
