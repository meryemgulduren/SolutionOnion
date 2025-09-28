using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.CompetitionCompany.DeleteCompetitionCompany
{
    public class DeleteCompetitionCompanyCommandHandler : IRequestHandler<DeleteCompetitionCompanyCommandRequest, DeleteCompetitionCompanyCommandResponse>
    {
        private readonly ICompetitionCompanyService _competitionCompanyService;

        public DeleteCompetitionCompanyCommandHandler(ICompetitionCompanyService competitionCompanyService)
        {
            _competitionCompanyService = competitionCompanyService;
        }

        public async Task<DeleteCompetitionCompanyCommandResponse> Handle(DeleteCompetitionCompanyCommandRequest request, CancellationToken cancellationToken)
        {
            await _competitionCompanyService.DeleteCompetitionCompanyAsync(request.Id);
            return new()
            {
                Succeeded = true
            };
        }
    }
}
