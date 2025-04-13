using OrderMonitoring.Model;
using System.Collections.Concurrent;

namespace OrderMonitoring.Business
{
    public class AlertQueue
    {
        private readonly ConcurrentQueue<AlertMessage> _alerts = new ConcurrentQueue<AlertMessage>();

        public async Task EnqueueAlert(AlertMessage alert)
        {
            _alerts.Enqueue(alert);
        }

        public bool TryDequeueAlert(out AlertMessage alert)
        {
            return _alerts.TryDequeue(out alert);
        }
    }
}
