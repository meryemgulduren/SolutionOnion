using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.ProposalModule.ResourceRequirement;
using System.Collections.Generic;
namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class UpdateProposalResourceRequirements
    {
        public string ProposalId { get; set; }
        public List<CreateResourceRequirement> ResourceRequirements { get; set; } = new();
    }
}