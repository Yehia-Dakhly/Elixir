using Blood_Donation.Hubs;
using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction;
using Shared;

namespace Blood_Donation.Services
{
    public class RequestsUpdateService(IHubContext<RequestsHub> HubContext, ILogger<RequestsUpdateService> _logger) : IRequestsUpdate
    {
        public async Task UpdateRequestAsync(int RequestId, RequestUpdateSignalRDTo Data)
        {
            _logger.LogInformation("Updating Request with Id: {RequestId} - Data: {@Data}", RequestId, Data);
            await HubContext.Clients.Group("RequestRealTime").SendAsync("UpdateRequest", RequestId, Data);
        }
    }
}
