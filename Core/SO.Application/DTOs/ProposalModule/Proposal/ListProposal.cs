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
        public string ProposalCode { get; set; }
        public string ProposalName { get; set; }
        public string CompanyName { get; set; }
        public DateTime ProposalDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PreparedBy { get; set; }
        public string? CreatedById { get; set; } // Kullanıcı ID'si
    }
}
