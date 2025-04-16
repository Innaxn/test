using Hangfire;
using Microsoft.AspNetCore.Connections;
using OrderMonitoring.Business;
using OrderMonitoring.Domain.Thresholds;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Infrastructure.Repositories;
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
            services.AddHangfireServer();

            // Hangfire set up
            services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage()
            );

            string connectionString = Environment.GetEnvironmentVariable("EximiusDb")!;

            services.AddSingleton<IEximiusDbConnectionFactory>(new EximiusDbConnectionFactory(connectionString));
            services.AddScoped<IOrderEximiusRepository, OrderEximiusRepository>();

            // in memory thresholds and order store
            services.AddSingleton<IThresholdProvider, InMemoryThresholdProvider>();
            services.AddSingleton<IOrderStore, InMemoryOrderStore>();

            // Alert
            string webhookurl = Environment.GetEnvironmentVariable("WebhookUrl")!;
            services.AddSingleton<IAlertChannel, TeamsAlertChannel>();
            services.AddSingleton<IAlertChannel, SignalRAlertChannel>();
            services.AddSingleton<IAlertChannel, ConsoleAlertServiceS>();

            services.AddSingleton<AlertFactory>();
            services.AddSingleton<AlertManager>();
            services.AddSingleton<AlertQueue>();

            // services
            services.AddScoped<OrderMonitoringService>();
            services.AddScoped<DashboardService>();

            // Hangfire Recurring JOB for checking for abnormal orders and triggers alerting channels
            services.AddTransient<MonitoringAlertJob>();


            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy("DashboardPolicy",
                     policy =>
                     {
                         policy.WithOrigins("http://localhost:3000")
                             .AllowAnyMethod()
                             .AllowAnyHeader()
                             .AllowCredentials();
                     });
            });

            // old background services
            //services.AddHostedService<MonitoringJob>();
            //services.AddHostedService<AlertProcessingService>();
            //services.AddHostedService<StatusCheckerService>();
        }
    }
}
