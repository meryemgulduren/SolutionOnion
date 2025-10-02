using MediatR;
using SO.Application.DTOs.Dashboard;
using SO.Application.Features.Queries.DashboardModule.GetDashboardStatistics;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.DTOs.AccountModule.Account;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SO.Application.Features.Queries.DashboardModule.GetDashboardStatistics
{
    public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQueryRequest, GetDashboardStatisticsQueryResponse>
    {
        private readonly IProposalService _proposalService;
        private readonly IAccountService _accountService;
        private readonly ILogger<GetDashboardStatisticsQueryHandler> _logger;

        public GetDashboardStatisticsQueryHandler(IProposalService proposalService, IAccountService accountService, ILogger<GetDashboardStatisticsQueryHandler> logger)
        {
            _proposalService = proposalService;
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<GetDashboardStatisticsQueryResponse> Handle(GetDashboardStatisticsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting dashboard statistics generation for user: {UserId}, IsAdmin: {IsAdmin}", 
                    request.CurrentUserId, request.IsAdmin);

                // Kullanıcıya göre veri filtreleme
                var allProposals = await _proposalService.GetAllProposalsAsync();
                var allAccounts = await _accountService.GetAllAccountsAsync();

                // Dashboard'da sadece kullanıcının kendi verilerini göster
                var userProposals = allProposals?.Where(p => p.CreatedById == request.CurrentUserId).ToList() ?? new List<ListProposal>();
                var userAccounts = allAccounts?.Where(a => a.CreatedById == request.CurrentUserId).ToList() ?? new List<ListAccount>();

                _logger.LogInformation($"Retrieved {userProposals.Count} user proposals and {userAccounts.Count} user accounts");

                var dashboardStats = new DashboardStatisticsDto
                {
                    DraftProposalsCount = userProposals.Count(p => p.Status == "Draft"),
                    SentProposalsCount = userProposals.Count(p => p.Status == "Sent"),
                    ApprovedProposalsCount = userProposals.Count(p => p.Status == "Approved"),
                    RejectedProposalsCount = userProposals.Count(p => p.Status == "Rejected"),
                    CancelledProposalsCount = userProposals.Count(p => p.Status == "Cancelled"),
                    TotalRevenue = userProposals.Sum(p => p.TotalAmount),
                    BusinessActivities = await GenerateBusinessActivitiesAsync(userProposals, userAccounts)
                };

                var totalProposals = dashboardStats.DraftProposalsCount + dashboardStats.SentProposalsCount + dashboardStats.ApprovedProposalsCount + dashboardStats.RejectedProposalsCount + dashboardStats.CancelledProposalsCount;
                _logger.LogInformation($"Generated dashboard stats: {totalProposals} proposals (Draft:{dashboardStats.DraftProposalsCount}, Sent:{dashboardStats.SentProposalsCount}, Approved:{dashboardStats.ApprovedProposalsCount}, Rejected:{dashboardStats.RejectedProposalsCount}, Cancelled:{dashboardStats.CancelledProposalsCount}), {dashboardStats.BusinessActivities.Count} activities");

                return new GetDashboardStatisticsQueryResponse
                {
                    Statistics = dashboardStats
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics");
                // Hata durumunda boş istatistik döndür
                return new GetDashboardStatisticsQueryResponse
                {
                    Statistics = new DashboardStatisticsDto()
                };
            }
        }

        private async Task<List<BusinessActivityItem>> GenerateBusinessActivitiesAsync(IEnumerable<ListProposal> proposals, IEnumerable<ListAccount> accounts)
        {
            var activities = new List<BusinessActivityItem>();
            var now = DateTime.UtcNow;

            _logger.LogInformation($"GenerateBusinessActivitiesAsync started. Proposals: {proposals?.Count() ?? 0}, Accounts: {accounts?.Count() ?? 0}");

            try
            {
                if (proposals != null && proposals.Any())
                {
                    _logger.LogInformation($"Processing {proposals.Count()} proposals for business activities");

                    // Son oluşturulan proposal'lar (en son 5 tane)
                    var recentProposals = proposals
                        .OrderByDescending(p => p.CreatedDate)
                        .Take(5);

                    _logger.LogInformation($"Found {recentProposals.Count()} recent proposals");

                    foreach (var proposal in recentProposals)
                    {
                        var timeAgo = GetTimeAgo(proposal.CreatedDate, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = proposal.Id.ToString(),
                            Title = $"Proposal Created: {proposal.ProposalName}",
                            Description = $"New proposal for {proposal.CompanyName} has been created with status: {proposal.Status}",
                            IconClass = "fas fa-file-contract",
                            IconColor = "bg-primary",
                            ActivityDate = proposal.CreatedDate,
                            TimeAgo = timeAgo,
                            ActivityType = "Proposal",
                            RelatedEntityName = proposal.ProposalName,
                            Status = "Created",
                            UserName = proposal.PreparedBy ?? "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }

                    // Son güncellenen proposal'lar (en son 3 tane)
                    var updatedProposals = proposals
                        .Where(p => p.UpdatedDate.HasValue && p.UpdatedDate != p.CreatedDate)
                        .OrderByDescending(p => p.UpdatedDate)
                        .Take(3);

                    _logger.LogInformation($"Found {updatedProposals.Count()} updated proposals");

                    foreach (var proposal in updatedProposals)
                    {
                        var timeAgo = GetTimeAgo(proposal.UpdatedDate.Value, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = proposal.Id.ToString(),
                            Title = $"Proposal Updated: {proposal.ProposalName}",
                            Description = $"Proposal for {proposal.CompanyName} has been updated. Current status: {proposal.Status}",
                            IconClass = "fas fa-edit",
                            IconColor = "bg-info",
                            ActivityDate = proposal.UpdatedDate.Value,
                            TimeAgo = timeAgo,
                            ActivityType = "Proposal",
                            RelatedEntityName = proposal.ProposalName,
                            Status = "Updated",
                            UserName = proposal.PreparedBy ?? "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }

                    // Onaylanan proposal'lar (en son 2 tane)
                    var approvedProposals = proposals
                        .Where(p => p.Status == "Approved")
                        .OrderByDescending(p => p.UpdatedDate ?? p.CreatedDate)
                        .Take(2);

                    _logger.LogInformation($"Found {approvedProposals.Count()} approved proposals");

                    foreach (var proposal in approvedProposals)
                    {
                        var approvalDate = proposal.UpdatedDate ?? proposal.CreatedDate;
                        var timeAgo = GetTimeAgo(approvalDate, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = proposal.Id.ToString(),
                            Title = $"Proposal Approved: {proposal.ProposalName}",
                            Description = $"Proposal for {proposal.CompanyName} has been approved",
                            IconClass = "fas fa-check-circle",
                            IconColor = "bg-success",
                            ActivityDate = approvalDate,
                            TimeAgo = timeAgo,
                            ActivityType = "Proposal",
                            RelatedEntityName = proposal.ProposalName,
                            Status = "Approved",
                            UserName = proposal.PreparedBy ?? "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }

                    // Draft proposal'lar (en son 2 tane)
                    var draftProposals = proposals
                        .Where(p => p.Status == "Draft")
                        .OrderByDescending(p => p.CreatedDate)
                        .Take(2);

                    _logger.LogInformation($"Found {draftProposals.Count()} draft proposals");

                    foreach (var proposal in draftProposals)
                    {
                        var timeAgo = GetTimeAgo(proposal.CreatedDate, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = proposal.Id.ToString(),
                            Title = $"Draft Proposal: {proposal.ProposalName}",
                            Description = $"Draft proposal for {proposal.CompanyName} is ready for review",
                            IconClass = "fas fa-clock",
                            IconColor = "bg-warning",
                            ActivityDate = proposal.CreatedDate,
                            TimeAgo = timeAgo,
                            ActivityType = "Proposal",
                            RelatedEntityName = proposal.ProposalName,
                            Status = "Draft",
                            UserName = proposal.PreparedBy ?? "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }
                }
                else
                {
                    _logger.LogWarning("No proposals found in database");
                }

                if (accounts != null && accounts.Any())
                {
                    _logger.LogInformation($"Processing {accounts.Count()} accounts for business activities");

                    // Yeni eklenen müşteriler (en son 3 tane)
                    var newAccounts = accounts
                        .OrderByDescending(a => a.CreatedDate)
                        .Take(3);

                    _logger.LogInformation($"Found {newAccounts.Count()} new accounts");

                    foreach (var account in newAccounts)
                    {
                        var timeAgo = GetTimeAgo(account.CreatedDate, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = account.Id.ToString(),
                            Title = $"New Client: {account.CompanyName}",
                            Description = $"New strategic client {account.CompanyName} has been successfully onboarded",
                            IconClass = "fas fa-user-plus",
                            IconColor = "bg-warning",
                            ActivityDate = account.CreatedDate,
                            TimeAgo = timeAgo,
                            ActivityType = "Account",
                            RelatedEntityName = account.CompanyName,
                            Status = "Created",
                            UserName = "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }

                    // Aktif müşteriler (en son 2 tane)
                    var activeAccounts = accounts
                        .Where(a => a.IsActive)
                        .OrderByDescending(a => a.CreatedDate)
                        .Take(2);

                    _logger.LogInformation($"Found {activeAccounts.Count()} active accounts");

                    foreach (var account in activeAccounts)
                    {
                        var timeAgo = GetTimeAgo(account.CreatedDate, now);
                        var activity = new BusinessActivityItem
                        {
                            Id = account.Id.ToString(),
                            Title = $"Active Client: {account.CompanyName}",
                            Description = $"Client {account.CompanyName} is actively engaged in projects",
                            IconClass = "fas fa-users",
                            IconColor = "bg-success",
                            ActivityDate = account.CreatedDate,
                            TimeAgo = timeAgo,
                            ActivityType = "Account",
                            RelatedEntityName = account.CompanyName,
                            Status = "Active",
                            UserName = "System"
                        };
                        
                        activities.Add(activity);
                        _logger.LogInformation($"Added activity: {activity.Title} for {activity.RelatedEntityName}");
                    }
                }
                else
                {
                    _logger.LogWarning("No accounts found in database");
                }

                // Eğer hiç aktivite yoksa, demo aktivite ekle
                if (!activities.Any())
                {
                    _logger.LogInformation("No real activities found, adding demo activities");
                    var demoActivity = new BusinessActivityItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Welcome to Proposal Management System",
                        Description = "Your dashboard is ready. Start by creating your first proposal or adding a new client!",
                        IconClass = "fas fa-rocket",
                        IconColor = "bg-primary",
                        ActivityDate = now,
                        TimeAgo = "Just now",
                        ActivityType = "System",
                        RelatedEntityName = "Dashboard",
                        Status = "Ready",
                        UserName = "System"
                    };
                    
                    activities.Add(demoActivity);
                    _logger.LogInformation($"Added demo activity: {demoActivity.Title}");
                }

                // Tarihe göre sırala (en yeni en üstte) ve en fazla 10 tane göster
                var result = activities
                    .OrderByDescending(a => a.ActivityDate)
                    .Take(10)
                    .ToList();

                _logger.LogInformation($"Generated {result.Count} business activities total");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating business activities");
                return new List<BusinessActivityItem>
                {
                    new BusinessActivityItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "System Ready",
                        Description = "Dashboard system is operational and ready to track activities",
                        IconClass = "fas fa-check-circle",
                        IconColor = "bg-success",
                        ActivityDate = now,
                        TimeAgo = "Just now",
                        ActivityType = "System",
                        RelatedEntityName = "Dashboard",
                        Status = "Ready",
                        UserName = "System"
                    }
                };
            }
        }

        private string GetTimeAgo(DateTime activityDate, DateTime now)
        {
            var timeSpan = now - activityDate;

            if (timeSpan.TotalMinutes < 1)
            {
                return "Just now";
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                var minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeSpan.TotalHours < 24)
            {
                var hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeSpan.TotalDays < 7)
            {
                var days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else if (timeSpan.TotalDays < 30)
            {
                var weeks = (int)(timeSpan.TotalDays / 7);
                return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";
            }
            else if (timeSpan.TotalDays < 365)
            {
                var months = (int)(timeSpan.TotalDays / 30);
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }
            else
            {
                var years = (int)(timeSpan.TotalDays / 365);
                return years == 1 ? "1 year ago" : $"{years} years ago";
            }
        }
    }
}
