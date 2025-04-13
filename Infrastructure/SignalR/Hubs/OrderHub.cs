using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure.SignalR.Hubs
{
    public class OrderHub: Hub
    {
        //public async Task SendOrderUpdate(OrderDto order)
        //{
        //    await Clients.All.SendAsync("ReceiveOrderUpdate", order);
        //}
    }
}
