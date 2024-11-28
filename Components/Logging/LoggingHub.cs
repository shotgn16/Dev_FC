using Microsoft.AspNetCore.SignalR;

namespace ForestChurches.Components.Logging
{
    public class LoggingHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("RecieveMessage", message);
        }
    }
}
