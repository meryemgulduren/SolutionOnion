using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetByIdProposal
{
    // Bu sınıf, GetByIdProposalQueryRequest isteğini alıp işleyecek olan asıl mantığı içerir.
    public class GetByIdProposalQueryHandler : IRequestHandler<GetByIdProposalQueryRequest, GetByIdProposalQueryResponse>
    {
        private readonly IProposalService _proposalService;

        public GetByIdProposalQueryHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<GetByIdProposalQueryResponse> Handle(GetByIdProposalQueryRequest request, CancellationToken cancellationToken)
        {
            // Servis katmanını çağırarak ID'ye göre teklifi getiriyoruz.
            var proposal = await _proposalService.GetProposalByIdAsync(request.Id);

            // Dönen sonucu Response nesnesine atayarak geri döndürüyoruz.
            return new()
            {
                Result = proposal
            };
        }
    }
}
