using OrderMonitoring.Domain.Dtos;
using OrderMonitoring.Infrastructure;

namespace OrderMonitoring.Business
{
    public class DashboardService
    {
        private readonly IOrderEximiusRepository _orderEximiusRepository;

        public DashboardService(IOrderEximiusRepository orderEximiusRepository)
        {
            _orderEximiusRepository = orderEximiusRepository;
        }

        public async Task<OrderTrendResponseDto> GetOrdersTrends()
        {
            var rawData = await _orderEximiusRepository.GetOrdersTrends();
            var daily = rawData
                .Where(x => x.RowType == "Daily")
                .Select(x => new DailyOrderDto
                {
                    OrderDate = x.OrderDate!.Value,
                    OrderCount = x.OrderCount,
                })
                .ToList();

            var summaryDate = rawData.FirstOrDefault(x => x.RowType == "Summary");
            var summary = summaryDate != null ? new OrderTrendSummaryDto
            {
                TotalOrderCount = summaryDate.OrderCount,
                OrderCountChangePercentage = summaryDate.OrderCountChangePercentage,
            } : new OrderTrendSummaryDto
            {
                TotalOrderCount = 0,
                OrderCountChangePercentage = null,
            };

            return new OrderTrendResponseDto
            {
                DailyOrders = daily,
                Summary = summary,
            };
        }

        public async Task<List<OrderCountStatusDTO>> GetOrdersAmountPerStatus(DateTime? startDate, DateTime? endDate)
        {
            DateTime effectiveStartDate;
            DateTime effectiveEndDate;

            if (!startDate.HasValue)
            {
                effectiveStartDate = DateTime.Today;
                effectiveEndDate = DateTime.Today.AddDays(1).AddTicks(-1);
            }
            else
            {
                effectiveStartDate = startDate.Value;

                if (!endDate.HasValue)
                {
                    effectiveEndDate = effectiveStartDate.Date.AddDays(1).AddTicks(-1);
                }
                else
                {
                    if (endDate.Value.TimeOfDay.TotalSeconds == 0)
                    {
                        effectiveEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                    }
                    else
                    {
                        effectiveEndDate = endDate.Value;
                    }
                }
            }

            if (effectiveStartDate > effectiveEndDate)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            var result = await _orderEximiusRepository.GetOrdersAmountPerStatus(effectiveStartDate, effectiveEndDate);

            return result.ToList();
        }
    }
}
