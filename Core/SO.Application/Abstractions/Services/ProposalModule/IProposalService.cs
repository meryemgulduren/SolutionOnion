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
        Task<List<ListProposal>> GetAllProposalsAsync(string? currentUserId = null, bool isAdmin = false);
        Task<SingleProposal> GetProposalByIdAsync(string id); // EKLENDİ
        Task<string> CreateProposalAsync(CreateProposal createProposal);
        Task UpdateProposalAsync(UpdateProposal updateProposal); // EKLENDİ
        Task UpdateProposalSummaryAsync(UpdateProposalSummary summary);
        Task DeleteProposalAsync(string id); // EKLENDİ

        // Eski adım bazlı metotlar kaldırıldı (tek akışta güncelleniyor)



    }
}
