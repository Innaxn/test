using DotNetEnv;
using OrderMonitoring;
using OrderMonitoring.Infrastructure.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    // laod env vars
    Env.Load();
}
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                 .AllowAnyMethod()
//                 .AllowAnyHeader();
//         });
//});


DependencyInjection.ConfigureServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

}
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AlertHub>("/hubs/alert-hub");
    endpoints.MapHub<OrderHub>("/hubs/order-hub");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.UseCors("AllowAll");

app.Run();
