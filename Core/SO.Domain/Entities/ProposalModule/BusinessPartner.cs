using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class BusinessPartner : BaseEntity
    {
        public Guid ProposalId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public virtual Proposal Proposal { get; set; } = null!;
    }
}
