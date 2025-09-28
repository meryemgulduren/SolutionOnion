using MediatR;

namespace SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsWord
{
    public class ExportAllProposalsWordCommandRequest : IRequest<ExportAllProposalsWordCommandResponse>
    {
        public List<Guid> ProposalIds { get; set; } = new List<Guid>();
    }
}
