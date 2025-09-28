using SO.Application.DTOs.AccountModule.Address;
using SO.Domain.Entities.AccountModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Abstractions.Services.AccountModule
{
    public interface IAddressService
    {
        Task CreateAddressAsync(CreateAddress createAddress);
        Task UpdateAddressAsync(UpdateAddress updateAddress);
        Task DeleteAddressAsync(string id);
        Task<List<ListAddress>> GetAllAddressesAsync();
        Task<SingleAddress> GetAddressByIdAsync(string id);
    }
}
