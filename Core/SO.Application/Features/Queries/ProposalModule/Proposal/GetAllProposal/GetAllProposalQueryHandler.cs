using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal
{
    public class GetAllProposalQueryHandler : IRequestHandler<GetAllProposalQueryRequest, GetAllProposalQueryResponse>
    {
        private readonly IProposalService _proposalService;

        public GetAllProposalQueryHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<GetAllProposalQueryResponse> Handle(GetAllProposalQueryRequest request, CancellationToken cancellationToken)
        {
            var proposals = await _proposalService.GetAllProposalsAsync();
            return new() { Result = proposals };
        }
    }
}
