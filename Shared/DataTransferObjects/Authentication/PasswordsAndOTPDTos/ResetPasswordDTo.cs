using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos
{
    public class ResetPasswordDTo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string ResetToken { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
