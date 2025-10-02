using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class ProposalRisk : BaseEntity
    {
        public Guid ProposalId { get; set; }
        public string Title { get; set; } = string.Empty; // e.g., "Planlama", "Projelendirme", etc.
        public string? Description { get; set; } // User-entered description
        public bool? IsApplicable { get; set; } // Whether this risk is applicable
        
        // Navigation property
        public virtual Proposal Proposal { get; set; } = null!;
    }
} 