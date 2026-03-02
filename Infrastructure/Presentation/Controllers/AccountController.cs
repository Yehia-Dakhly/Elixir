using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using ServiceAbstraction;
using Shared;
using Shared.Constants;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize(Roles = ElixirRoles.User)]
    public class AccountController(IAccountService _accountService) : ApiBaseController
    {
        [HttpGet("profile")]
        public async Task<ActionResult<AccountDTo>> GetAccountProfileAsync()
        {
            return Ok(await _accountService.GetAccountProfileAsync(GetUserId()));
        }
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateAccountProfileAsync(UpdateAccountDTo updateAccountDTo)
        {
            await _accountService.UpdateAccountProfileAsync(updateAccountDTo, GetUserId().ToString());
            return Ok();
        }
        [HttpPost("location-fcm")]
        public async Task<ActionResult> UpdateFCMTokenAndLocation(UpdateDeviceTokenAndLoccationDTo updateDeviceTokenAndLocation)
        {
            await _accountService.RefreshDeviceTokenAndLocationAsync(updateDeviceTokenAndLocation, GetUserId().ToString());
            return Ok();
        }
        [HttpGet("donation-history")]
        public async Task<ActionResult<PaginatedResult<DonationHistoryDTo>>> GetDonationHistoryToUser(DonationHistoryQueryParams Params)
        {
            return Ok(await _accountService.GetDonationHistoryAsync(Params, GetUserId().ToString()));
        }
    }
}
