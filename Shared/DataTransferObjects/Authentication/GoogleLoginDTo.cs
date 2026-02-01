using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication
{
    public class GoogleLoginDTo
    {
        [Required(ErrorMessage = "Google IdToken is required")]
        public string IdToken { get; set; } = null!;
        [Required(ErrorMessage = "Device Token FCM is required")]
        public string DeviceToken { get; set; } = null!;
    }
}
