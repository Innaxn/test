using OrderMonitoring.Model;
namespace OrderMonitoring.Infrastructure
{
    public interface IOrderEximiusRepository
    {
        Task<List<OrderDto>> GetOrdersAsync();
        Task<List<OrderDto>> GetOrdersSinceAsync(DateTime lastTimeFetched);
        Task<OrderDto?> GetOrderByIdAsync(int Number);

        Task<List<OrderCountStatusDTO>> GetOrderStatusCount();
    }
}
