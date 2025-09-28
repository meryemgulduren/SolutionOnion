using MediatR;
using SO.Application.Abstractions.Services;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportProposalWord
{
    public class ExportProposalWordCommandHandler : IRequestHandler<ExportProposalWordCommandRequest, ExportProposalWordCommandResponse>
    {
        private readonly IDocumentExportService _documentExportService;

        public ExportProposalWordCommandHandler(IDocumentExportService documentExportService)
        {
            _documentExportService = documentExportService;
        }

        public async Task<ExportProposalWordCommandResponse> Handle(ExportProposalWordCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var proposalIds = new List<Guid> { request.ProposalId };
                var fileBytes = await _documentExportService.ExportAllProposalsToWordAsync(proposalIds);
                var fileName = $"Proposal_{request.ProposalId}_{DateTime.Now:yyyyMMdd}.docx";

                return new ExportProposalWordCommandResponse
                {
                    Succeeded = true,
                    FileBytes = fileBytes,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new ExportProposalWordCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Error exporting proposal to Word: {ex.Message}"
                };
            }
        }
    }
}
