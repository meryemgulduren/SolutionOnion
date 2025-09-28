using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetAllCompetitionCompany
{
    public class GetAllCompetitionCompanyQueryRequest : IRequest<GetAllCompetitionCompanyQueryResponse>
    {
        public string? CurrentUserId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
