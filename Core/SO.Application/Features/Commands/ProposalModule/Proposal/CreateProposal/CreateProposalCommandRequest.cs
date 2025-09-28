using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using MediatR;
using System.Collections.Generic;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal
{
    public class CreateProposalCommandRequest : IRequest<CreateProposalCommandResponse>
    {
        [Required(ErrorMessage = "Account ID is required")]
        public string AccountId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Proposal name is required")]
        public string ProposalName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Prepared by is required")]
        public string PreparedBy { get; set; } = string.Empty;
        
        public string? ProjectDescription { get; set; }
    }
}
