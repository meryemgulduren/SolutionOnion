using SO.Application.DTOs.ProposalModule;
using SO.Application.DTOs.Dashboard;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.DTOs.AccountModule.Account;

namespace SO.Application.Abstractions.Services
{
    public interface IDocumentExportService
    {
        Task<ProposalExportDto> GetProposalExportDataAsync(Guid proposalId);
        Task<byte[]> ExportProposalToWordAsync(Guid proposalId);
        Task<byte[]> ExportAllProposalsToWordAsync(List<Guid> proposalIds);
        Task<byte[]> ExportAllProposalsToExcelAsync(List<Guid> proposalIds);
        Task<byte[]> ExportDashboardToExcelAsync(DashboardStatisticsDto dashboardData);
        Task<byte[]> ExportProposalsToExcelAsync(List<ListProposal> proposals);
        Task<byte[]> ExportAccountsToExcelAsync(List<ListAccount> accounts);
        Task<byte[]> ExportAllDataToExcelAsync(DashboardStatisticsDto dashboardData, List<ListProposal> proposals, List<ListAccount> accounts);
        Task<byte[]> ExportDashboardToPdfAsync(DashboardStatisticsDto dashboardData);
        Task<byte[]> ExportProposalsToPdfAsync(List<ListProposal> queries);
        Task<byte[]> ExportProposalsToCsvAsync(List<ListProposal> proposals);
        Task<byte[]> ExportAccountsToCsvAsync(List<ListAccount> accounts);
    }
}
