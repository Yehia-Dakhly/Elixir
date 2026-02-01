using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class UpdateAccountDTo
    {
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public int CityId { get; set; }
    }
}
