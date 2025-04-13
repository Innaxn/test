using OrderMonitoring.Model;

namespace OrderMonitoring.Business
{
    public class AlertManager
    {
        private readonly List<IAlertChannel> _alertChannels = new List<IAlertChannel>();

        public AlertManager(AlertFactory factory)
        {
            //var alertWays = new List<AlertEnum> { AlertEnum.MsTeams, AlertEnum.Console, AlertEnum.SignalR };
            var alertWays = new List<AlertEnum> {AlertEnum.Console, AlertEnum.MsTeams};

            foreach (var alertWay in alertWays)
            {
                var alertChannel = factory.CreateAlertChannel(alertWay);
                RegisterAlertChannel(alertChannel);
            }
        }

        private void RegisterAlertChannel(IAlertChannel alertChannel)
        {
            if (alertChannel is null)
            {
                throw new ArgumentNullException(nameof(alertChannel));
            }
            _alertChannels.Add(alertChannel);
        }

        public void TriggerAlerts(AlertMessage alertMessage)
        {
            foreach(var alertChannel in _alertChannels)
            {
                try
                {
                    alertChannel.SendAlertAsync(alertMessage).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending alert: {ex.Message}");
                }
            }
        }
    }
}
