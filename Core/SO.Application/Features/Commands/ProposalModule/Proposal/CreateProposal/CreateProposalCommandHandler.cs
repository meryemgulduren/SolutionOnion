using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal; // Bu satır gereksiz, silebilirsin.

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
// DTO'ya takma ad veriyoruz.
using CreateProposalDTO = SO.Application.DTOs.ProposalModule.Proposal.CreateProposal;
using System.Threading;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal
{
    public class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommandRequest, CreateProposalCommandResponse>
    {
        private readonly IProposalService _proposalService;

        public CreateProposalCommandHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<CreateProposalCommandResponse> Handle(CreateProposalCommandRequest request, CancellationToken cancellationToken)
        {
            // Burada yeni takma adı kullanıyoruz.
            await _proposalService.CreateProposalAsync(new CreateProposalDTO
            {
                AccountId = request.AccountId,
                ProposalName = request.ProposalName,
                PreparedBy = request.PreparedBy,
                ProposalItems = request.ProposalItems
            });
            return new();
        }
    }
}