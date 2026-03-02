using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.Constants;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize(Roles = ElixirRoles.User)]
    public class ResponseController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost("respond")]
        public async Task<ActionResult<RespondBloodRequestDTo>> RespondBloodRequest([FromQuery]int RequestId)
        {
            return Ok(await _serviceManager.ResponseService.RespondBloodRequestAsync(GetUserId(), RequestId));
        }
        [HttpGet("responses")]
        public async Task<ActionResult<DonationResponseDTo>> GetAllResponsesByRequestId(int RequestId)
        {
            return Ok(await _serviceManager.ResponseService.GetAllResponseByRequestId(GetUserId(), RequestId));
        }
        [HttpPatch("confirm")]
        public async Task<ActionResult<bool>> ConfirmBloodRequestResponse(int RequestId, Guid DonorId, bool HasDonated)
        {
            return Ok(await _serviceManager.ResponseService.ConfirmBloodRequestResponse(GetUserId(), DonorId, RequestId, HasDonated));
        }

    }
}
