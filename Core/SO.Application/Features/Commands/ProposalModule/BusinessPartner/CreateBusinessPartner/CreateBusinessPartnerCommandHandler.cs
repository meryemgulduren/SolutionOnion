using SO.Application.Abstractions.Services.ProposalModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.ProposalModule.BusinessPartner.CreateBusinessPartner
{
    public class CreateBusinessPartnerCommandHandler : IRequestHandler<CreateBusinessPartnerCommandRequest, CreateBusinessPartnerCommandResponse>
    {
        private readonly IBusinessPartnerService _businessPartnerService;

        public CreateBusinessPartnerCommandHandler(IBusinessPartnerService businessPartnerService)
        {
            _businessPartnerService = businessPartnerService;
        }

        public async Task<CreateBusinessPartnerCommandResponse> Handle(CreateBusinessPartnerCommandRequest request, CancellationToken cancellationToken)
        {
            await _businessPartnerService.CreateBusinessPartnerAsync(new()
            {
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
