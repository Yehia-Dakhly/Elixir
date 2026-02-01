using Blood_Donation.Hubs;
using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction;

namespace Blood_Donation.Services
{
    public class RequestsUpdateService(IHubContext<RequestsHub> HubContext) : IRequestsUpdate
    {
        public async Task UpdateRequestAsync(int RequestId, object Data)
        {
            await HubContext.Clients.Group("RequestsRealTime").SendAsync("UpdateRequest", RequestId, Data);
        }
    }
}
