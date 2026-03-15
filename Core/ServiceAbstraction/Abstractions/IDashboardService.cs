using Shared;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface IDashboardService
    {
        public Task<PaginatedResult<BloodRequestDTo>> GetCriticalRequestsAsync(RequestQueryParams Params);
        public Task<float> GetCompleteRequestsPercentageAsync();
        public Task<float> GetResponsesCompletedPercentageAsync();
        public Task<float> GetResponsesFailedPercentageAsync();
        public Task<DonorsBloodTypesAnalysis> GetDonorsDistributionPercentageAsync();
        public Task<float> GetDonorsReadinessPercentageAsync();
        public Task<PaginatedResult<ElixirUserDTo>> GetElixirUsersAsync(UsersQueryParams queryParams);
        public Task SendSystemNotificationAsync(SystemNotificationQueryParams queryParams);
        public Task<PaginatedResult<AdminNotificationDTo>> GetNotificationsForAdminAsync(AdminNotificationQueryParams queryParams);
    }
}
