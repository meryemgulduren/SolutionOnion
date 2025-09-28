using SO.Application.DTOs.Dashboard;

namespace SO.Application.Features.Queries.DashboardModule.GetDashboardStatistics
{
    public class GetDashboardStatisticsQueryResponse
    {
        public DashboardStatisticsDto Statistics { get; set; } = new DashboardStatisticsDto();
    }
}
