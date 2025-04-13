using Microsoft.AspNetCore.Connections;
using OrderMonitoring.Business;
using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Infrastructure.SignalR;
using OrderMonitoring.Model;

namespace OrderMonitoring
{
    public class DependencyInjection
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            Register(services);
        }

        private static void Register(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddSignalR();

            string connectionString = Environment.GetEnvironmentVariable("EximiusDb")!;

            services.AddSingleton<IEximiusDbConnectionFactory>(new EximiusDbConnectionFactory(connectionString));
            services.AddScoped<IOrderEximiusRepository, OrderEximiusRepository>();
            
            string webhookurl = Environment.GetEnvironmentVariable("WebhookUrl")!;
            services.AddSingleton<IThresholdProvider, InMemoryThresholdProvider>();
            services.AddSingleton<IAlertChannel, TeamsAlertChannel>();
            services.AddSingleton<IAlertChannel, SignalRAlertChannel>();
            services.AddSingleton<IAlertChannel, ConsoleAlertServiceS>();
            services.AddSingleton<AlertFactory>();

            services.AddSingleton<AlertQueue>();
            services.AddSingleton<AlertManager>();

            //services.AddSingleton<IAlertService, ConsoleAlertService>();
            services.AddSingleton<IOrderStore, InMemoryOrderStore>();

            services.AddHostedService<MonitoringJob>();

            services.AddScoped<OrderMonitoringService>();
            services.AddHostedService<AlertProcessingService>();
            services.AddHostedService<StatusCheckerService>();
            //services.AddHostedService<OrderPollingService>();
        }
    }
}
