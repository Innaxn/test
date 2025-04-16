using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderMonitoring.Infrastructure.SignalR.Hubs;
using OrderMonitoring.Model;

namespace OrderMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<AlertHub, IAlertClient> _hubContext;

        public TestController(IHubContext<AlertHub, IAlertClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Post()
        {
            var alert = new AlertMessage
            {
                Id = Guid.NewGuid(),
                Content = "This is a server-generated alert",
                Severity = AlertSeverity.Critical,
                Timestamp = DateTime.Now
            };


            await _hubContext.Clients.All.ReceiveAlert(alert);
            return Ok(alert);
        }
    }
}
