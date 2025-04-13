using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderMonitoring.Business;

namespace OrderMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderMonitoringService _orderMonitoringService;

        public OrdersController(OrderMonitoringService orderMonitoringService)
        {
            _orderMonitoringService = orderMonitoringService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderMonitoringService.GetOrdersAsync();
            return Ok(orders);
        }
    }
}
