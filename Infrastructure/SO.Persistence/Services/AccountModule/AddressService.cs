using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.AccountModule.Address;
using SO.Application.Repositories;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Common;
using SO.Domain.Entities.Identity;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Persistence.Services.AccountModule
{
    public class AddressService : IAddressService
    {
        readonly IReadRepository<Address> _addressReadRepository;
        readonly IWriteRepository<Address> _addressWriteRepository;
        private readonly UserManager<AppUser> _userManager;

        public AddressService(IReadRepository<Address> addressReadRepository,
                              IWriteRepository<Address> addressWriteRepository,
                              UserManager<AppUser> userManager)
        {
            _addressReadRepository = addressReadRepository;
            _addressWriteRepository = addressWriteRepository;
            _userManager = userManager;
        }

        public async Task CreateAddressAsync(CreateAddress createAddress)
        {
            await _addressWriteRepository.AddAsync(new()
            {
                AccountId = createAddress.AccountId,
                isDefault = createAddress.isDefault,
                AddressType = createAddress.AddressType,
                AddressName = createAddress.AddressName,
                AddressLine1 = createAddress.AddressLine1,
                AddressLine2 = createAddress.AddressLine2,
                Street = createAddress.Street,
                City = createAddress.City,
                State = createAddress.State,
                Zip = createAddress.Zip,
                Country = createAddress.Country,
                Description = createAddress.Description,
                Phone = createAddress.Phone,
                Fax = createAddress.Fax,
                Mail = createAddress.Mail,
                Active = createAddress.Active

            });
            await _addressWriteRepository.SaveAsync();
        }

        public async Task UpdateAddressAsync(UpdateAddress updateAddress)
        {
            Address address = await _addressReadRepository.GetByIdAsync(updateAddress.Id.ToString());
            address.AccountId = updateAddress.AccountId;
            address.isDefault = updateAddress.isDefault;
            address.AddressType = updateAddress.AddressType;
            address.AddressName = updateAddress.AddressName;
            address.AddressLine1 = updateAddress.AddressLine1;
            address.AddressLine2 = updateAddress.AddressLine2;
            address.Street = updateAddress.Street;
            address.City = updateAddress.City;
            address.State = updateAddress.State;
            address.Zip = updateAddress.Zip;
            address.Country = updateAddress.Country;
            address.Description = updateAddress.Description;
            address.Phone = updateAddress.Phone;
            address.Fax = updateAddress.Fax;
            address.Mail = updateAddress.Mail;
            address.Active = updateAddress.Active;
            await _addressWriteRepository.SaveAsync();
        }

        public async Task DeleteAddressAsync(string id)
        {
            await _addressWriteRepository.RemoveAsync(id);
            await _addressWriteRepository.SaveAsync();
        }

        public async Task<SingleAddress> GetAddressByIdAsync(string id)
        {
            var address = await _addressReadRepository.GetByIdAsync(id);
            
            if (address == null)
            {
                throw new ArgumentException($"Address with ID {id} not found.");
            }
            
            return new()
            {
                Id = address.Id.ToString(),
                AccountId = address.AccountId.ToString(),
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
                Active = address.Active,
                CreatedDate = address.CreatedDate
            };
        }

        public async Task<List<ListAddress>> GetAllAddressesAsync()
        {
            var addresses = _addressReadRepository.GetAll(false).ToList();
            System.Diagnostics.Debug.WriteLine($"AddressService: Found {addresses.Count} addresses in database");
            
            var result = new List<ListAddress>();
            
            foreach (var address in addresses)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(address.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(address.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }
                
                result.Add(new ListAddress
                {
                    Id = address.Id.ToString(),
                    AccountId = address.AccountId.ToString(),
                    AddressName = address.AddressName,
                    AddressType = address.AddressType,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    AddressLine = $"{address.AddressLine1} {address.AddressLine2}".Trim(), // Birleştirilmiş adres
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    Zip = address.Zip,
                    PostalCode = address.Zip, // JavaScript için alias
                    Phone = address.Phone,
                    Fax = address.Fax, // ✅ FAX ALANI EKLENDİ
                    Mail = address.Mail, // ✅ MAIL ALANI EKLENDİ
                    isDefault = address.isDefault,
                    Active = address.Active,
                    CreatedById = address.CreatedById,
                    CreatedBy = createdBy,
                    CreatedDate = address.CreatedDate,
                    CompanyName = "Unknown Company" // TODO: Account ile ilişkilendir
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"AddressService: Returning {result.Count} addresses");
            return result;
        }
    }
}
