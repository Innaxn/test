using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure.SignalR.Hubs
{
    public interface IAlertClient
    {
        Task ReceiveAlert(AlertMessage message);
    }

    public class AlertHub : Hub<IAlertClient>
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            //await Clients.All.ReceiveAlert($"ReceiveAlert", message);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
