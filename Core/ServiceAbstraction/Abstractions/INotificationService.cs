using Shared;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface INotificationService
    {
        public Task<PaginatedResult<NotificationDTo>> GetAllNotificationAsync(NotificationQueryParams Params, Guid UserId);
        public Task ReadNotificationAsync(Guid UserId, long NotificationId);
        public Task ReadAllNotificationAsync(Guid UserId);
        public Task<int> GetAllNotificationCount(Guid UserId);
    }
}
