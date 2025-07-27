using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.DTOs.ProposalModule.ProjectStakeholder;
using System.Collections.Generic;
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateInitiatorSponsor
{
    public class UpdateInitiatorSponsorCommandRequest : IRequest<UpdateInitiatorSponsorCommandResponse>
    {
        public string Id { get; set; }
        public List<CreateProjectStakeholder> Stakeholders { get; set; } = new();
    }
}