using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class CompatibilityMatrix : BaseEntity<int>
    {
        public int DonationCategoryId { get; set; }
        public DonationCategories DonationCategory { get; set; } = null!;
        public int DonorTypeId { get; set; }
        public BloodTypes DonorType { get; set; } = null!;
        public int RecipientTypeId { get; set; }
        public BloodTypes RecipientType { get; set; } = null!;
    }
}
