using OrderMonitoring.Business;
using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public class MonitoringAlertJob
    {
        private readonly IOrderEximiusRepository _orderEximiusRepository; // eximius
        private readonly IOrderStore _orderStore; //In memeory order store
        private readonly IThresholdProvider _thresholdProvider; // thresholds
        private readonly AlertManager _alertManager; //alert instance
        private readonly ILogger<MonitoringAlertJob> _logger;


        public MonitoringAlertJob(IOrderEximiusRepository orderEximiusRepository, IOrderStore orderStore, IThresholdProvider thresholdProvider, AlertManager alertManager, ILogger<MonitoringAlertJob> logger)
        {
            _orderEximiusRepository = orderEximiusRepository;
            _orderStore = orderStore;
            _thresholdProvider = thresholdProvider;
            _alertManager = alertManager;
            _logger = logger;
        }

        public async Task ExecuteMonitoringCycleAsync()
        {
            try
            {
                _logger.LogInformation("Starting monitoring cycle...");
                DateTime cycleStartTime = DateTime.Now;

                // 1. Get order with time in status from eximius
                var orders = await _orderEximiusRepository.GetOrdersWithTimeStatusAsync(_orderStore.LastFetchedTime);
                _logger.LogInformation($"Fetched {orders.Count} orders from Eximius.");

                // 2. Process orders and check thresholds in one pass
                Dictionary<string, List<OrderDtoWithTime>> alertsByStatus = new();

                foreach (var order in orders)
                {
                    // Update in-memry store with order data
                    await _orderStore.SaveOrUpdateAsync(order);

                    //Check thresholds
                    var rule = _thresholdProvider.GetRule(order.LifeCycleStatusID);
                    if (rule != null)
                    {
                        bool alreadyAlerted = await _orderStore.IsAlertSentAsync(
                            order.ObjID, order.LifeCycleStatusID, order.LifeCycleStatusDate);

                        if (!alreadyAlerted)
                        {
                            bool shouldAlert = false;

                            if (rule is ThresholdRuleCriticalStatusOrder)
                            {
                                shouldAlert = true;
                            }
                            else if (rule is ThresholdRuleNormalStatusOrder normalRule)
                            {
                                int thresholdMinutes = (int)normalRule.StatusDurationThreshold.TotalMinutes;
                                if (order.MinutesInStatus > thresholdMinutes)
                                {
                                    shouldAlert = true;
                                }
                            }

                            if (shouldAlert)
                            {
                                // Group by status for batched alerts
                                if (!alertsByStatus.ContainsKey(rule.StatusCaption))
                                {
                                    alertsByStatus[rule.StatusCaption] = new List<OrderDtoWithTime>();
                                }
                                alertsByStatus[rule.StatusCaption].Add(order);

                                await _orderStore.MarkAlertSentAsync(
                                    order.ObjID, order.LifeCycleStatusID, order.LifeCycleStatusDate);
                            }
                        }
                    }
                }

                // 3. Send batched alerts
                List<Task> alertTasks = new();
                foreach (var statusGroup in alertsByStatus)
                {
                    var ordersList = statusGroup.Value;
                    var status = statusGroup.Key;

                    string content;
                    if (ordersList.Count == 1)
                    {
                        var order = ordersList[0];
                        var orderNumber = order.InternalOrderNumber ?? order.InternalBlockOrderNumber;
                        content = $"Order #{orderNumber} with status {status} is stuck for {order.MinutesInStatus} minutes.";
                    }
                    else
                    {
                        content = $"{ordersList.Count} orders with status {status} are stuck. Oldest older is stuck for: {ordersList.Max(o => o.MinutesInStatus)} minutes";
                    }

                    var alert = new AlertMessage
                    {
                        Id = Guid.NewGuid(),
                        Content = content,
                        Severity = AlertSeverity.Critical,
                        Timestamp = cycleStartTime
                    };

                    alertTasks.Add(_alertManager.TriggerAlerts(alert));
                }

                if (alertTasks.Any())
                {
                    await Task.WhenAll(alertTasks);
                    _logger.LogInformation($"Sent {alertTasks.Count} batch alerts.");
                }

                // 4. Update dashboard via signalr (maybe)

                // 5. Cleanup in-memory storage
                //await CleanupInMemoryStorage();

                // 6. Update last fetched time
                _orderStore.LastFetchedTime = cycleStartTime;

                _logger.LogInformation("Monitoring cycle completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during monitoring cycle");
            }
        }

        private async Task CleanupInMemoryStorage()
        {
            // Cleanup logic for in-memory storage
            var now = DateTime.Now;
            var staleCutOff = now.AddMinutes(-120); // Example: remove orders older than 30 minutes

            var alertsCutOff = now.AddMinutes(-120); // Example: remove alerts older than 30 minutes

            await _orderStore.RemoveOrdersOlderThanAsync(staleCutOff);
            await _orderStore.CleanupAlertHistoryOrderThatAsync(alertsCutOff);

        }
    }


    public class OrderDtoWithTime:OrderDto
    {
        public string StatusCaption { get; set; } = string.Empty;
        public int MinutesInStatus { get; set; }
    }
}
