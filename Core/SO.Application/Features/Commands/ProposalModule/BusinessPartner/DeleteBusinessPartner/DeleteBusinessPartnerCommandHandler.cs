using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.BusinessPartner.DeleteBusinessPartner
{
    public class DeleteBusinessPartnerCommandHandler : IRequestHandler<DeleteBusinessPartnerCommandRequest, DeleteBusinessPartnerCommandResponse>
    {
        private readonly IBusinessPartnerService _businessPartnerService;

        public DeleteBusinessPartnerCommandHandler(IBusinessPartnerService businessPartnerService)
        {
            _businessPartnerService = businessPartnerService;
        }

        public async Task<DeleteBusinessPartnerCommandResponse> Handle(DeleteBusinessPartnerCommandRequest request, CancellationToken cancellationToken)
        {
            await _businessPartnerService.DeleteBusinessPartnerAsync(request.Id);
            return new()
            {
                Succeeded = true
            };
        }
    }
}
