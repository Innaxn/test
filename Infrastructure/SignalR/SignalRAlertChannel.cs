using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Infrastructure.SignalR.Hubs;
using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure.SignalR
{
    public class SignalRAlertChannel : IAlertChannel
    {
        public readonly IHubContext<AlertHub, IAlertClient> _hubContext;
        public readonly ILogger<SignalRAlertChannel> _logger;
        public SignalRAlertChannel(IHubContext<AlertHub, IAlertClient> hubContext, ILogger<SignalRAlertChannel> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendAlertAsync(AlertMessage message)
        {
            try
            {
                //await _hubContext.Clients.All.SendAsync("ReceiveAlert", message);
                await _hubContext.Clients.All.ReceiveAlert(message);
                _logger.LogInformation($"Alert sent thorugh SignalR: {message.Content}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error sending alert with SignalR");
            }
        }

    }
}
