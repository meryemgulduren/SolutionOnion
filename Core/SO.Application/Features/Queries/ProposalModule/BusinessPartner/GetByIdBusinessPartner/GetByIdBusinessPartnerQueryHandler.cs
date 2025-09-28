using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Features.Queries.ProposalModule.BusinessPartner.GetByIdBusinessPartner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.BusinessPartner.GetByIdBusinessPartner
{
    public class GetByIdBusinessPartnerQueryHandler : IRequestHandler<GetByIdBusinessPartnerQueryRequest, GetByIdBusinessPartnerQueryResponse>
    {
        private readonly IBusinessPartnerService _businessPartnerService;

        public GetByIdBusinessPartnerQueryHandler(IBusinessPartnerService businessPartnerService)
        {
            _businessPartnerService = businessPartnerService;
        }

        public async Task<GetByIdBusinessPartnerQueryResponse> Handle(GetByIdBusinessPartnerQueryRequest request, CancellationToken cancellationToken)
        {
            var businessPartner = await _businessPartnerService.GetBusinessPartnerByIdAsync(request.Id);
            return new() { Result = businessPartner };
        }
    }
}
