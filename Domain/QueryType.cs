namespace OrderMonitoring.Model
{
    public enum QueryType
    {
        GetAllActiveOrders,
        GetOrderById,
        GetActiveOrdersByStatusCount,
        GetOrdersSinceLastFetch
    }
}
