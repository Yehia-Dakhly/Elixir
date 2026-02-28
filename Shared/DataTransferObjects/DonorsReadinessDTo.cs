using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class DonorsReadinessDTo
    {
        public float Percentage { get; set; }
        public string StatusText { get; set; } = null!;
    }
}
