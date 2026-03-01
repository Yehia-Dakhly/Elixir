using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public enum ResponseStatus
    {
        Pending = 0,
        Arrived = 1,
        Rejected = 2,
        Cancelled = 3,
    }
}
