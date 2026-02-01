using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; } = null!;
        public string ConfirmationLink { get; set; } = null!;
    }
}
