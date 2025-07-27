using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class ListProposal
    {
        public Guid Id { get; set; }
        public string ProposalName { get; set; }
        public string CompanyName { get; set; }
        public DateTime ProposalDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
