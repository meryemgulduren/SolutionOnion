using SO.Application.DTOs.ProposalModule.BusinessPartner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Abstractions.Services.ProposalModule
{
    public interface IBusinessPartnerService
    {
        Task CreateBusinessPartnerAsync(CreateBusinessPartner createBusinessPartner);
        Task UpdateBusinessPartnerAsync(UpdateBusinessPartner updateBusinessPartner);
        Task DeleteBusinessPartnerAsync(string id);
        Task<List<BusinessPartnerDto>> GetAllBusinessPartnersAsync();
        Task<BusinessPartnerDto> GetBusinessPartnerByIdAsync(string id);
        Task<List<BusinessPartnerDto>> GetBusinessPartnersByProposalIdAsync(string proposalId);
    }
}
