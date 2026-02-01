using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class DonationResponses : BaseEntity<long>
    {
        public Guid DonorUserId { get; set; }
        public BloodDonationUser DonorUser { get; set; } = null!;
        public int BloodRequestId { get; set; }
        public BloodRequests BloodRequest { get; set; } = null!;
        public DateTime ResponseAt { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
