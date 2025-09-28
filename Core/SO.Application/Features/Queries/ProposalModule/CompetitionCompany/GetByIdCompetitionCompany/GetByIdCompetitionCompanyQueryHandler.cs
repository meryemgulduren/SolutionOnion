using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetByIdCompetitionCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetByIdCompetitionCompany
{
    public class GetByIdCompetitionCompanyQueryHandler : IRequestHandler<GetByIdCompetitionCompanyQueryRequest, GetByIdCompetitionCompanyQueryResponse>
    {
        private readonly ICompetitionCompanyService _competitionCompanyService;

        public GetByIdCompetitionCompanyQueryHandler(ICompetitionCompanyService competitionCompanyService)
        {
            _competitionCompanyService = competitionCompanyService;
        }

        public async Task<GetByIdCompetitionCompanyQueryResponse> Handle(GetByIdCompetitionCompanyQueryRequest request, CancellationToken cancellationToken)
        {
            var competitionCompany = await _competitionCompanyService.GetCompetitionCompanyByIdAsync(request.Id);
            return new() { Result = competitionCompany };
        }
    }
}
