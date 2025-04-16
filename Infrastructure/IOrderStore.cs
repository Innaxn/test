using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public interface IOrderStore
    {
        Task<OrderDtoWithTime> GetOrderAsync(Guid objId);
        Task SaveOrUpdateAsync(OrderDtoWithTime order);
        Task<bool> IsAlertSentAsync(Guid objId, int statusId, DateTime statusDate);
        Task MarkAlertSentAsync(Guid objId, int statusId, DateTime statusDate);

        Task<List<OrderDtoWithTime>> GetAllOrders();
        DateTime LastFetchedTime { get; set; }
        Task RemoveOrdersOlderThanAsync(DateTime cutoff);
        Task CleanupAlertHistoryOrderThatAsync(DateTime cutoff);

    }
}
