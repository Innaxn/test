using OrderMonitoring.Model;
using Dapper;
using Microsoft.AspNetCore.Connections;

namespace OrderMonitoring.Infrastructure
{
    public class OrderEximiusRepository : IOrderEximiusRepository
    {
        private readonly IEximiusDbConnectionFactory _dbConnectionFactory;
        public OrderEximiusRepository(IEximiusDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var sql = QueryProvider.QueryRequest[QueryType.GetAllActiveOrders];
            var result = await connection.QueryAsync<OrderDto>(sql);
            return result.ToList();

        }
        public async Task<OrderDto?> GetOrderByIdAsync(int Number)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderCountStatusDTO>> GetOrderStatusCount()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var sql = QueryProvider.QueryRequest[QueryType.GetActiveOrdersByStatusCount];
            var result = await connection.QueryAsync<OrderCountStatusDTO>(sql);
            return result.ToList();
        }

        public async Task<List<OrderDto>> GetOrdersSinceAsync(DateTime lastTimeFetched)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var sql = QueryProvider.QueryRequest[QueryType.GetOrdersSinceLastFetch];
            var result = await connection.QueryAsync<OrderDto>(sql, new { lastTimeFetched = lastTimeFetched });
            return result.ToList();
        }
    }
}
