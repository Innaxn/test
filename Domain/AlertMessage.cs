namespace OrderMonitoring.Model
{
    public class AlertMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public AlertSeverity Severity { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
