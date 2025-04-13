using OrderMonitoring.Domain.Thresholds;

namespace OrderMonitoring.Infrastructure
{
    public class InMemoryThresholdProvider : IThresholdProvider
    {
        private readonly Dictionary<int, ThresholdRule> _rules = new()
        {
            { 11, new ThresholdRuleNormalStatusOrder { Id = 11, StatusCaption = "Ready to be sent to OMS", StatusDurationThreshold = TimeSpan.FromMinutes(1)} },
            { 34, new ThresholdRuleNormalStatusOrder { Id = 34, StatusCaption = "Pending New", StatusDurationThreshold = TimeSpan.FromMinutes(3) } },
            { 12, new ThresholdRuleNormalStatusOrder { Id = 12, StatusCaption = "Sent to OMS", StatusDurationThreshold = TimeSpan.FromMinutes(1)} },
            { 36, new ThresholdRuleNormalStatusOrder { Id = 36, StatusCaption = "Pending Cancelled", StatusDurationThreshold = TimeSpan.FromMinutes(1) } },
            { 35, new ThresholdRuleNormalStatusOrder { Id = 35, StatusCaption = "Pending Update", StatusDurationThreshold = TimeSpan.FromMinutes(2)} },
            { 47, new ThresholdRuleCriticalStatusOrder { Id = 47, StatusCaption = "Executed with Errors"} }
        };

        public ThresholdRule? GetRule(int statusId) => _rules.TryGetValue(statusId, out var rule) ? rule : null;

    }
}
