using OrderMonitoring.Infrastructure;
using OrderMonitoring.Model;

namespace OrderMonitoring.Business
{
    public class OrderMonitoringService
    {
        private readonly IOrderEximiusRepository _orderEximiusRepository;
        public OrderMonitoringService(IOrderEximiusRepository orderEximiusRepository)
        {
            _orderEximiusRepository = orderEximiusRepository;
        }
        public async Task<OrderDto?> GetOrderByIdAsync(int number)
        {
            return await _orderEximiusRepository.GetOrderByIdAsync(number);
        }
    }
}
