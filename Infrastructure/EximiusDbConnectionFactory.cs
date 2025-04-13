
using System.Data;
using Microsoft.Data.SqlClient;

namespace OrderMonitoring.Infrastructure
{
    public class EximiusDbConnectionFactory : IEximiusDbConnectionFactory
    {
        private readonly string _connectionString;

        public EximiusDbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.");
            }
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
        {
            var connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync(token);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Failed to make a database connection.", exception);
            }
            return connection;
        }
    }
}
