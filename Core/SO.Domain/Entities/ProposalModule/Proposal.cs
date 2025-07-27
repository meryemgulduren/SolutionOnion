using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Common;
using System.Collections.Generic;

namespace SO.Domain.Entities.ProposalModule
{
    public class Proposal : BaseEntity
    {
        public string ProposalName { get; set; } = string.Empty;
        public string PreparedBy { get; set; } = string.Empty;
        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;
        public DateTime ProposalDate { get; set; }
        public string? ProjectDescription { get; set; }
        public string? StatementOfNeed { get; set; }
        public DateTime? ValidUntilDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string? ProjectApproach { get; set; }
        public string? Phasing { get; set; }
        public string? OutsourcingPlans { get; set; }
        public string? Interoperability { get; set; }
        
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; } = null!; // DÜZELTME
        public virtual ICollection<BusinessObjective> BusinessObjectives { get; set; } = new List<BusinessObjective>();
        public virtual ICollection<ProjectStakeholder> ProjectStakeholders { get; set; } = new List<ProjectStakeholder>();
        public virtual ICollection<CustomerBeneficiary> CustomerBeneficiaries { get; set; } = new List<CustomerBeneficiary>();
        public virtual ICollection<ProposalItem> ProposalItems { get; set; } = new List<ProposalItem>();
        public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
        public virtual ICollection<SuccessCriterion> SuccessCriteria { get; set; } = new List<SuccessCriterion>();
        public virtual ICollection<CriticalSuccessFactor> CriticalSuccessFactors { get; set; } = new List<CriticalSuccessFactor>();
        public virtual ICollection<ResourceRequirement> ResourceRequirements { get; set; } = new List<ResourceRequirement>();
    }

    public enum ProposalStatus
    {
        Draft = 1, Sent = 2, Approved = 3, Rejected = 4, Cancelled = 5
    }
}
