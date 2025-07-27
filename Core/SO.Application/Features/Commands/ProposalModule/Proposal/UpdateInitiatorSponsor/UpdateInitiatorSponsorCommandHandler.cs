using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.Proposal;
using System.Threading;
using System.Threading.Tasks;
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateInitiatorSponsor
{
    public class UpdateInitiatorSponsorCommandHandler : IRequestHandler<UpdateInitiatorSponsorCommandRequest, UpdateInitiatorSponsorCommandResponse>
    {
        private readonly IProposalService _proposalService;
        public UpdateInitiatorSponsorCommandHandler(IProposalService proposalService) { _proposalService = proposalService; }
        public async Task<UpdateInitiatorSponsorCommandResponse> Handle(UpdateInitiatorSponsorCommandRequest request, CancellationToken cancellationToken)
        {
            await _proposalService.UpdateProposalInitiatorSponsorAsync(new UpdateProposalInitiatorSponsor
            {
                ProposalId = request.Id,
                Stakeholders = request.Stakeholders
            });
            return new();
        }
    }
}
