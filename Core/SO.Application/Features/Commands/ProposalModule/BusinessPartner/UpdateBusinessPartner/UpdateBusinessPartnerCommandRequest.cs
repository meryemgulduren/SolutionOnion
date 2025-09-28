using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.BusinessPartner.UpdateBusinessPartner
{
    public class UpdateBusinessPartnerCommandRequest : IRequest<UpdateBusinessPartnerCommandResponse>
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        public string? Notes { get; set; }
    }
}
