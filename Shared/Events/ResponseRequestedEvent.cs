using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record ResponseRequestedEvent
    {
        public string DonorName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int RequestId { get; set; }
        public string RequesterId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime ResponseAt { get; set; }
        public int BloodRequestId { get; set; }
        public Guid DonorId { get; set; }
        public string RequesterDeviceToken { get; set; } = null!;
    }
}
