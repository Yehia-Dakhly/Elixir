using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record SendEmailEvent
    {
        public string Body { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string To { get; set; } = null!;
    }
}
