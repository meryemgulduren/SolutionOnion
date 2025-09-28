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
            try
            {
                // Validation
                if (string.IsNullOrEmpty(request.AccountId))
                {
                    throw new ArgumentException("Account ID cannot be null or empty");
                }

                if (string.IsNullOrEmpty(request.ProposalName))
                {
                    throw new ArgumentException("Proposal name cannot be null or empty");
                }

                if (string.IsNullOrEmpty(request.PreparedBy))
                {
                    throw new ArgumentException("Prepared by cannot be null or empty");
                }

                // Validate AccountId is a valid GUID
                if (!Guid.TryParse(request.AccountId, out _))
                {
                    throw new ArgumentException("Account ID must be a valid GUID");
                }

                await _proposalService.CreateProposalAsync(new CreateProposalDTO
                {
                    AccountId = request.AccountId,
                    ProposalName = request.ProposalName,
                    PreparedBy = request.PreparedBy,
                    ProjectDescription = request.ProjectDescription
                });
                
                return new();
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Validation error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Proposal creation failed: {ex.Message}", ex);
            }
        }
    }
}