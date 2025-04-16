using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Infrastructure.SignalR;
using OrderMonitoring.Infrastructure.SignalR.Hubs;
using OrderMonitoring.Model;

namespace OrderMonitoring.Business
{
    public class AlertFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AlertFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAlertChannel CreateAlertChannel(AlertEnum alertType)
        {
            return alertType switch
            {
                AlertEnum.MsTeams => new TeamsAlertChannel(
                    _serviceProvider.GetRequiredService<IHttpClientFactory>()
                ),
                AlertEnum.SignalR => new SignalRAlertChannel(
                    _serviceProvider.GetRequiredService<IHubContext<AlertHub, IAlertClient>>(),
                    _serviceProvider.GetRequiredService<ILogger<SignalRAlertChannel>>()
                ),
                AlertEnum.Console => new ConsoleAlertServiceS(),
                _ => throw new ArgumentException("Invalid alert type")
            };
        }
    }
}
