using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class BloodTypeDTo
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public char RhFactor { get; set; }
    }
}
