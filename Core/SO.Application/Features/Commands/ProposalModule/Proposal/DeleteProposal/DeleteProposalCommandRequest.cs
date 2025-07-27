using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.DeleteProposal
{
    // Bu sınıf, bir teklifi silme isteğini temsil eder.
    // Hangi teklifin silineceğini belirtmek için sadece Id bilgisi yeterlidir.
    public class DeleteProposalCommandRequest : IRequest<DeleteProposalCommandResponse>
    {
        public string Id { get; set; }
    }
}
