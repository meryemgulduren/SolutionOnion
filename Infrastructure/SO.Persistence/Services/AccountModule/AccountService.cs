using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.AccountModule.Account;
using SO.Application.Repositories.AccountModule;
using SO.Domain.Entities.AccountModule;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Persistence.Services.AccountModule
{
    public class AccountService : IAccountService
    {
        private readonly IAccountReadRepository _accountReadRepository;
        private readonly IAccountWriteRepository _accountWriteRepository;

        public AccountService(IAccountReadRepository accountReadRepository, IAccountWriteRepository accountWriteRepository)
        {
            _accountReadRepository = accountReadRepository;
            _accountWriteRepository = accountWriteRepository;
        }

        public async Task CreateAccountAsync(CreateAccount createAccount)
        {
            await _accountWriteRepository.AddAsync(new()
            {
                CompanyName = createAccount.CompanyName,
                ContactPerson = createAccount.ContactPerson,
                Email = createAccount.Email,
                PhoneNumber = createAccount.PhoneNumber,
                IsActive = true
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
                IsActive = account.IsActive
            };
        }

        public Task<List<ListAccount>> GetAllAccountsAsync()
        {
            var accounts = _accountReadRepository.GetAll(false)
                .Select(a => new ListAccount
                {
                    Id = a.Id,
                    CompanyName = a.CompanyName,
                    ContactPerson = a.ContactPerson,
                    Email = a.Email
                }).ToList();
            return Task.FromResult(accounts);
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
                account.IsActive = updateAccount.IsActive;
                _accountWriteRepository.Update(account);
                await _accountWriteRepository.SaveAsync();
            }
        }
    }
}