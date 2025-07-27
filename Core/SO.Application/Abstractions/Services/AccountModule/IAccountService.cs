using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.AccountModule.Account;
namespace SO.Application.Abstractions.Services.AccountModule
{
    public interface IAccountService
    {
        Task<List<ListAccount>> GetAllAccountsAsync();
        Task<SingleAccount> GetAccountByIdAsync(string id);
        Task CreateAccountAsync(CreateAccount createAccount);
        Task UpdateAccountAsync(UpdateAccount updateAccount);
        Task DeleteAccountAsync(string id);
    }
}
