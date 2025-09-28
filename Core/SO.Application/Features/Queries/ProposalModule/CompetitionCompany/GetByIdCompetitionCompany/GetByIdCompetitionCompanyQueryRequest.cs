using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetByIdCompetitionCompany
{
    public class GetByIdCompetitionCompanyQueryRequest : IRequest<GetByIdCompetitionCompanyQueryResponse>
    {
        public string Id { get; set; } = string.Empty;
    }
}
