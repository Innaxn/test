using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public class InMemoryOrderStore : IOrderStore
    {
        private readonly Dictionary<Guid, OrderDto> _orders = new();
        private readonly HashSet<string> _alerts = new();

        private string AlertKey(Guid objId, int statusId, DateTime statusDate) =>
            $"{objId}_{statusId}_{statusDate:O}";

        public DateTime LastFetchedTime { get; set; } = new DateTime(2025, 3, 2, 0, 0, 0); //TODO chnage to now
        //public DateTime LastFetchedTime { get; set; } = DateTime.Now;
        public Task<OrderDto> GetOrderAsync(Guid objId)
        {
            _orders.TryGetValue(objId, out var order);
            return Task.FromResult(order);
        }

        public Task<bool> IsAlertSentAsync(Guid objId, int statusId, DateTime statusDate)
        {
            return Task.FromResult(_alerts.Contains(AlertKey(objId, statusId, statusDate)));
        }

        public Task MarkAlertSentAsync(Guid objId, int statusId, DateTime statusDate)
        {
            _alerts.Add(AlertKey(objId, statusId, statusDate));
            return Task.CompletedTask;
        }

        public Task SaveOrUpdateAsync(OrderDto order)
        {
            Console.WriteLine($"Order {order.LifeCycleStatusDate} is saved/updated in memory");
            _orders[order.ObjID] = order;
            return Task.CompletedTask;
        }
        public Task<List<OrderDto>> GetAllOrders()
        {
            return Task.FromResult(_orders.Values.ToList());
        }
    }
}
