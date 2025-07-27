using SO.Application.Abstractions.Services.AccountModule;
using SO.Domain.Entities.AccountModule;
using MediatR;
using Microsoft.AspNetCore.Server.IISIntegration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Address.CreateAddress
{
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommandRequest, CreateAddressCommandResponse>
    {
        readonly IAddressService _addressService;

        public CreateAddressCommandHandler(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<CreateAddressCommandResponse> Handle(CreateAddressCommandRequest request, CancellationToken cancellationToken)
        {
            await _addressService.CreateAddressAsync(new()
            {
                AccountId = request.AccountId,
                isDefault = request.isDefault,
                AddressType = request.AddressType,
                AddressName = request.AddressName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                Country = request.Country,
                Description = request.Description,
                Phone = request.Phone,
                Fax = request.Fax,
                Mail = request.Mail,
                Active = request.Active
            });
            return new()
            {

            };
        }
    }
}
