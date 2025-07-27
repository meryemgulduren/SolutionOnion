using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Application.DTOs.ProposalModule.Milestone;
using System.Collections.Generic;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    // Bu DTO, sadece Project Approach adımından gelen verileri taşımak için kullanılır.
    public class UpdateProposalApproach
    {
        public string ProposalId { get; set; }
        public string? Phasing { get; set; }
        public string? OutsourcingPlans { get; set; }
        public string? Interoperability { get; set; }
        public List<CreateMilestone> Milestones { get; set; } = new();
    }
}
