using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication
{
    public class LoginDTo
    {
        // Login
        // Take Email And Password, Return Token, Display Name
        [Required]
        [EmailAddress(ErrorMessage = "يرجى إدخال البريد الالكتروني الخاص بك")]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        //[Required(ErrorMessage = "Device Token (FCM) is required")]
        public string? DeviceToken { get; set; } = null!;
    }
}
