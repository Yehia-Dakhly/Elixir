using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class NotificationMessageDTo
    {
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Dictionary<string, string> Data { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime SendAt { get; set; }
    }
}
