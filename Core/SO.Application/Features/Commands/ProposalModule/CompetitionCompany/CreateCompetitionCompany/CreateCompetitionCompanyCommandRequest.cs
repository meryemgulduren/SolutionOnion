using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.CompetitionCompany.CreateCompetitionCompany
{
    public class CreateCompetitionCompanyCommandRequest : IRequest<CreateCompetitionCompanyCommandResponse>
    {
        public Guid ProposalId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal? CompetedPrice { get; set; }
        public string? Notes { get; set; }
    }
}
