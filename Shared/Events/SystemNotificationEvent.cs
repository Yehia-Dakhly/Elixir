using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record SystemNotificationEvent
    {
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int? BloodTypeId { get; set; }
        public int? GovernorateId { get; set; }
    }
}
