using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class ConfirmRequestResponseDTo
    {
        public string Message { get; set; } = null!;
        public bool Success { get; set; }
    }
}
