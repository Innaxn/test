using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public interface IOrderStore
    {
        Task<OrderDto> GetOrderAsync(Guid objId);
        Task SaveOrUpdateAsync(OrderDto order);
        Task<bool> IsAlertSentAsync(Guid objId, int statusId, DateTime statusDate);
        Task MarkAlertSentAsync(Guid objId, int statusId, DateTime statusDate);

        Task<List<OrderDto>> GetAllOrders();
        DateTime LastFetchedTime { get; set; }

    }
}
