using SO.Application.DTOs.ProposalModule;
using System;
using System.Threading.Tasks;

namespace SO.Application.Interfaces.Services
{
    public interface IDocumentExportService
    {
        Task<byte[]> ExportProposalToWordAsync(Guid proposalId);
        Task<ProposalExportDto> GetProposalExportDataAsync(Guid proposalId);
    }
}