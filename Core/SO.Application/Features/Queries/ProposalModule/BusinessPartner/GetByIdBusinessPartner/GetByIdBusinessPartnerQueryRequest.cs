using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.BusinessPartner.GetByIdBusinessPartner
{
    public class GetByIdBusinessPartnerQueryRequest : IRequest<GetByIdBusinessPartnerQueryResponse>
    {
        public string Id { get; set; } = string.Empty;
    }
}
