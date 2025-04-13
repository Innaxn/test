using OrderMonitoring.Infrastructure;

namespace OrderMonitoring.Business
{
    public class OrderPollingService : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public OrderPollingService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var orderEximiusRepository = scope.ServiceProvider.GetRequiredService<IOrderEximiusRepository>();
                Console.WriteLine("Polling for new orders...");
                var orders = await orderEximiusRepository.GetOrderStatusCount();
                foreach (var order in orders)
                {
                    Console.WriteLine($"{order.Count} Order {order.StatusCaption} has {order.Count} orders");
                }

                await Task.Delay(1000);
                Console.WriteLine($"And again");
            }
        }
    }
}
