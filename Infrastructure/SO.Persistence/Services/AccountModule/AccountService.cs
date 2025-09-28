using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.AccountModule.Account;
using SO.Application.Repositories;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Persistence.Services.AccountModule
{
    public class AccountService : IAccountService
    {
        private readonly IReadRepository<Account> _accountReadRepository;
        private readonly IWriteRepository<Account> _accountWriteRepository;
        private readonly UserManager<AppUser> _userManager;

        public AccountService(IReadRepository<Account> accountReadRepository, IWriteRepository<Account> accountWriteRepository, UserManager<AppUser> userManager)
        {
            _accountReadRepository = accountReadRepository;
            _accountWriteRepository = accountWriteRepository;
            _userManager = userManager;
        }

        public async Task CreateAccountAsync(CreateAccount createAccount)
        {
            await _accountWriteRepository.AddAsync(new()
            {
                CompanyName = createAccount.CompanyName,
                ContactPerson = createAccount.ContactPerson,
                Email = createAccount.Email,
                PhoneNumber = createAccount.PhoneNumber,
                TaxOffice = createAccount.TaxOffice,
                TaxNumber = createAccount.TaxNumber,
                IsActive = createAccount.IsActive
            });
            await _accountWriteRepository.SaveAsync();
        }

        public async Task DeleteAccountAsync(string id)
        {
            await _accountWriteRepository.RemoveAsync(id);
            await _accountWriteRepository.SaveAsync();
        }

        public async Task<SingleAccount> GetAccountByIdAsync(string id)
        {
            var account = await _accountReadRepository.GetByIdAsync(id);
            // Not: Gerçek bir projede, account null ise bir hata fırlatmak daha doğrudur.
            return new SingleAccount
            {
                Id = account.Id.ToString(),
                CompanyName = account.CompanyName,
                ContactPerson = account.ContactPerson,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                TaxOffice = account.TaxOffice,
                TaxNumber = account.TaxNumber,
                IsActive = account.IsActive,
                CreatedDate = account.CreatedDate
            };
        }

        public async Task<List<ListAccount>> GetAllAccountsAsync()
        {
            var accounts = _accountReadRepository.GetAll(false).ToList();
            System.Diagnostics.Debug.WriteLine($"AccountService: Found {accounts.Count} accounts in database");
            
            var result = new List<ListAccount>();
            
            foreach (var account in accounts)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(account.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(account.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }
                
                result.Add(new ListAccount
                {
                    Id = account.Id,
                    CompanyName = account.CompanyName,
                    ContactPerson = account.ContactPerson,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    Phone = account.PhoneNumber, // JavaScript için alias
                    TaxOffice = account.TaxOffice,
                    TaxNumber = account.TaxNumber,
                    IsActive = account.IsActive,
                    Status = account.IsActive ? "Active" : "Inactive", // Status string
                    CreatedDate = account.CreatedDate,
                    CreatedById = account.CreatedById,
                    CreatedBy = createdBy
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"AccountService: Returning {result.Count} accounts");
            return result;
        }

        public async Task UpdateAccountAsync(UpdateAccount updateAccount)
        {
            var account = await _accountReadRepository.GetByIdAsync(updateAccount.Id);
            if (account != null)
            {
                account.CompanyName = updateAccount.CompanyName;
                account.ContactPerson = updateAccount.ContactPerson;
                account.Email = updateAccount.Email;
                account.PhoneNumber = updateAccount.PhoneNumber;
                account.TaxOffice = updateAccount.TaxOffice;
                account.TaxNumber = updateAccount.TaxNumber;
                account.IsActive = updateAccount.IsActive;
                _accountWriteRepository.Update(account);
                await _accountWriteRepository.SaveAsync();
            }
        }
    }
}