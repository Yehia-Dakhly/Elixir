using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class AdminNotificationDTo
    {
        public int BloodRequestId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string NotificationType { get; set; } =  null!;
        public DateTime SendAt { get; set; }
        public int SendedToCount { get; set; }
    }
}
