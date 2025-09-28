using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.DeleteProposal
{
    // Bu sınıf, DeleteProposalCommandRequest isteğini alıp işleyecek olan asıl mantığı içerir.
    public class DeleteProposalCommandHandler : IRequestHandler<DeleteProposalCommandRequest, DeleteProposalCommandResponse>
    {
        private readonly IProposalService _proposalService;

        public DeleteProposalCommandHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<DeleteProposalCommandResponse> Handle(DeleteProposalCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Servis katmanını çağırarak ID'ye göre teklifi siliyoruz.
                await _proposalService.DeleteProposalAsync(request.Id);

                // İşlem başarılı olduğunda boş bir response döndürüyoruz.
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete proposal with ID {request.Id}: {ex.Message}", ex);
            }
        }
    }
}
