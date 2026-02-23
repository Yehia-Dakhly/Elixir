using Shared;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAccountService
    {
        public Task<AccountDTo> GetAccountProfileAsync(Guid UserId);
        public Task UpdateAccountProfileAsync(UpdateAccountDTo updateAccountDTo, string UserId);
        public Task RefreshDeviceTokenAndLocationAsync(UpdateDeviceTokenAndLoccationDTo updateDeviceTokenAndLoccationDTo, string UserId);
        public Task<PaginatedResult<DonationHistoryDTo>> GetDonationHistoryAsync(DonationHistoryQueryParams Params, string UserId);
    }
}
