using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class DonationHistory : BaseEntity<long>
    {
        public Guid DonorId { get; set; }
        public BloodDonationUser Donor { get; set; } = null!;
        public int DonationCategoryId { get; set; }
        public DonationCategories DonationCategory { get; set; } = null!;
        public DateTime DonationDate { get; set; }
    }
}
