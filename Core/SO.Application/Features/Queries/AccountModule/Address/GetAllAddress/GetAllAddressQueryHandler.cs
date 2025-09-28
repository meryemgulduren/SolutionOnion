using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.Features.Queries.AccountModule.Address.GetAllAddress;
 
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
            var allAddresses = await _addressService.GetAllAddressesAsync();

            // Admin ise tüm address'leri göster
            if (request.IsAdmin)
            {
                return new() { Result = allAddresses };
            }

            // User ise sadece kendi oluşturduğu address'leri göster
            if (!string.IsNullOrEmpty(request.CurrentUserId))
            {
                var filteredAddresses = allAddresses?.Where(a => 
                    a.CreatedById == request.CurrentUserId).ToList();
                return new() { Result = filteredAddresses ?? new List<SO.Application.DTOs.AccountModule.Address.ListAddress>() };
            }
            
            // Kullanıcı ID yoksa boş liste döndür
            return new() { Result = new List<SO.Application.DTOs.AccountModule.Address.ListAddress>() };
        }
    }
}



