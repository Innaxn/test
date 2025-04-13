namespace OrderMonitoring.Model
{
    public interface IAlertChannel
    {
        Task SendAlertAsync(AlertMessage message);
    }
}
