namespace OrderMonitoring.Model
{
    public enum QueryType
    {
        GetOrderById,
        GetOrdersAmountPerStatus,
        GetOrdersWithTimeStatus, // s razlika
        GetOrdersTrend
    }
}
