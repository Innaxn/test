using System.Data;

namespace OrderMonitoring.Infrastructure
{
    public interface IEximiusDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
    }
}
