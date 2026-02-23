using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class FirebaseNotificationController(IFirebaseNotificationService _firebaseNotificationService) : ApiBaseController
    {
        [HttpPost("send")]
        public async Task<ActionResult<bool>> SendNotification(string UserToken, NotificationMessageDTo NotificationMessage)
        {
            return Ok(await _firebaseNotificationService.SendToUserAsync(UserToken, NotificationMessage));
        }
    }
}
