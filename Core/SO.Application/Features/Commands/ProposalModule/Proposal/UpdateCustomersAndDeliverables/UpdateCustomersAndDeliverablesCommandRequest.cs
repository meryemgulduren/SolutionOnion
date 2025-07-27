using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.DTOs.ProposalModule.CustomerBeneficiary;
using SO.Application.DTOs.ProposalModule.Milestone;
using System.Collections.Generic;
namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateCustomersAndDeliverables
{
    public class UpdateCustomersAndDeliverablesCommandRequest : IRequest<UpdateCustomersAndDeliverablesCommandResponse>
    {
        public string Id { get; set; }
        public List<CreateCustomerBeneficiary> CustomerBeneficiaries { get; set; } = new();
        public List<CreateMilestone> Milestones { get; set; } = new();
    }
}