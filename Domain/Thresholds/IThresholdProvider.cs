namespace OrderMonitoring.Domain.Thresholds
{
    public interface IThresholdProvider
    {
        ThresholdRule? GetRule(int statusId);
    }
}
