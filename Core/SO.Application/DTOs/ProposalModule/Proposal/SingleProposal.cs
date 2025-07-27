using SO.Application.DTOs.ProposalModule.BusinessObjective;
using SO.Application.DTOs.ProposalModule.CriticalSuccessFactor;
using SO.Application.DTOs.ProposalModule.CustomerBeneficiary;
using SO.Application.DTOs.ProposalModule.Milestone;
using SO.Application.DTOs.ProposalModule.ProjectStakeholder;
using SO.Application.DTOs.ProposalModule.ProposalItem;
using SO.Application.DTOs.ProposalModule.ResourceRequirement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class SingleProposal
    {
        public Guid Id { get; set; }
        public string ProposalName { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public DateTime ProposalDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string? Description { get; set; }
        public string? ProjectApproach { get; set; }
       
        public string? ProjectDescription { get; set; }
        public string? Phasing { get; set; }
        public string? OutsourcingPlans { get; set; }
        public string? Interoperability { get; set; }
        public string? StatementOfNeed { get; set; }
        public List<ListBusinessObjective> BusinessObjectives { get; set; }
        // İlişkili listeler
        public List<ListProposalItem> ProposalItems { get; set; }
        public List<ListMilestone> Milestones { get; set; }
        
        public List<ListCriticalSuccessFactor> CriticalSuccessFactors { get; set; }
        public List<ListProjectStakeholder> ProjectStakeholders { get; set; }
        public List<ListCustomerBeneficiary> CustomerBeneficiaries { get; set; }
        public List<ListResourceRequirement> ResourceRequirements { get; set; }

    }
}
