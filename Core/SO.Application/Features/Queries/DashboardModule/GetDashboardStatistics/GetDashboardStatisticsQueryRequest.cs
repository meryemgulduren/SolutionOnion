using MediatR;

namespace SO.Application.Features.Queries.DashboardModule.GetDashboardStatistics
{
    public class GetDashboardStatisticsQueryRequest : IRequest<GetDashboardStatisticsQueryResponse>
    {
        public string? CurrentUserId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
