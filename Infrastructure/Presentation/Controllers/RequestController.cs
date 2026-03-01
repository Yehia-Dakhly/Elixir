using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{ 
    public class RequestController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost("create")]
        public async Task<ActionResult<bool>> CreateBloodRequest(CreateBloodRequestDTo createBloodRequest)
        {
            return Ok(await _serviceManager.RequestService.CreateBloodRequestAsync(createBloodRequest, GetUserId())); 
        }
        [HttpGet("requests")]
        public async Task<ActionResult<PaginatedResult<BloodRequestDTo>>> GetRequests(RequestQueryParams Params) // Query Params
        {
            return Ok(await _serviceManager.RequestService.GetRequestsAsync(Params));
        }
        [HttpGet("personal-requests")]
        public async Task<ActionResult<PaginatedResult<PersonalRequestsDTo>>> GetPersonalRequests(PersonalRequestsQueryParams Params) // Query Params
        {
            return Ok(await _serviceManager.RequestService.GetPersonalRequestsAsync(Params));
        }
        [HttpGet("request")]
        public async Task<ActionResult<PaginatedResult<BloodRequestDTo>>> GetRequestById(int Id)
        {
            return Ok(await _serviceManager.RequestService.GetRequestByIdAsync(Id));
        }
        [HttpPatch("close")]
        public async Task<ActionResult> CloseBloodRequest(int RequestId)
        {
            await _serviceManager.RequestService.CloseBloodRequestAsync(GetUserId(), RequestId);
            return Ok(new { Message = "!تم قفل الطلب بنجاح" });
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteBloodRequest(int RequestId)
        {
            await _serviceManager.RequestService.DeleteBloodRequestAsync(GetUserId(), RequestId);
            return Ok(new { Message = "!تم حذف الطلب بنجاح" });
        }
    }
}
