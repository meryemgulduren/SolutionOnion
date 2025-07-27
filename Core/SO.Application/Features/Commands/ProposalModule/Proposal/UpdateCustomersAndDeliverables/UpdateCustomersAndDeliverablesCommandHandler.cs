using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.Proposal;
using System.Threading;
using System.Threading.Tasks;
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateCustomersAndDeliverables
{
    public class UpdateCustomersAndDeliverablesCommandHandler : IRequestHandler<UpdateCustomersAndDeliverablesCommandRequest, UpdateCustomersAndDeliverablesCommandResponse>
    {
        private readonly IProposalService _proposalService;
        public UpdateCustomersAndDeliverablesCommandHandler(IProposalService proposalService) { _proposalService = proposalService; }
        public async Task<UpdateCustomersAndDeliverablesCommandResponse> Handle(UpdateCustomersAndDeliverablesCommandRequest request, CancellationToken cancellationToken)
        {
            await _proposalService.UpdateProposalCustomersAndDeliverablesAsync(new UpdateProposalCustomersAndDeliverables
            {
                ProposalId = request.Id,
                CustomerBeneficiaries = request.CustomerBeneficiaries,
                Milestones = request.Milestones
            });
            return new();
        }
    }
}
