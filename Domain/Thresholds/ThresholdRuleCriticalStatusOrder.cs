namespace OrderMonitoring.Domain.Thresholds
{
    public class ThresholdRuleCriticalStatusOrder : ThresholdRule
    {
        public int Id { get; set; }
        public string StatusCaption { get; set; }
    }
}
