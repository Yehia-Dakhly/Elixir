using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class DonationHistoryDTo
    {
        public string DonationCategoryName { get; set; } = null!;
        public DateTime DonationDate { get; set; }
    }
}