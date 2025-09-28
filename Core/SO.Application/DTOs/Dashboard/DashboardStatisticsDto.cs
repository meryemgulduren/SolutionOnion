namespace SO.Application.DTOs.Dashboard
{
    public class DashboardStatisticsDto
    {
        public int TotalProposalsCount { get; set; }
        public int ActiveClientsCount { get; set; }
        public int CompletedProjectsCount { get; set; }
        public int DraftProposalsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<BusinessActivityItem> BusinessActivities { get; set; } = new List<BusinessActivityItem>();
    }

    public class BusinessActivityItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty; // "Proposal", "Account", "Update", "Completion"
        public string RelatedEntityName { get; set; } = string.Empty; // Proposal name, Company name, etc.
        public string Status { get; set; } = string.Empty; // "Created", "Updated", "Completed", "Deleted"
        public string UserName { get; set; } = string.Empty; // Who performed the action
    }
}
