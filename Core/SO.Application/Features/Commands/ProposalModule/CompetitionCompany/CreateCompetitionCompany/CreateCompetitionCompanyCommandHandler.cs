using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.CompetitionCompany.CreateCompetitionCompany
{
    public class CreateCompetitionCompanyCommandHandler : IRequestHandler<CreateCompetitionCompanyCommandRequest, CreateCompetitionCompanyCommandResponse>
    {
        private readonly ICompetitionCompanyService _competitionCompanyService;

        public CreateCompetitionCompanyCommandHandler(ICompetitionCompanyService competitionCompanyService)
        {
            _competitionCompanyService = competitionCompanyService;
        }

        public async Task<CreateCompetitionCompanyCommandResponse> Handle(CreateCompetitionCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            await _competitionCompanyService.CreateCompetitionCompanyAsync(new()
            {
                ProposalId = request.ProposalId,
                CompanyName = request.CompanyName,
                CompetedPrice = request.CompetedPrice,
                Notes = request.Notes
            });
            return new()
            {
                Succeeded = true
            };
        }
    }
}
