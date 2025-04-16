using OrderMonitoring.Domain.Dtos;
using OrderMonitoring.Model;
namespace OrderMonitoring.Infrastructure
{
    public interface IOrderEximiusRepository
    {
        Task<OrderDto?> GetOrderByIdAsync(int Number);

        // this is for the dashboard
        Task<List<OrderCountStatusDTO>> GetOrdersAmountPerStatus(DateTime? startDate = null, DateTime? endDate = null);

        // this is for the abckground job
        Task<List<OrderDtoWithTime>> GetOrdersWithTimeStatusAsync(DateTime sinceLastFetched);

        // gets orders trends per 30 days compared to previous 30 days
        Task<List<OrderTrendDto>> GetOrdersTrends();
    }
}
