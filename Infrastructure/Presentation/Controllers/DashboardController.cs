using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using ServiceAbstraction;
using Shared;
using Shared.Constants;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    //[Authorize(Roles = ElixirRoles.Admin)]
    //[SkipApiKey]
    //public class DashboardController(IServiceManager _serviceManager) : ApiBaseController
    //{
    //    [HttpGet("completed-requests-percentage")]
    //    public async Task<ActionResult<float>> GetCompletedRequestsPercentage()
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetCompleteRequestsPercentageAsync());
    //    }
    //    [HttpGet("failed-responses-percentage")]
    //    public async Task<ActionResult<float>> GetResponsesFailedPercentage()
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetResponsesFailedPercentageAsync());
    //    }
    //    [HttpGet("completed-responses-percentage")]
    //    public async Task<ActionResult<float>> GetResponsesCompletedPercentage()
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetResponsesCompletedPercentageAsync());
    //    }
    //    [HttpGet("critical-requests")]
    //    public async Task<ActionResult<PaginatedResult<BloodRequestDTo>>> GetCriticalRequests(RequestQueryParams Params)
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetCriticalRequestsAsync(Params));
    //    }
    //    [HttpGet("blood-types-percentage")]
    //    public async Task<ActionResult<DonorsBloodTypesAnalysis>> GetBloodTypesPercentages()
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetDonorsDistributionPercentageAsync());
    //    }
    //    [HttpGet("donors-readiness")]
    //    public async Task<ActionResult<float>> GetDonorsReadinessPercentage()
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetDonorsReadinessPercentageAsync());
    //    }
    //    [HttpGet("elixir-users")]
    //    public async Task<ActionResult<PaginatedResult<ElixirUserDTo>>> GetElixirUsers([FromQuery] UsersQueryParams queryParams)
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetElixirUsersAsync(queryParams));
    //    }
    //    [HttpPost("system-notification")]
    //    public async Task<ActionResult> SendSystemNotification(SystemNotificationQueryParams queryParams)
    //    {
    //        await _serviceManager.DashboardService.SendSystemNotificationAsync(queryParams);
    //        return Ok(new { Message = "يتم إرسال الإشعارات" });
    //    }
    //    [HttpGet("notifications")]
    //    public async Task<ActionResult<PaginatedResult<AdminNotificationDTo>>> GetAdminNotifications([FromQuery] AdminNotificationQueryParams queryParams)
    //    {
    //        return Ok(await _serviceManager.DashboardService.GetNotificationsForAdminAsync(queryParams));
    //    }
    //}
}
