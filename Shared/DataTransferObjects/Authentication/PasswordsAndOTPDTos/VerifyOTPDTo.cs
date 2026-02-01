using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos
{
    public class VerifyOTPDTo
    {
        [Required]
        public string OTP { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
