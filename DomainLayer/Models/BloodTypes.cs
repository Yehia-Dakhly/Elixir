using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class BloodTypes : BaseEntity<int>
    {
        public string Symbol { get; set; } = null!;
        public char RhFactor { get; set; }
        public ICollection<BloodDonationUser> Users { get; set; } = new HashSet<BloodDonationUser>();
        public ICollection<BloodRequests> BloodRequests { get; set; } = new HashSet<BloodRequests>();
        public ICollection<CompatibilityMatrix> RecipientCompatibilities { get; set; } = new HashSet<CompatibilityMatrix>();
        public ICollection<CompatibilityMatrix> DonorCompatibilities { get; set; } = new HashSet<CompatibilityMatrix>();

    }
}
