using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.Proposal.GetByIdProposal
{
    // Bu sınıf, Handler'dan dönecek olan sonucu (tek bir teklifin detaylı DTO'su) tutar.
    public class GetByIdProposalQueryResponse
    {
        public object Result { get; set; }
    }
}
