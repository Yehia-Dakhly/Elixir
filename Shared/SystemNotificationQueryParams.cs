using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SystemNotificationQueryParams
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Body { get; set; } = null!;
        [Range(1, 8, ErrorMessage = "Blood Types From 1 To 8")]
        public int? BloodTypeId { get; set; }
        [Range(1, 27, ErrorMessage = "Governorates From 1 To 27")]
        public int? GovernorateId { get; set; }
    }
}
