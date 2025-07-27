using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.ProposalModule.BusinessObjective;
using System.Collections.Generic;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    // Bu DTO, sadece Project Summary adımından gelen verileri taşımak için kullanılır.
    public class UpdateProposalSummary
    {
        public string ProposalId { get; set; }
        public string? ProjectDescription { get; set; }
        public string? StatementOfNeed { get; set; }
        public List<CreateBusinessObjective> BusinessObjectives { get; set; } = new();
    }
}