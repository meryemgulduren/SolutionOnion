using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.CompetitionCompany.DeleteCompetitionCompany
{
    public class DeleteCompetitionCompanyCommandRequest : IRequest<DeleteCompetitionCompanyCommandResponse>
    {
        public string Id { get; set; } = string.Empty;
    }
}
