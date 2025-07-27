using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.Repositories.AccountModule;
using SO.Domain.Entities.AccountModule;
using SO.Persistence.Contexts;

namespace SO.Persistence.Repositories.AccountModule
{
    public class AccountWriteRepository : WriteRepository<Account>, IAccountWriteRepository
    {
        public AccountWriteRepository(SODbContext context) : base(context)
        {
        }
    }
}
