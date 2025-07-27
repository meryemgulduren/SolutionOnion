using SO.Application.DTOs.ProposalModule.CustomerBeneficiary;
using SO.Application.DTOs.ProposalModule.Milestone;
using System.Collections.Generic;
namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class UpdateProposalCustomersAndDeliverables
    {
        public string ProposalId { get; set; }
        public List<CreateCustomerBeneficiary> CustomerBeneficiaries { get; set; } = new();
        public List<CreateMilestone> Milestones { get; set; } = new();
    }
}
