using OrderMonitoring.Model;
using Dapper;
using Microsoft.AspNetCore.Connections;
using OrderMonitoring.Domain.Dtos;

namespace OrderMonitoring.Infrastructure.Repositories
{
    public class OrderEximiusRepository : IOrderEximiusRepository
    {
        private readonly IEximiusDbConnectionFactory _dbConnectionFactory;
        public OrderEximiusRepository(IEximiusDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        //public async Task<List<OrderDto>> GetOrdersAsync()
        //{
        //    using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        //    var sql = QueryProvider.QueryRequest[QueryType.GetAllActiveOrders];
        //    var result = await connection.QueryAsync<OrderDto>(sql);
        //    return result.ToList();

        //}
        public async Task<OrderDto?> GetOrderByIdAsync(int Number)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderCountStatusDTO>> GetOrdersAmountPerStatus(DateTime? startDate, DateTime? endDate)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var sql = QueryProvider.QueryRequest[QueryType.GetOrdersAmountPerStatus];
            var result = await connection.QueryAsync<OrderCountStatusDTO>(sql, new { startDate, endDate });
            return result.ToList();
        }

        public async Task<List<OrderDtoWithTime>> GetOrdersWithTimeStatusAsync(DateTime sinceLastFetched)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
           
            var sql = QueryProvider.QueryRequest[QueryType.GetOrdersWithTimeStatus];
            var result = await connection.QueryAsync<OrderDtoWithTime>(sql, new {lastTimeFetched = sinceLastFetched});
            return result.ToList();
        }

        public async Task<List<OrderTrendDto>> GetOrdersTrends()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            var sql = QueryProvider.QueryRequest[QueryType.GetOrdersTrend];
            var result = await connection.QueryAsync<OrderTrendDto>(sql);
            return result.ToList();
        }
    }
}
