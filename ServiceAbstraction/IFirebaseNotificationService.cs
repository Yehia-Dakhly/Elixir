using Shared.DataTransferObjects;

namespace ServiceAbstraction
{
    public interface IFirebaseNotificationService
    {
        public Task<bool> SendToUserAsync(string UserToken, NotificationMessageDTo NotificationMessage);
        public Task<bool> SendToUsersAsync(IEnumerable<string> UserTokens, NotificationMessageDTo NotificationMessage);
    }
}
