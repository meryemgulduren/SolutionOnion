using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class CompetitionCompany : BaseEntity
    {
        public Guid ProposalId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal? CompetedPrice { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public virtual Proposal Proposal { get; set; } = null!;
    }
}
