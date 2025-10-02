using SO.Application.DTOs.ProposalModule.ProposalRisk;

namespace SO.Application.Abstractions.Services.ProposalModule
{
    public interface IProposalRiskService
    {
        Task<List<ProposalRiskDto>> GetRisksByProposalIdAsync(Guid proposalId);
        Task<ProposalRiskDto> CreateRiskAsync(ProposalRiskDto dto);
        Task<ProposalRiskDto> UpdateRiskAsync(ProposalRiskDto dto);
        Task<bool> DeleteRiskAsync(Guid id);
        Task InitializeDefaultRisksAsync(Guid proposalId);
    }
} 