using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.DTOs.ProposalModule.ResourceRequirement;
using System.Collections.Generic;
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateResourceRequirements
{
    public class UpdateResourceRequirementsCommandRequest : IRequest<UpdateResourceRequirementsCommandResponse>
    {
        public string Id { get; set; }
        public List<CreateResourceRequirement> ResourceRequirements { get; set; } = new();
    }
}