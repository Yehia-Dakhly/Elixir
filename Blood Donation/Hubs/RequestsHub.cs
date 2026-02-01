using Microsoft.AspNetCore.SignalR;

namespace Blood_Donation.Hubs
{
    public class RequestsHub : Hub
    {
        public async Task JoinRequestRealTime()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "RequestsRealTime");
        }
    }
}
