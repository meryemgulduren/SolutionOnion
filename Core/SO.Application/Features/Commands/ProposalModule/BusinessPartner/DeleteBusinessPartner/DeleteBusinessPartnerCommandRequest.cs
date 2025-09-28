using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.BusinessPartner.DeleteBusinessPartner
{
    public class DeleteBusinessPartnerCommandRequest : IRequest<DeleteBusinessPartnerCommandResponse>
    {
        public string Id { get; set; } = string.Empty;
    }
}
