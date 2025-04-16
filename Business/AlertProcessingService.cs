//namespace OrderMonitoring.Business
//{
//    // dequeues alerts from the queue and sends them to the alert manager
//    public class AlertProcessingService : BackgroundService
//    {
//        private readonly AlertQueue _alertQueue;
//        private readonly AlertManager _alertManager;

//        public AlertProcessingService(AlertQueue alertQueue, AlertManager alertManager)
//        {
//            _alertQueue = alertQueue;
//            _alertManager = alertManager;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                var tasks = new List<Task>();
//                while (_alertQueue.TryDequeueAlert(out var alert))
//                {
//                    tasks.Add(Task.Run(() => _alertManager.TriggerAlerts(alert)));
//                }
//                await Task.WhenAll(tasks);
//                await Task.Delay(1000);
//                Console.WriteLine($"Alerts sent.. hopefully");
//            }
//        }
//    }
//}
