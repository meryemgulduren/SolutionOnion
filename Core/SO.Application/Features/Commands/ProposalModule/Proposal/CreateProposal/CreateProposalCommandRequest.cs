using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.DTOs.ProposalModule.ProposalItem;
using System.Collections.Generic;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal
{
    public class CreateProposalCommandRequest : IRequest<CreateProposalCommandResponse>
    {
        public string AccountId { get; set; }
        public string ProposalName { get; set; }
        public string PreparedBy { get; set; }
        public List<CreateProposalItem> ProposalItems { get; set; } = new();
    }
}
