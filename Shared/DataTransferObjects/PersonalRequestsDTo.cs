using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class PersonalRequestsDTo
    {
        public BloodRequestDTo BloodRequestDTo { get; set; } = null!;
        public IEnumerable<ResponsesDTo> ResponsesDTos { get; set; } = [];
    }
}
