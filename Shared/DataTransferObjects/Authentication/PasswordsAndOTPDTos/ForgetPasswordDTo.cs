using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos
{
    public class ForgetPasswordDTo
    {
        [Required(ErrorMessage = "يرجى كتابة البريد الإلكتروني لإستعادة الحساب")]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
