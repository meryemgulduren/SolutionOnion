using SO.Application.Abstractions.Services.AccountModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Address.GetByIdAddress
{
    public class GetByIdAddressQueryHandler : IRequestHandler<GetByIdAddressQueryRequest, GetByIdAddressQueryResponse>
    {
        readonly IAddressService _addressService;

        public GetByIdAddressQueryHandler(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<GetByIdAddressQueryResponse> Handle(GetByIdAddressQueryRequest request, CancellationToken cancellationToken)
        {
            var address = await _addressService.GetAddressByIdAsync(request.Id);
            return new()
            {
                Id = address.Id,
                AccountId = address.AccountId,
                isDefault = address.isDefault,
                AddressType = address.AddressType,
                AddressName = address.AddressName,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Street = address.Street,
                City = address.City,
                State = address.State,
                Zip = address.Zip,
                Country = address.Country,
                Description = address.Description,
                Phone = address.Phone,
                Fax = address.Fax,
                Mail = address.Mail,
                Active = address.Active
            };
        }
    }
}
