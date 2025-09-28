using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal
{
    public class GetAllProposalQueryHandler : IRequestHandler<GetAllProposalQueryRequest, GetAllProposalQueryResponse>
    {
        private readonly IProposalService _proposalService;

        public GetAllProposalQueryHandler(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        public async Task<GetAllProposalQueryResponse> Handle(GetAllProposalQueryRequest request, CancellationToken cancellationToken)
        {
            // Service'e kullanıcı bilgilerini gönder, filtreleme veritabanı seviyesinde yapılsın
            var proposals = await _proposalService.GetAllProposalsAsync(request.CurrentUserId, request.IsAdmin);
            
            return new() { Result = proposals ?? new List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>() };
        }
    }
}
