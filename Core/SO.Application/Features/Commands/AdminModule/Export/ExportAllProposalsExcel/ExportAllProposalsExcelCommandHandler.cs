using MediatR;
using SO.Application.Abstractions.Services;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsExcel
{
    public class ExportAllProposalsExcelCommandHandler : IRequestHandler<ExportAllProposalsExcelCommandRequest, ExportAllProposalsExcelCommandResponse>
    {
        private readonly IDocumentExportService _documentExportService;

        public ExportAllProposalsExcelCommandHandler(IDocumentExportService documentExportService)
        {
            _documentExportService = documentExportService;
        }

        public async Task<ExportAllProposalsExcelCommandResponse> Handle(ExportAllProposalsExcelCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var fileBytes = await _documentExportService.ExportAllProposalsToExcelAsync(request.ProposalIds);
                var fileName = $"All_Proposals_{DateTime.Now:yyyyMMdd}.xlsx";

                return new ExportAllProposalsExcelCommandResponse
                {
                    Succeeded = true,
                    FileBytes = fileBytes,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new ExportAllProposalsExcelCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Error exporting all proposals to Excel: {ex.Message}"
                };
            }
        }
    }
}
