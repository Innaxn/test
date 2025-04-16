namespace OrderMonitoring.Domain.Dtos
{
    public class OrderTrendResponseDto
    {
        public List<DailyOrderDto> DailyOrders { get; set; }
        public OrderTrendSummaryDto Summary { get; set; }
    }
}
