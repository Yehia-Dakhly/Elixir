using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication
{
    public class AuthRegisterDTo
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
