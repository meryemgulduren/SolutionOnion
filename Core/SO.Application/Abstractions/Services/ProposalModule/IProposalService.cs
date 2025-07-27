using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.ProposalModule.Proposal;
using System.Collections.Generic;
using System.Threading.Tasks;


using System.Collections.Generic;
using System.Threading.Tasks;

namespace SO.Application.Abstractions.Services.ProposalModule
{
    public interface IProposalService
    {
        Task<List<ListProposal>> GetAllProposalsAsync();
        Task<SingleProposal> GetProposalByIdAsync(string id); // EKLENDİ
        Task<string> CreateProposalAsync(CreateProposal createProposal);
        Task UpdateProposalAsync(UpdateProposal updateProposal); // EKLENDİ
        Task UpdateProposalSummaryAsync(UpdateProposalSummary summary);
        Task DeleteProposalAsync(string id); // EKLENDİ

        Task UpdateProposalInitiatorSponsorAsync(UpdateProposalInitiatorSponsor initiatorSponsor);

        Task UpdateProposalCustomersAndDeliverablesAsync(UpdateProposalCustomersAndDeliverables dto);
        Task UpdateProposalApproachAsync(UpdateProposalApproach dto);
        Task UpdateProposalResourceRequirementsAsync(UpdateProposalResourceRequirements dto);



    }
}
