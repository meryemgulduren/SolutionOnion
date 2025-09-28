using MediatR;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportProposalWord
{
    public class ExportProposalWordCommandRequest : IRequest<ExportProposalWordCommandResponse>
    {
        public Guid ProposalId { get; set; }
    }
}
