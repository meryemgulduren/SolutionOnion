namespace SO.Application.DTOs.ProposalModule.ProposalRisk
{
    public class ProposalRiskDto
    {
        public Guid? Id { get; set; }
        public Guid ProposalId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsApplicable { get; set; }
    }
} 