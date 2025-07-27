using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.DTOs.ProposalModule.BusinessObjective;
using System.Collections.Generic;

using MediatR;
using SO.Application.DTOs.ProposalModule.BusinessObjective;
using System.Collections.Generic;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposalSummary
{
    public class UpdateProposalSummaryCommandRequest : IRequest<UpdateProposalSummaryCommandResponse>
    {
        public string Id { get; set; }
        public string? ProjectDescription { get; set; }
        public string? StatementOfNeed { get; set; }
        // DİKKAT: Bu özelliğin adının formdaki 'name' attribute'u ile ('BusinessObjectives') eşleşmesi kritik.
        public List<CreateBusinessObjective> BusinessObjectives { get; set; } = new();
    }
}