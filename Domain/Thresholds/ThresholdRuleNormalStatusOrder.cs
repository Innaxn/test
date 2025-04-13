namespace OrderMonitoring.Domain.Thresholds
{
    public class ThresholdRuleNormalStatusOrder : ThresholdRule
    {
        public int Id { get; set; }
        public string StatusCaption { get; set; }
        public TimeSpan StatusDurationThreshold { get; set; }
    }
}
