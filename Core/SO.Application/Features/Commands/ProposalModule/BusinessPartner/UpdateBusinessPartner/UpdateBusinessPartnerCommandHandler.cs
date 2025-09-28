using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.BusinessPartner.UpdateBusinessPartner
{
    public class UpdateBusinessPartnerCommandHandler : IRequestHandler<UpdateBusinessPartnerCommandRequest, UpdateBusinessPartnerCommandResponse>
    {
        private readonly IBusinessPartnerService _businessPartnerService;

        public UpdateBusinessPartnerCommandHandler(IBusinessPartnerService businessPartnerService)
        {
            _businessPartnerService = businessPartnerService;
        }

        public async Task<UpdateBusinessPartnerCommandResponse> Handle(UpdateBusinessPartnerCommandRequest request, CancellationToken cancellationToken)
        {
            await _businessPartnerService.UpdateBusinessPartnerAsync(new()
            {
                Id = request.Id,
                ProposalId = request.ProposalId,
                PartnerName = request.PartnerName,
                Role = request.Role,
                ContactInfo = request.ContactInfo,
                Notes = request.Notes
            });
            return new()
            {
                Succeeded = true
            };
        }
    }
}
