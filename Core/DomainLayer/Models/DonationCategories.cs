using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class DonationCategories: BaseEntity<int>
    {
        public string NameAr { get; set; } = null!;
        public int MaleMinDaysInterval { get; set; }
        public int FemaleMinDaysInterval { get; set; }

        public ICollection<BloodRequests> BloodRequests { get; set; } = new HashSet<BloodRequests>();
        public ICollection<CompatibilityMatrix> Compatibilities { get; set; } = new HashSet<CompatibilityMatrix>();
        public ICollection<DonationHistory> DonationHistory { get; set; } = new HashSet<DonationHistory>();
    }
}
