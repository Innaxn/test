using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public static class QueryProvider
    {
        public static readonly Dictionary<QueryType, string> QueryRequest = new Dictionary<QueryType, string>{
            {
                QueryType.GetAllActiveOrders,
                @"select ObjID, InternalBlockOrderNumber, InternalOrderNumber, LifeCycleStatusDate, LifeCycleStatusID
                from StockOrder (NOLOCK) so 
                WHERE so.LifeCycleStatusID IN (5, 11, 12, 14, 34, 35, 36, 47 ) and  so.LifeCycleStatusDate between '2025-02-01 00:00:00.000' and '2025-03-01 00:00:00.000'
                ORDER BY so.LifeCycleStatusDate DESC;"
//and  so.LifeCycleStatusDate between '2025-02-01 00:00:00.000' and '2025-03-01 00:00:00.000'
            },
            {
                QueryType.GetActiveOrdersByStatusCount,
                @"select pol.Caption as StatusCaption, COUNT(*) AS Count
                    from StockOrder (NOLOCK) so
                    inner join [par_OrderLifeCycleStatus] pol (nolock) on pol.CodeId = so.LifeCycleStatusID and pol.CodeLanguage = 'EN' and pol.CodeCountry = 'GB'
                    GROUP BY pol.Caption;"
            },
            {
                QueryType.GetOrdersSinceLastFetch,
                @"select ObjID, InternalBlockOrderNumber, InternalOrderNumber, LifeCycleStatusDate, LifeCycleStatusID
                    from StockOrder (NOLOCK) so
                    WHERE so.LifeCycleStatusDate >= @lastTimeFetched
                    ORDER BY so.LifeCycleStatusDate DESC;"
            }
        };
    }
}
