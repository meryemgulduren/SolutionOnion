using MediatR;
using SO.Application.Abstractions.Services;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsWord
{
    public class ExportAllProposalsWordCommandHandler : IRequestHandler<ExportAllProposalsWordCommandRequest, ExportAllProposalsWordCommandResponse>
    {
        private readonly IDocumentExportService _documentExportService;

        public ExportAllProposalsWordCommandHandler(IDocumentExportService documentExportService)
        {
            _documentExportService = documentExportService;
        }

        public async Task<ExportAllProposalsWordCommandResponse> Handle(ExportAllProposalsWordCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var fileBytes = await _documentExportService.ExportAllProposalsToWordAsync(request.ProposalIds);
                var fileName = $"All_Proposals_{DateTime.Now:yyyyMMdd}.docx";

                return new ExportAllProposalsWordCommandResponse
                {
                    Succeeded = true,
                    FileBytes = fileBytes,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new ExportAllProposalsWordCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Error exporting all proposals to Word: {ex.Message}"
                };
            }
        }
    }
}
