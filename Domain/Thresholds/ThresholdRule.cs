using System.Data;
using OrderMonitoring.Business;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Model;

namespace OrderMonitoring.Domain.Thresholds
{
    public interface ThresholdRule 
    {
        public int Id { get; set; }
        public string StatusCaption { get; set; }
    }

    //public class AlertState
    //{
    //    public string ObjID { get; set; } = string.Empty;
    //    public int StatusId { get; set; }
    //    public DateTime StatusDate { get; set; }
    //    public bool IsAlertSent { get; set; }
    //}

    //public interface IAlertService
    //{
    //    Task TriggerAlerAsync(OrderDto entry, string reason);
    //}

    //public class ConsoleAlertService : IAlertService
    //{
    //    public Task TriggerAlerAsync(OrderDto entry, string reason)
    //    {
    //        Console.WriteLine($"Alert: Order {(entry.InternalOrderNumber != 0 ? entry.InternalOrderNumber : entry.InternalBlockOrderNumber)} with status {entry.LifeCycleStatusID} has exceeded the threshold. Reason: {reason}");
    //        return Task.CompletedTask;
    //    }
    //}

    /// <summary>
    /// Console alert service  just for testing
    /// </summary>
    public class ConsoleAlertServiceS : IAlertChannel
    {
        public Task SendAlertAsync(AlertMessage message)
        {
            Console.WriteLine($"Alert: Order {message.Content} ");
            return Task.CompletedTask;
        }

    }
}

