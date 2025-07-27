using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal
{
    public class GetAllProposalQueryRequest : IRequest<GetAllProposalQueryResponse>
    {
        // Tüm teklifleri listeleyeceğimiz için bu isteğin şimdilik bir parametreye ihtiyacı yok.
    }
}
