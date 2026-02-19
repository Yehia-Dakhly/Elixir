using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record SendNotificationEvent
    {
        public Guid UserId { get; set; }
        public DateTime SendAt { get; set; }
        public int BloodRequestId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Dictionary<string, string> Data { get; set; } = null!;
        public string DeviceToken { get; set; } = null!;
        public int NotificationType { get; set; }
    }
}
