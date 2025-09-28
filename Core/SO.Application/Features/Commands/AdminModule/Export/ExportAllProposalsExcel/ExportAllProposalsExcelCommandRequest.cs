using MediatR;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsExcel
{
    public class ExportAllProposalsExcelCommandRequest : IRequest<ExportAllProposalsExcelCommandResponse>
    {
        public List<Guid> ProposalIds { get; set; } = new List<Guid>();
    }
}
