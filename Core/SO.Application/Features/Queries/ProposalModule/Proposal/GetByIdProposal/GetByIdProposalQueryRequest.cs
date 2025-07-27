using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetByIdProposal
{
    // Bu sınıf, tek bir teklifi getirme isteğini ve bunun için gereken Id bilgisini temsil eder.
    public class GetByIdProposalQueryRequest : IRequest<GetByIdProposalQueryResponse>
    {
        public string Id { get; set; }
    }
}