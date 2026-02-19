using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication
{
    public class RefreshTokenDTo
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
