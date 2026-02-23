using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    [ApiKey]
    public abstract class ApiBaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            Guid UserId;
            var StringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(StringId, out UserId))
            {
                return UserId;
            }
            throw new UnauthorizedAccessException();
        }
    }
}
