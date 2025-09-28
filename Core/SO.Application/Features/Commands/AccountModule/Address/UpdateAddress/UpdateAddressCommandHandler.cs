using SO.Application.Abstractions.Services.AccountModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Application.Features.Commands.AccountModule.Address.UpdateAddress;

namespace SO.Application.Features.Commands.AccountModule.Address.UpdateAddress
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommandRequest, UpdateAddressCommandResponse>
    {
        readonly IAddressService _addressService;

        public UpdateAddressCommandHandler(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<UpdateAddressCommandResponse> Handle(UpdateAddressCommandRequest request, CancellationToken cancellationToken)
        {
            await _addressService.UpdateAddressAsync(new()
            {
                Id = request.Id,
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
                Succeeded = true
            };
        }
    }
}
