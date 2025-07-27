using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.Features.Queries.AccountModule.Address.GetAllAddress;
using SO.Application.Repositories.AccountModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Address.GetAllAddress
{
    public class GetAllAddressQueryHandler : IRequestHandler<GetAllAddressQueryRequest, GetAllAddressQueryResponse>
    {
        readonly IAddressService _addressService;

        public GetAllAddressQueryHandler(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<GetAllAddressQueryResponse> Handle(GetAllAddressQueryRequest request, CancellationToken cancellationToken)
        {
            var address = await _addressService.GetAllAddressesAsync();
            return new()
            {
                Result = address
            };
        }
    }
}



