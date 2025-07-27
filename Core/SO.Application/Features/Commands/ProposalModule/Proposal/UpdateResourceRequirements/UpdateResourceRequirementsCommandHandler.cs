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
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateResourceRequirements
{
    public class UpdateResourceRequirementsCommandHandler : IRequestHandler<UpdateResourceRequirementsCommandRequest, UpdateResourceRequirementsCommandResponse>
    {
        private readonly IProposalService _proposalService;
        public UpdateResourceRequirementsCommandHandler(IProposalService proposalService) { _proposalService = proposalService; }
        public async Task<UpdateResourceRequirementsCommandResponse> Handle(UpdateResourceRequirementsCommandRequest request, CancellationToken cancellationToken)
        {
            await _proposalService.UpdateProposalResourceRequirementsAsync(new UpdateProposalResourceRequirements
            {
                ProposalId = request.Id,
                ResourceRequirements = request.ResourceRequirements
            });
            return new();
        }
    }
}
