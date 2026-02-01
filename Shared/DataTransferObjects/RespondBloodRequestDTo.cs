using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class RespondBloodRequestDTo
    {
        [Required]
        public bool CanResponse { get; set; }
        public DateTime LastDonationDate { get; set; }
        public string LastDonationCatrgory { get; set; } = null!;
        public DateTime NextDonationDate { get; set; }
    }
}
