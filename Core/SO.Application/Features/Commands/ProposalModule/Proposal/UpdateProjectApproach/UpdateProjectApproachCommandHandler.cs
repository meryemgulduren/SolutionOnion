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

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProjectApproach
{
    public class UpdateProjectApproachCommandHandler : IRequestHandler<UpdateProjectApproachCommandRequest, UpdateProjectApproachCommandResponse>
    {
        private readonly IProposalService _proposalService;
        public UpdateProjectApproachCommandHandler(IProposalService proposalService) { _proposalService = proposalService; }

        public async Task<UpdateProjectApproachCommandResponse> Handle(UpdateProjectApproachCommandRequest request, CancellationToken cancellationToken)
        {
            await _proposalService.UpdateProposalApproachAsync(new UpdateProposalApproach
            {
                ProposalId = request.Id,
                Phasing = request.Phasing,
                OutsourcingPlans = request.OutsourcingPlans,
                Interoperability = request.Interoperability,
                Milestones = request.Milestones
            });
            return new();
        }
    }
}