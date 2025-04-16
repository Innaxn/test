using DotNetEnv;
using Hangfire;
using OrderMonitoring;
using OrderMonitoring.Infrastructure;
using OrderMonitoring.Infrastructure.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    // laod env vars
    Env.Load();
}

DependencyInjection.ConfigureServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
app.UseRouting();

RecurringJob.AddOrUpdate<MonitoringAlertJob>(
    "order-polling-job",
    job => job.ExecuteMonitoringCycleAsync(),
    Cron.MinuteInterval(1));

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AlertHub>("/alert");
app.UseCors("DashboardPolicy");
app.Run();
