using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class CreateProposal
    {
        public string AccountId { get; set; }
        public string ProposalName { get; set; }
        public string PreparedBy { get; set; }
        public string? ProjectDescription { get; set; }
    }
}
