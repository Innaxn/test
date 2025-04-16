using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure.Repositories
{
    public class InMemoryOrderStore : IOrderStore
    {
        private readonly Dictionary<Guid, OrderDtoWithTime> _orders = new();
        private readonly HashSet<string> _alerts = new();

        private string AlertKey(Guid objId, int statusId, DateTime statusDate) =>
            $"{objId}_{statusId}_{statusDate:O}";

        public DateTime LastFetchedTime { get; set; } = DateTime.Now.AddDays(-14); // initial fetch
        //public DateTime LastFetchedTime { get; set; } = DateTime.Now;
        public Task<OrderDtoWithTime> GetOrderAsync(Guid objId)
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

        public Task SaveOrUpdateAsync(OrderDtoWithTime order)
        {
            Console.WriteLine($"Order {order.LifeCycleStatusDate} is saved/updated in memory");
            _orders[order.ObjID] = order;
            return Task.CompletedTask;
        }
        public Task<List<OrderDtoWithTime>> GetAllOrders()
        {
            return Task.FromResult(_orders.Values.ToList());
        }

        public Task RemoveOrdersOlderThanAsync(DateTime cutoff)
        {
            var keysToRemove = _orders
                .Where(kvp => kvp.Value.LifeCycleStatusDate < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();
            foreach(var key in keysToRemove)
            {
                _orders.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task CleanupAlertHistoryOrderThatAsync(DateTime cutoff)
        {
            var keysToRemove = _alerts
                 .Where(k => {
                   var parts = k.Split('_');
                     if (parts.Length > 2 && DateTime.TryParse(parts[2], out var date))
                     {
                         return date < cutoff;
                     }
                     return false;
                 })
                .ToList();

            foreach (var key in keysToRemove)
            {
                _alerts.Remove(key);
            }
            return Task.CompletedTask;
        }
    }
}
