using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Infrastructure.SignalR.Hubs;
using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure.SignalR
{
    public class SignalRAlertChannel : IAlertChannel
    {
        public readonly IHubContext<AlertHub> _hubContext;
        public readonly ILogger<SignalRAlertChannel> _logger;
        public SignalRAlertChannel(IHubContext<AlertHub> hubContext, ILogger<SignalRAlertChannel> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendAlertAsync(AlertMessage message)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveAlert", message);
                // _logger.LogInformation($"Alert sent: {message.Content}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error sending alert");
            }
        }
    }
}
