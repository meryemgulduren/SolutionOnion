using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.ProposalModule.ProjectStakeholder;
using System.Collections.Generic;
namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class UpdateProposalInitiatorSponsor
    {
        public string ProposalId { get; set; }
        public List<CreateProjectStakeholder> Stakeholders { get; set; } = new();
    }
}