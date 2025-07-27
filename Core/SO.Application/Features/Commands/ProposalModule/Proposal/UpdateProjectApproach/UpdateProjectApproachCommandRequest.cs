using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.DTOs.ProposalModule.Milestone;
using System.Collections.Generic;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProjectApproach
{
    public class UpdateProjectApproachCommandRequest : IRequest<UpdateProjectApproachCommandResponse>
    {
        public string Id { get; set; }
        public string? Phasing { get; set; }
        public string? OutsourcingPlans { get; set; }
        public string? Interoperability { get; set; }
        public List<CreateMilestone> Milestones { get; set; } = new();
    }
}