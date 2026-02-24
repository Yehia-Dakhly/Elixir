using Microsoft.AspNetCore.SignalR;

namespace Blood_Donation.Hubs
{
    public class RequestsHub(ILogger<RequestsHub> _logger) : Hub
    {
        public async Task JoinRequestRealTime()
        {
            _logger.LogInformation("Client with ConnectionId: {ConnectionId} is joining RequestsRealTime group.", Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "RequestsRealTime");
        }
    }
}
