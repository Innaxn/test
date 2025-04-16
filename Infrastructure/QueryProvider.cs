using OrderMonitoring.Model;

namespace OrderMonitoring.Infrastructure
{
    public static class QueryProvider
    {
        public static readonly Dictionary<QueryType, string> QueryRequest = new Dictionary<QueryType, string>{
            {
                QueryType.GetOrdersAmountPerStatus,
                @"SELECT
                   pol.Caption AS StatusCaption,
                   COUNT(*) AS Count
                   FROM
                       StockOrder (NOLOCK) so
                   INNER JOIN
                       [par_OrderLifeCycleStatus] pol (NOLOCK)
                       ON pol.CodeId = so.LifeCycleStatusID
                       AND pol.CodeLanguage = 'EN'
                       AND pol.CodeCountry = 'GB'
                   WHERE
                       so.LifeCycleStatusDate BETWEEN
                           ISNULL(@startDate, DATEADD(DAY, -30, GETDATE())) AND
                           ISNULL(@endDate, GETDATE())
                   GROUP BY pol.Caption"
            },
            {
                QueryType.GetOrdersWithTimeStatus,
                @"select 
                    so.ObjID, 
                    so.InternalBlockOrderNumber, 
                    so.InternalOrderNumber, 
                    so.LifeCycleStatusDate, 
                    so.LifeCycleStatusID,
                    pol.caption as StatusCaption,
                    DATEDIFF(MINUTE, so.LifeCycleStatusDate, GETDATE()) as MinutesInStatus  
                from StockOrder (NOLOCK) so
                inner join [par_OrderLifeCycleStatus] pol (nolock) 
                on pol.CodeId = so.LifeCycleStatusID 
                and pol.CodeLanguage = 'EN' 
                and pol.CodeCountry = 'GB'
                WHERE so.LifeCycleStatusID IN (5, 11, 12, 14, 34, 35, 36, 47 )
                and so.LifeCycleStatusDate >= @lastTimeFetched
                ORDER BY so.LifeCycleStatusDate DESC;"
            },
            {
                QueryType.GetOrdersTrend,
                @"WITH DailyOrderCounts AS (
                   SELECT
                       CAST(so.LifeCycleStatusDate AS DATE) AS OrderDate,
                       COUNT(*) AS OrderCount
                   FROM StockOrder so
                   WHERE so.LifeCycleStatusDate >= DATEADD(DAY, -60, GETDATE())
                   GROUP BY CAST(so.LifeCycleStatusDate AS DATE)
                ),
                LabeledPeriods AS (
                   SELECT
                       OrderDate,
                       OrderCount,
                       CASE
                           WHEN OrderDate >= CAST(DATEADD(DAY, -30, GETDATE()) AS DATE) THEN 'Current30'
                           WHEN OrderDate >= CAST(DATEADD(DAY, -60, GETDATE()) AS DATE)
                                AND OrderDate < CAST(DATEADD(DAY, -30, GETDATE()) AS DATE) THEN 'Previous30'
                       END AS Period
                   FROM DailyOrderCounts
                ),
                AggregatedCounts AS (
                   SELECT
                       Period,
                       SUM(OrderCount) AS TotalOrders
                   FROM LabeledPeriods
                   WHERE Period IS NOT NULL
                   GROUP BY Period
                )
                SELECT
                   RowType,
                   OrderDate,
                   OrderCount,
                   OrderCountChangePercentage
                FROM (
                   -- Daily counts for each day in the current 30-day window
                   SELECT
                       'Daily' AS RowType,
                       OrderDate,
                       OrderCount,
                       NULL AS OrderCountChangePercentage
                   FROM LabeledPeriods
                   WHERE Period = 'Current30'
                   UNION ALL
                   -- Summary row with total and percentage change
                   SELECT
                       'Summary' AS RowType,
                       NULL AS OrderDate,
                       c.TotalOrders AS OrderCount,
                       ROUND(
                           CASE
                               WHEN p.TotalOrders = 0 THEN NULL
                               ELSE (CAST(c.TotalOrders AS FLOAT) - p.TotalOrders) / p.TotalOrders * 100
                           END, 2
                       ) AS OrderCountChangePercentage
                   FROM AggregatedCounts c
                   JOIN AggregatedCounts p ON c.Period = 'Current30' AND p.Period = 'Previous30'
                ) AS CombinedResult
                ORDER BY
                   CASE WHEN RowType = 'Daily' THEN 0 ELSE 1 END,
                   OrderDate;"
            }
        };
    }
}
