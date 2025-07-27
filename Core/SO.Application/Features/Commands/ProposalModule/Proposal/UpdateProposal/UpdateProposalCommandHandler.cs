using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.DTOs.ProposalModule.Proposal;
using System.Threading;
using System.Threading.Tasks;
using SO.Application.Abstractions.Services.ProposalModule;
// using SO.Application.DTOs.ProposalModule.Proposal; // Bu satır yerine tam namespace kullanılacak


namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposal
{
    public class UpdateProposalCommandHandler : IRequestHandler<UpdateProposalCommandRequest, UpdateProposalCommandResponse>
    {
        private readonly IProposalService _proposalService;

        public UpdateProposalCommandHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<UpdateProposalCommandResponse> Handle(UpdateProposalCommandRequest request, CancellationToken cancellationToken)
        {
            // DÜZELTME: DTO'nun tam yolunu (namespace) belirterek isim çakışmasını önlüyoruz.
            await _proposalService.UpdateProposalAsync(new SO.Application.DTOs.ProposalModule.Proposal.UpdateProposal
            {
                Id = request.Id,
                ProposalName = request.ProposalName,
                Status = request.Status
            });

            return new();
        }
    }
}