namespace OrderMonitoring.Domain.Dtos
{
    public class OrderTrendDto
    {
        public string RowType { get; set; }

        public DateTime? OrderDate { get; set; }
        public int OrderCount { get; set; }
        public double? OrderCountChangePercentage { get; set; }
    }
}
