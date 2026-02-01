using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ResponseController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost("respond")]
        public async Task<ActionResult<bool>> RespondBloodRequest(int RequestId)
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
