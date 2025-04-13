using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Model;

namespace OrderMonitoring.Business
{
    public class StatusCheckerService : BackgroundService
    {
        private readonly IThresholdProvider _thresholdProvider;
        private readonly IOrderStore _orderStore;
        private readonly AlertQueue _alertQueue;

        public StatusCheckerService(IThresholdProvider thresholdProvider, IOrderStore orderStore, AlertQueue alertQueue)
        {
            _thresholdProvider = thresholdProvider;
            _orderStore = orderStore;
            _alertQueue = alertQueue;
        }

        //[AutomaticRetry(Attempts=0)]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                IEnumerable<OrderDto> orders = await _orderStore.GetAllOrders();

                var now = DateTime.Now;

                var alertTasks = orders
                    .Select(async order =>
                    {
                        var rule = _thresholdProvider.GetRule(order.LifeCycleStatusID);
                        if (rule == null) return;

                        bool alreadyAlerted = await _orderStore.IsAlertSentAsync(order.ObjID, order.LifeCycleStatusID, order.LifeCycleStatusDate);

                        if (alreadyAlerted) return;

                        if (rule is ThresholdRuleCriticalStatusOrder)
                        {
                            // await _alertService.TriggerAlerAsync(order, "Instant alert");
                            //await _alertChannel.SendAlertAsync(new AlertMessage
                            //{
                            //    Id = order.ObjID,
                            //    Content = $"Order {order.InternalOrderNumber} with status {rule.StatusCaption} is in a bad state.",
                            //    Severity = AlertSeverity.Critical,
                            //    Timestamp = DateTime.Now
                            //});
                            // put alert in queue
                            await _alertQueue.EnqueueAlert(new AlertMessage
                            {
                                Id = order.ObjID,
                                Content = $"Order {(order.InternalOrderNumber == null ? order.InternalBlockOrderNumber : order.InternalOrderNumber)} with status {rule.StatusCaption} is in a bad state.",
                                Severity = AlertSeverity.Critical,
                                Timestamp = DateTime.Now
                            });

                            await _orderStore.MarkAlertSentAsync(order.ObjID, order.LifeCycleStatusID, order.LifeCycleStatusDate);
                            return;
                        }
                        else if (rule is ThresholdRuleNormalStatusOrder normalRule)
                        {
                            var timeInStatus = now - order.LifeCycleStatusDate;
                            if (timeInStatus > normalRule.StatusDurationThreshold)
                            {
                                //await _alertService.TriggerAlerAsync(order, $"Order has been in status {rule.StatusCaption} for {timeInStatus.TotalMinutes} minutes, exceeding the threshold of {rule.MaxDuration.Value.TotalMinutes} minutes.");
                                //await _alertChannel.SendAlertAsync(new AlertMessage
                                //{
                                //    Id = order.ObjID,
                                //    Content = $"Order {order.InternalOrderNumber} with status {rule.StatusCaption} is in a bad state.",
                                //    Severity = AlertSeverity.Critical,
                                //    Timestamp = DateTime.Now
                                //});
                                // put alert in queue
                                await _alertQueue.EnqueueAlert(new AlertMessage
                                {
                                    Id = order.ObjID,
                                    Content = $"Order {(order.InternalOrderNumber == null ? order.InternalBlockOrderNumber : order.InternalOrderNumber)} with status {rule.StatusCaption} is in a bad state.",
                                    Severity = AlertSeverity.Critical,
                                    Timestamp = DateTime.Now
                                });
                                await _orderStore.MarkAlertSentAsync(order.ObjID, order.LifeCycleStatusID, order.LifeCycleStatusDate);
                            }
                            //await _orderStore.SaveOrUpdateAsync(order);
                        }
                    });

                await Task.WhenAll(alertTasks);
                //_orderStore.LastFetchedTime = now;
                await Task.Delay(1000);
                Console.WriteLine($"And again checking alert");
            }
        }
    }
}
