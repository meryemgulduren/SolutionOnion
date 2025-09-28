using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Features.Queries.ProposalModule.BusinessPartner.GetAllBusinessPartner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.BusinessPartner.GetAllBusinessPartner
{
    public class GetAllBusinessPartnerQueryHandler : IRequestHandler<GetAllBusinessPartnerQueryRequest, GetAllBusinessPartnerQueryResponse>
    {
        private readonly IBusinessPartnerService _businessPartnerService;

        public GetAllBusinessPartnerQueryHandler(IBusinessPartnerService businessPartnerService)
        {
            _businessPartnerService = businessPartnerService;
        }

        public async Task<GetAllBusinessPartnerQueryResponse> Handle(GetAllBusinessPartnerQueryRequest request, CancellationToken cancellationToken)
        {
            var allBusinessPartners = await _businessPartnerService.GetAllBusinessPartnersAsync();

            // Admin ise tüm business partner'ları göster
            if (request.IsAdmin)
            {
                return new() { Result = allBusinessPartners };
            }

            // User ise sadece kendi oluşturduğu business partner'ları göster
            if (!string.IsNullOrEmpty(request.CurrentUserId))
            {
                var filteredBusinessPartners = allBusinessPartners?.Where(bp => 
                    bp.CreatedById == request.CurrentUserId).ToList();
                return new() { Result = filteredBusinessPartners ?? new List<SO.Application.DTOs.ProposalModule.BusinessPartner.BusinessPartnerDto>() };
            }
            
            // Kullanıcı ID yoksa boş liste döndür
            return new() { Result = new List<SO.Application.DTOs.ProposalModule.BusinessPartner.BusinessPartnerDto>() };
        }
    }
}
