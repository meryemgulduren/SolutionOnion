using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.CompetitionCompany.UpdateCompetitionCompany
{
    public class UpdateCompetitionCompanyCommandHandler : IRequestHandler<UpdateCompetitionCompanyCommandRequest, UpdateCompetitionCompanyCommandResponse>
    {
        private readonly ICompetitionCompanyService _competitionCompanyService;

        public UpdateCompetitionCompanyCommandHandler(ICompetitionCompanyService competitionCompanyService)
        {
            _competitionCompanyService = competitionCompanyService;
        }

        public async Task<UpdateCompetitionCompanyCommandResponse> Handle(UpdateCompetitionCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            await _competitionCompanyService.UpdateCompetitionCompanyAsync(new()
            {
                Id = request.Id,
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
