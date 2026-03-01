using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class ResponsesDTo
    {
        public Guid DonorUserId { get; set; }
        public int BloodRequestId { get; set; }
        public DateTime ResponseAt { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
