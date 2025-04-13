using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Infrastructure.SignalR.Hubs;
using OrderMonitoring.Model;

namespace OrderMonitoring.Business
{
    // fetches orders and stores them in memory
    public class MonitoringJob : BackgroundService
    {
        //private readonly IOrderFetcher _orderFetcher;
        private readonly IThresholdProvider _thresholdProvider;
        //private readonly IAlertService _alertService;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _initialFetchdone = false;
        private readonly IOrderStore _orderStore;
        private readonly IAlertChannel _alertChannel;
        private readonly AlertQueue _alertQueue;
        private readonly IHubContext<OrderHub> _hubContext;

        public MonitoringJob(IThresholdProvider thresholdProvider, IServiceScopeFactory scopeFactory, IOrderStore orderStore, IAlertChannel alertChannel, AlertQueue alertQueue, IHubContext<OrderHub> hubContext)
        {
            _thresholdProvider = thresholdProvider;
            _scopeFactory = scopeFactory;
            _orderStore = orderStore;
            _alertChannel = alertChannel;
            _alertQueue = alertQueue;
            _hubContext = hubContext;
        }

        //[AutomaticRetry(Attempts=0)]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var orderEximiusRepository = scope.ServiceProvider.GetRequiredService<IOrderEximiusRepository>();
                Console.WriteLine("Polling for new orders...");
                //IEnumerable<OrderDto> orders;
                List<OrderDto> orders;
                //List<OrderDto> orders = (await _orderStore.GetAllOrders()).ToList();


                if (!_initialFetchdone)
                {
                    orders = (await orderEximiusRepository.GetOrdersAsync()).ToList();
                    _initialFetchdone = true;
                    Console.WriteLine("Initial fetch done...");
                }
                else
                {
                    var since = _orderStore.LastFetchedTime;

                    orders = (await orderEximiusRepository.GetOrdersSinceAsync(since)).ToList();
                    Console.WriteLine($"fetched order since {since}");
                    _orderStore.LastFetchedTime = DateTime.Now;
                }

                foreach (var order in orders)
                {
                    Console.WriteLine($"{orders.Count()} Order in monitoring job");
                    await _orderStore.SaveOrUpdateAsync(order);
                    await _hubContext.Clients.All.SendAsync("OrderUpdated", order);
                }

                await Task.Delay(1000);
                Console.WriteLine($"Order fetched");
            }
        }
    }
}
