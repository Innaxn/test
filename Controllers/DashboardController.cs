using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderMonitoring.Business;
using OrderMonitoring.Infrastructure;

namespace OrderMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly OrderMonitoringService _orderMonitoringService;
        private readonly DashboardService _dashboardService;
        private readonly IOrderStore _inMemoryOrderStore;

        public DashboardController(OrderMonitoringService orderMonitoringService, IOrderStore inMemoryOrderStore, DashboardService dashboardService)
        {
            _orderMonitoringService = orderMonitoringService;
            _inMemoryOrderStore = inMemoryOrderStore;
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInMemoryOrders()
        {
            var orders = await _inMemoryOrderStore.GetAllOrders();
            return Ok(orders);
        }
        [HttpGet("order")]
        // todo dont use in memory, but the eximius rep
        public async Task<IActionResult> GetOrderById(int number)
        {
            var order = await _orderMonitoringService.GetOrderByIdAsync(number);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        //Old
        //[HttpGet("status-count")]
        ////todo use exmius rep
        //public async Task<IActionResult> GetOrderCountByStatus()
        //{
        //    var order = await _inMemoryOrderStore.GetAllOrders();
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(order);
        //}

        [HttpGet("trends")]
        public async Task<IActionResult> GetOrdersTrends()
        {
            var orders = await _dashboardService.GetOrdersTrends();
           
            return Ok(orders);
        }

        [HttpGet("statusCounts")]
        public async Task<IActionResult> GetOrderStatusCounts([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _dashboardService.GetOrdersAmountPerStatus(startDate, endDate);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
