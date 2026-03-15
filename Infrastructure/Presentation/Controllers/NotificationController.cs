using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Abstractions;
using Shared;
using Shared.Constants;
using Shared.DataTransferObjects;

namespace Presentation.Controllers
{
    [Authorize(Roles = ElixirRoles.User)]
    public class NotificationController(INotificationService _notificationService) : ApiBaseController
    {
        [HttpGet("notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDTo>>> GetAllNotifcations(NotificationQueryParams _notificationQueryParams)
        {
            return Ok(await _notificationService.GetAllNotificationAsync(_notificationQueryParams, GetUserId()));
        }
        [HttpGet("notifications-count")]
        public async Task<ActionResult<IEnumerable<NotificationDTo>>> GetAllNotifcationsCount()
        {
            return Ok(await _notificationService.GetAllNotificationCount(GetUserId()));
        }
        [HttpPatch("read")]
        public async Task<ActionResult> ReadNotification(long Id)
        {
            await _notificationService.ReadNotificationAsync(GetUserId(), Id);
            return Ok();
        }
        [HttpPatch("read-all")]
        public async Task<ActionResult> ReadNotification()
        {
            await _notificationService.ReadAllNotificationAsync(GetUserId());
            return Ok();
        }
    }
}
