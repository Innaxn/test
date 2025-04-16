namespace OrderMonitoring.Domain.Dtos
{
    public class OrderTrendSummaryDto
    {
        public int TotalOrderCount { get; set; }
        public double? OrderCountChangePercentage { get; set; }
    }
}
