using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
// using SO.Application.DTOs.ProposalModule.Proposal; // Bu using ifadesi kafa karıştırabilir, tam yol kullanmak daha güvenli.
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposalSummary
{
    public class UpdateProposalSummaryCommandHandler : IRequestHandler<UpdateProposalSummaryCommandRequest, UpdateProposalSummaryCommandResponse>
    {
        private readonly IProposalService _proposalService;

        public UpdateProposalSummaryCommandHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<UpdateProposalSummaryCommandResponse> Handle(UpdateProposalSummaryCommandRequest request, CancellationToken cancellationToken)
        {
            // Controller'dan gelen Request nesnesini, Service katmanının beklediği DTO'ya dönüştürüyoruz.
            // DÜZELTME: DTO'nun tam yolunu (namespace) belirterek isim çakışmasını önlüyoruz.
            await _proposalService.UpdateProposalSummaryAsync(new SO.Application.DTOs.ProposalModule.Proposal.UpdateProposalSummary
            {
                ProposalId = request.Id,
                ProjectDescription = request.ProjectDescription,
                StatementOfNeed = request.StatementOfNeed,
                BusinessObjectives = request.BusinessObjectives
            });

            // İşlem tamamlandığında boş bir cevap döndürüyoruz.
            return new();
        }
    }
}
