using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class DonorsBloodTypesAnalysis
    {
        public BloodTypesPercentagesDTo BloodTypesPercentagesDTo { get; set; } = null!;
        public int TotalDonorsCount { get; set; }
    }
}
